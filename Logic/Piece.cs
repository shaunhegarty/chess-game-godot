using Godot;

using System;
using System.Collections.Generic;

using Rules;

namespace Chess
{
    public class Piece
    {
        // Inputs
        public PieceType type;
        public Team TeamColor;
        public Movement PieceMovement { get; private set; }

        public ChessGame Game;

        // State
        public Square CurrentSquare;
        public int MoveCount { get { return movesList.Count; } }
        public List<Square> allowedSquares;

        private List<Move> movesList = new();

        // Property
        public bool IsKing { get { return type == PieceType.King; } }
        public bool EnPassantable
        {
            get
            {
                // Must be a pawn, who moved 2 spaces, 1 turn ago.
                if (type != PieceType.Pawn)
                {
                    return false;
                }
                var lastMove = LastMove();
                if (lastMove != null)
                {
                    bool oneTurnAgo = Game.Turn - lastMove.Turn == 1;
                    return type == PieceType.Pawn && MoveCount > 0 && oneTurnAgo && lastMove.Distance == 2;
                }
                return false;
            }
        }

        public List<Move> MovesList { get => movesList; set => movesList = value; }

        public delegate void OnMoveCallback();
        private OnMoveCallback onMoveCallback;
        public void SetOnMoveCallback(OnMoveCallback onMoveCb) => onMoveCallback = onMoveCb;

        public Piece(PieceType pieceType, Team pieceTeam, ChessGame game)
        {
            type = pieceType;
            TeamColor = pieceTeam;
            PieceMovement = Movement.GetMovement(type);
            Game = game;
        }

        // PostMove updates and calculations
        public void PostMove()
        {
            onMoveCallback?.Invoke();
        }

        public List<Square> GetValidSquares(bool simulate)
        {
            if (CurrentSquare == null)
            {
                return new();
            }
            var moves = PieceMovement.GetValidSquares(Game.board, CurrentSquare.Coordinates);
            if (simulate)
            {

                List<Square> slimMoves = new();
                foreach (var move in moves)
                {
                    Game.SimulateMove(this, move.Coordinates, out bool InCheck);
                    if (!InCheck)
                    {
                        slimMoves.Add(move);
                    }
                }

                // Castle Checks
                slimMoves.AddRange(GetCastleMoves());

                return slimMoves;
            }
            else
            {
                return moves;
            }
        }

        private List<Square> GetCastleMoves()
        {
            List<Square> castleMoves = new();
            if (!Game.IsTeamInCheck(TeamColor) && type == PieceType.King && MoveCount == 0)
            {
                foreach (Piece rook in Game.GetRooks(TeamColor))
                {
                    if (rook.MoveCount == 0)
                    {
                        // Get the rook direction
                        Vector2I rookDirection = rook.CurrentSquare.Coordinates - CurrentSquare.Coordinates;
                        rookDirection.Y = Math.Sign(rookDirection.Y);

                        // check each space on the way to the rook
                        bool allowed = true;
                        for (int i = 1; i <= 2 && allowed; i++)
                        {
                            Square interSquare = Game.board.GetSquare(CurrentSquare.Coordinates - i * rookDirection);
                            if (interSquare.state == SquareState.Occupied)
                            {
                                allowed = false;
                            }
                            if (allowed)
                            {
                                Game.SimulateMove(this, interSquare.Coordinates, out bool InCheck);
                                if (InCheck)
                                {
                                    allowed = false;
                                }
                            }
                        }
                        if (allowed)
                        {
                            castleMoves.Add(Game.board.GetSquare(CurrentSquare.Coordinates - 2 * rookDirection));
                        }
                    }
                }
            }
            return castleMoves;
        }

        public override string ToString()
        {
            return $"{TeamColor} {type} @ {CurrentSquare}";
        }

        public void AddMove(Move move)
        {
            movesList.Add(move);
        }

        public Move LastMove()
        {
            if (movesList.Count > 0)
            {
                return movesList[^1];
            }
            return null;
        }

        public bool WasLastMovePawnAttack()
        {
            // Should try to reuse code from the Movement class if possible
            if (type == PieceType.Pawn)
            {
                Move lastMove = LastMove();
                if (lastMove != null)
                {
                    // This direction stuff is getting reused, should probably abstract it away a bit.
                    Vector2I teamDirection = new(TeamColor == Team.Black ? -1 : 1, 1);
                    PawnMovement movement = (PawnMovement)PieceMovement;
                    return movement.attackMoves.Contains(lastMove.Direction * teamDirection);
                }

            }
            return false;
        }

        public bool IsPromotablePawn()
        {
            int backRow = TeamColor == Team.White ? 7 : 0;
            return type == PieceType.Pawn && CurrentSquare.Coordinates.X == backRow;
        }
    }


}



using Godot;

using System;
using System.Collections.Generic;

namespace Chess
{
    public class ChessGame
    {
        // static
        private static readonly List<Team> Teams = new() { Team.White, Team.Black };

        // Settings
        private readonly int boardSize = 8;

        // State
        public Board board;
        public Dictionary<Team, List<Piece>> pieces = new();
        private Check checkState = new();

        public int Turn { get; private set; } = 1;
        public Team TeamTurn { get; private set; } = Team.White;
        public Team NonTeamTurn { get; private set; } = Team.Black;
        public bool CheckMate { get { return checkState.isMate; } }
        public string LastMove = "";

        // Callbacks
        public delegate void NextTurnCallback();
        private NextTurnCallback nextTurnCallback;
        public void SetNextTurnCallBack(NextTurnCallback cb) => nextTurnCallback = cb;

        public delegate void PromotionCallBack(Piece piece);
        private PromotionCallBack promotionCallBack;
        public void SetPromotionCallback(PromotionCallBack pcb) => promotionCallBack = pcb;

        public ChessGame() => board = new Board(boardSize);

        private int IndexByTeam(Team team, int i) => team == Team.White ? i : boardSize - i - 1;

        private static Team GetOppositeTeam(Team team) => team == Team.Black ? Team.White : Team.Black;

        public void SetupStalemateBoard()
        {
            pieces = new()
            {
                { Team.White, new() },
                { Team.Black, new() }
            };

            // King and Queen
            AddPieceToBoard(PieceType.Queen, Team.Black, new(1, 7));
            AddPieceToBoard(PieceType.Queen, Team.Black, new(7, 1));
            AddPieceToBoard(PieceType.King, Team.Black, new(7, 7));
            AddPieceToBoard(PieceType.King, Team.White, new(1, 1));

        }
        public void SetupBoard()
        {
            pieces = new() {
                { Team.White, new()},
                { Team.Black, new()}
            };
            foreach (Team team in ChessGame.Teams)
            {
                int startRow = IndexByTeam(team, 0);
                // Pawns
                int pawnRow = IndexByTeam(team, 1);
                for (int i = 0; i < boardSize; i++)
                {

                    AddPieceToBoard(PieceType.Pawn, team, new(pawnRow, i));
                }

                // Rooks
                AddPieceToBoard(PieceType.Rook, team, new(startRow, IndexByTeam(team, 0)));
                AddPieceToBoard(PieceType.Rook, team, new(startRow, IndexByTeam(team, 7)));

                // Knights
                AddPieceToBoard(PieceType.Knight, team, new(startRow, IndexByTeam(team, 1)));
                AddPieceToBoard(PieceType.Knight, team, new(startRow, IndexByTeam(team, 6)));

                // Bishops
                AddPieceToBoard(PieceType.Bishop, team, new(startRow, IndexByTeam(team, 2)));
                AddPieceToBoard(PieceType.Bishop, team, new(startRow, IndexByTeam(team, 5)));

                // King and Queen
                AddPieceToBoard(PieceType.Queen, team, new(startRow, 3));
                AddPieceToBoard(PieceType.King, team, new(startRow, 4));
            }
        }

        private Square GetSquareByPosition(Vector2I position)
        {
            return board.GetSquare(position);
        }

        private void AddPieceToBoard(PieceType pieceType, Team team, Vector2I position)
        {
            Piece piece = new(pieceType, team, this);
            Square square = GetSquareByPosition(position);
            square.RemovePiece();
            square.AddPiece(piece);

            pieces.TryGetValue(team, out List<Piece> teamPieces);
            teamPieces.Add(piece);
        }

        public void MovePiece(Piece piece, Vector2I targetPosition)
        {
            LastMove = $"Moved {piece} to {Square.LabelFromPosition(targetPosition)}";

            Square oldSquare = piece.CurrentSquare;
            Square targetSquare = GetSquareByPosition(targetPosition);

            piece.AddMove(new Move(oldSquare.Coordinates, targetPosition, Turn));
            // Was that an en passant !?!?
            if (piece.WasLastMovePawnAttack())
            {
                DoEnPassant(piece);
            }

            oldSquare.RemovePiece();
            targetSquare.RemovePiece();
            targetSquare.AddPiece(piece);



            // Was this a castle move?
            if (piece.type == PieceType.King && Utils.Distance(oldSquare.Coordinates, targetSquare.Coordinates) == 2)
            {
                DoCastle(piece.TeamColor);
            }

            piece.PostMove();

            // Is there a promotable pawn?
            if (piece.IsPromotablePawn())
            {
                promotionCallBack(piece);
            }

            NextTurn();
        }

        public void NextTurn()
        {

            Turn++;
            TeamTurn = Turn % 2 == 0 ? Team.Black : Team.White;
            NonTeamTurn = TeamTurn == Team.White ? Team.Black : Team.White;

            var check = new Check
            {
                teamInCheck = TeamTurn,
                inCheck = CalculateCheckAll(NonTeamTurn),
                isMate = IsThatItMate(TeamTurn)
            };

            SetCheck(check);


            nextTurnCallback?.Invoke();

        }

        public void SetCheck(Check check)
        {
            checkState = check;
        }

        public Piece GetKing(Team team)
        {
            pieces.TryGetValue(team, out List<Piece> teamPieces);
            foreach (Piece piece in teamPieces)
            {
                if (piece.IsKing)
                {
                    return piece;
                }
            }
            throw new KeyNotFoundException("Can't find the King!");
        }

        public List<Piece> GetRooks(Team team)
        {
            List<Piece> rooks = new();
            pieces.TryGetValue(team, out List<Piece> teamPieces);
            foreach (Piece piece in teamPieces)
            {
                if (piece.type == PieceType.Rook)
                {
                    rooks.Add(piece);
                }
            }
            return rooks;
        }

        public void DoEnPassant(Piece passingPawn)
        {
            Move lastMove = passingPawn.LastMove();
            int direction = passingPawn.TeamColor == Team.Black ? -1 : 1;
            Vector2I attackPosition = lastMove.To - new Vector2I(direction, 0);
            var attackSquare = GetSquareByPosition(attackPosition);
            var attackPiece = attackSquare.occupant;
            if (attackPiece != null && attackPiece.EnPassantable)
            {
                attackSquare.RemovePiece();
                attackPiece.PostMove();
            }
        }

        public void DoCastle(Team team)
        {
            // Assumes the King has already been moved its two places
            var rooks = GetRooks(team);
            var king = GetKing(team);

            // Once the king's two step move is done we need to re-establish which is the correct rook in context
            float distance = float.MaxValue;
            Piece closestRook = null;

            foreach (Piece rook in rooks)
            {
                if (rook.CurrentSquare != null)
                {
                    float toKing = Utils.Distance(rook.CurrentSquare.Coordinates, king.CurrentSquare.Coordinates);
                    if (toKing < distance)
                    {
                        closestRook = rook;
                        distance = toKing;
                    }
                }
            }

            if (closestRook != null)
            {
                Vector2I rookDirection = closestRook.CurrentSquare.Coordinates - king.CurrentSquare.Coordinates;
                rookDirection.Y = Math.Sign(rookDirection.Y);  // normalize of sorts

                var targetSquare = GetSquareByPosition(king.CurrentSquare.Coordinates - rookDirection);
                closestRook.CurrentSquare.RemovePiece();
                targetSquare.AddPiece(closestRook);
                closestRook.PostMove();
            }
        }

        public bool IsTeamInCheck(Team team)
        {
            return checkState.inCheck && checkState.teamInCheck == team;
        }

        public bool CalculateCheckAll(Team team)
        {
            // Can pieces of a given team reach the opposing King
            HashSet<Square> coverage = new();
            pieces.TryGetValue(team, out var teamPieces);

            // Add all the covered squares to set
            foreach (var piece in teamPieces)
            {
                coverage.UnionWith(piece.GetValidSquares(simulate: false));
            }

            // Get the opposing King
            var king = GetKing(GetOppositeTeam(team));

            bool isInCheck = coverage.Contains(king.CurrentSquare);
            return isInCheck;
        }

        public void SimulateMove(Piece piece, Vector2I targetPosition, out bool stillInCheck)
        {
            /* Simulating a move */
            // Update the board state
            // Do Check Calc
            // Rewind Board State */

            // Debug.Log($"Simulating {piece} Moving to {Square.LabelFromPosition(targetPosition)}");

            // Update the board state
            var oldSquare = piece.CurrentSquare;
            var targetSquare = GetSquareByPosition(targetPosition);
            var pieceAtTarget = targetSquare.occupant;

            oldSquare.RemovePiece();
            targetSquare.AddPiece(piece);

            // Check if move leaves this team in Check            
            stillInCheck = CalculateCheckAll(GetOppositeTeam(piece.TeamColor));

            // Rewind the board state
            targetSquare.RemovePiece();
            oldSquare.AddPiece(piece);
            if (pieceAtTarget != null)
            {
                targetSquare.AddPiece(pieceAtTarget);
            }

        }

        public bool IsThatItMate(Team team)
        {
            // Simulate every possible move for a given turn
            // If king is in check and any result in a non-check scenario, then it is not check mate
            // If king is not in check and any result in a non-check scenario, then it is not stale mate            

            // Team X is in check, we want to find out if there is any way out. 
            // Get All the pieces for Team X
            pieces.TryGetValue(team, out var teamPieces);

            // For each piece 
            foreach (var piece in teamPieces)
            {
                // Don't bother with pieces which have been taken
                if (piece.CurrentSquare == null)
                {
                    continue;
                }
                // get all the moves
                var allowedSquares = piece.GetValidSquares(simulate: false);

                // simulate each move, and see if it still results in check
                foreach (var move in allowedSquares)
                {
                    SimulateMove(piece, move.Coordinates, out bool stillInCheck);
                    if (!stillInCheck)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public string GameInfo()
        {
            string checkMessage = "";
            if (checkState.inCheck && checkState.isMate)
            {
                checkMessage = $"\nThat's Checkmate! {GetOppositeTeam(checkState.teamInCheck)} Wins!";
            }
            else if (checkState.isMate)
            {
                checkMessage = $"\nOof! That's a stalemate! Nobody Wins!";
            }
            else if (checkState.inCheck)
            {
                checkMessage = $"{checkState.teamInCheck} is in check!";
            }

            return
                $"Last Move: \n - {LastMove}\n\n" +
                $"Turn: {Turn}\n" +
                $"Team: {TeamTurn}\n" +
                $"{checkMessage}";
        }

        public Piece PromotePawn(Piece pawn, PieceType promotion)
        {
            GD.Print($"Promoting to {promotion}");
            var square = pawn.CurrentSquare;
            var position = pawn.CurrentSquare.Coordinates;
            var moveHistory = pawn.MovesList;
            AddPieceToBoard(promotion, pawn.TeamColor, position);
            square.occupant.MovesList = moveHistory;

            pieces.TryGetValue(pawn.TeamColor, out var teamPieces);
            teamPieces.Remove(pawn);
            pawn.PostMove();

            return square.occupant;
        }

        public struct Check
        {
            public bool inCheck;
            public bool isMate;
            public Team teamInCheck;
        }
    }

    public enum SquareState
    {
        Empty, Occupied
    }

    public enum Team
    {
        Black, White
    }

    public enum PieceType
    {
        King, Queen, Bishop, Rook, Knight, Pawn
    }


}



using Godot;

using System;
using System.Collections.Generic;

using Rules;

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
        private Team teamInCheck;
        public int Turn { get; private set; } = 1;
        public Team TeamTurn { get; private set; } = Team.White;
        public Team NonTeamTurn { get; private set; } = Team.Black;

        public bool checkMate = false;


        public ChessGame()
        {
            board = new Board(boardSize);
        }

        private int IndexByTeam(Team team, int i)
        {
            return team == Team.White ? i : boardSize - i - 1;
        }

        public void SetupBoard()
        {
            pieces = new() {
                { Team.White, new()},
                { Team.Black, new()}
            };
            foreach (Team team in Teams)
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
            Piece piece = new(pieceType, team, board);
            Square square = GetSquareByPosition(position);
            square.AddPiece(piece);

            pieces.TryGetValue(team, out List<Piece> teamPieces);
            teamPieces.Add(piece);
        }

        public void MovePiece(Piece piece, Vector2I targetPosition)
        {
            GD.Print($"Moved {piece} to {Square.LabelFromPosition(targetPosition)}");
            Square oldSquare = piece.currentSquare;
            oldSquare.RemovePiece();

            Square targetSquare = GetSquareByPosition(targetPosition);
            targetSquare.RemovePiece();
            targetSquare.AddPiece(piece);

            piece.PostMove();

            NextTurn();
        }

        public void NextTurn()
        {

            Turn++;
            TeamTurn = Turn % 2 == 0 ? Team.Black : Team.White;
            NonTeamTurn = TeamTurn == Team.White ? Team.Black : Team.White;

            if (CalculateCheckAll(NonTeamTurn))
            {
                GD.Print($"{TeamTurn}'s King is in check");
                if (IsThatCheckMate(TeamTurn))
                {
                    GD.Print($"That's Checkmate! {NonTeamTurn} Wins!");
                    checkMate = true;
                }
            }

        }

        public void SetTeamInCheck(Team team)
        {
            teamInCheck = team;
            GD.Print($"{teamInCheck}'s King is in check");
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

        public bool CalculateCheck(Team team)
        {
            bool isInCheck = false;
            // Can black pieces reach the white king
            pieces.TryGetValue(team, out var teamPieces);
            foreach (var piece in teamPieces)
            {
                if (piece.CanRegicide)
                {
                    isInCheck = true;
                }
            }
            return isInCheck;
        }

        private static Team GetOppositeTeam(Team team)
        {
            return team == Team.Black ? Team.White : Team.Black;
        }

        public bool CalculateCheckAll(Team team)
        {
            // Can pieces of a given team reach the opposing King
            HashSet<Square> coverage = new();
            pieces.TryGetValue(team, out var teamPieces);

            // Add all the covered squares to set
            foreach (var piece in teamPieces)
            {
                coverage.UnionWith(piece.GetValidSquares());
            }

            // Get the opposing King
            var king = GetKing(GetOppositeTeam(team));

            bool isInCheck = coverage.Contains(king.currentSquare);
            return isInCheck;
        }

        /* Determining CheckMate */
        // Simulate every possible move for a given turn
        // If any result in a non-check scenario, then it is not check mate
        // Only need to do this if the king has been determined to be in check??

        /* Simulating a move */
        // Update the board state
        // Do Check Calc
        // Rewind Board State

        public void SimulateMoveUnderCheck(Piece piece, Vector2I targetPosition, out bool stillInCheck)
        {
            // GD.Print($"Simulating {piece} Moving to {Square.LabelFromPosition(targetPosition)}");

            // Update the board state
            var oldSquare = piece.currentSquare;
            var targetSquare = GetSquareByPosition(targetPosition);
            var pieceAtTarget = targetSquare.occupant;

            oldSquare.RemovePiece();
            targetSquare.AddPiece(piece);

            // Check if move leaves this team in Check            
            stillInCheck = CalculateCheckAll(GetOppositeTeam(piece.team));

            // Rewind the board state
            targetSquare.RemovePiece();
            oldSquare.AddPiece(piece);
            if (pieceAtTarget != null)
            {
                targetSquare.AddPiece(pieceAtTarget);
            }

        }

        public bool IsThatCheckMate(Team team)
        {
            // Team X is in check, we want to find out if there is any way out. 
            // Get All the pieces for Team X
            pieces.TryGetValue(team, out var teamPieces);

            // For each piece 
            foreach (var piece in teamPieces)
            {
                // Don't bother with pieces which have been taken
                if (piece.currentSquare == null)
                {
                    continue;
                }
                // get all the moves
                var allowedSquares = piece.GetValidSquares();

                // simulate each move, and see if it still results in check
                foreach (var move in allowedSquares)
                {
                    SimulateMoveUnderCheck(piece, move.position, out bool stillInCheck);
                    if (!stillInCheck)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }

    public enum SquareState
    {
        Empty, Occupied
    }

    public class Square
    {
        public Vector2I position;
        public SquareState state;
        public Piece occupant;

        public string Label { get { return LabelFromPosition(position); } }

        public static string LabelFromPosition(Vector2I pos)
        {           
            return $"{(char)('A' + pos.Y)}{pos.X + 1}";
        }

        public Square(int x, int y)
        {
            position = new Vector2I(x, y);
        }

        public void AddPiece(Piece piece)
        {
            occupant = piece;
            state = SquareState.Occupied;
            piece.currentSquare = this;
        }

        public void RemovePiece()
        {
            state = SquareState.Empty;
            if (occupant != null)
            {
                occupant.currentSquare = null;
                occupant = null;
            }
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public class Board
    {
        readonly List<List<Square>> internalBoard;
        public int size = 0;

        public Board(int boardSize)
        {
            internalBoard = new();
            for (int x = 0; x < boardSize; x++)
            {
                List<Square> row = new();
                for (int y = 0; y < boardSize; y++)
                {
                    row.Add(new Square(x, y));
                    size += 1;
                }
                internalBoard.Add(row);
            }
        }

        public Square GetSquare(Vector2I position)
        {
            return internalBoard[position.X][position.Y];
        }

        public Square GetSquare(int x, int y)
        {
            return internalBoard[x][y];
        }

        public List<Square> GetSquares()
        {
            List<Square> squares = new();
            foreach (List<Square> row in internalBoard)
            {
                foreach (Square square in row)
                {
                    squares.Add(square);
                }
            }
            return squares;
        }
    }

    public enum PieceType
    {
        King, Queen, Bishop, Rook, Knight, Pawn
    }


    public class Piece
    {
        // Inputs
        public PieceType type;
        public Team team;
        public Movement PieceMovement { get; private set; }

        public Board ChessBoard;

        // State
        public Square currentSquare;
        public int MoveCount { get; private set; }
        public List<Square> allowedSquares;
        public bool CanRegicide { get; private set; } = false;

        // Property
        public bool IsKing { get { return type == PieceType.King; } }


        public Piece(PieceType pieceType, Team pieceTeam, Board board)
        {
            type = pieceType;
            team = pieceTeam;
            PieceMovement = Movement.GetMovement(type);
            ChessBoard = board;
        }


        // PostMove updates and calculations
        public void PostMove()
        {
            allowedSquares = GetValidSquares();
            MoveCount++;
            CanRegicide = PieceCanRegicide();
        }

        public List<Square> GetValidSquares()
        {
            if (currentSquare == null)
            {
                return new();
            }
            try
            {
                return PieceMovement.GetValidSquares(ChessBoard, currentSquare.position);
            }
            catch (NullReferenceException e)
            {
                GD.Print($"What's going on with {this} ... {currentSquare}?");
                throw e;
            }

        }


        // i.e. can this piece attack the opponent's King
        public bool PieceCanRegicide()
        {
            foreach (Square square in allowedSquares)
            {
                if (square.occupant != null && square.occupant.IsKing)
                {
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"{team} {type} @ {currentSquare}";
        }
    }

    public enum Team
    {
        Black, White
    }

}



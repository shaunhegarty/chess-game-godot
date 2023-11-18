using Godot;

public partial class GameManager : Node
{
    public GameBoard Board;
    public ChessManager ChessManager;

    public void RegisterGameBoard(GameBoard board)
    {
        Board = board;
    }

    public void RegisterChessManager(ChessManager chessManager)
    {
        ChessManager = chessManager;
    }
}

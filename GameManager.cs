using Godot;

public partial class GameManager : Node
{
    public GameBoard Board;
    public ChessManager ChessManager;
    public Dragger Dragger;
    public GamePiece CurrentPiece;
    public ChessUI ChessUI;

    public void RegisterGameBoard(GameBoard board)
    {
        Board = board;
    }

    public void RegisterChessManager(ChessManager chessManager)
    {
        ChessManager = chessManager;
    }

    public void RegisterDragger(Dragger dragger)
    {
        Dragger = dragger;
    }

    public void RegisterChessUI(ChessUI chessUI)
    {
        ChessUI = chessUI;
    }

    public void SetSelectedPiece(GamePiece piece)
    {
        CurrentPiece = piece;
    }
}

using Chess;
using Godot;
using System.Collections.Generic;

[Tool]
public partial class GameBoard : Node3D
{
    private PackedScene BoardSquareScene = ResourceLoader.Load<PackedScene>("res://Scenes/BoardSquare/Square.tscn");
    private int _boardEdgeCount = 8;

    [Export] private int BoardSquareLength = 1;

    [Export] private int BoardEdgeCount
    {
        get { return _boardEdgeCount; }
        set {
            _boardEdgeCount = value;
            if (IsNodeReady())
            {
                BuildBoard();
            }            
        }
    }

    public List<BoardSquare> Squares;
    public Dictionary<Vector2I, BoardSquare> SquaresMap;

    public override void _Ready()
    {
        GameManager manager = Utils.GetManager(this);
        manager.RegisterGameBoard(this);
        BuildBoard();
        manager.ChessManager.SetupPieces();

    }

    private void ClearBoard()
    {
        foreach (Node child in GetChildren()) {
            if (child is BoardSquare) {
                child.QueueFree();
            }
        }
        Squares = new();
        SquaresMap = new();
    }

    private void BuildBoard()
    {
        
        ClearBoard();

        for (int j = 0; j < BoardEdgeCount; j++)
        {
            for (int i = 0; i < BoardEdgeCount; i++)
            {
                Vector2I index = new(j, i);
                AddSquare(index);
                
            }
        }
    }

    private BoardSquare AddSquare(Vector2I index)
    {
        BoardSquare boardSquare = BoardSquareScene.Instantiate<BoardSquare>();
        AddChild(boardSquare);
        boardSquare.SquareSize = BoardSquareLength;
        var i = index.X;
        var j = index.Y;

        Vector3 position = new(x: i * BoardSquareLength, y: 0, z: j * BoardSquareLength);
        boardSquare.Coordinates = index;
        boardSquare.BasePosition = position;
        boardSquare.TeamColor = (i + j) % 2 == 0 ? Team.Black : Team.White;
        boardSquare.Name = boardSquare.CoordinateString();

        Squares.Add(boardSquare);
        SquaresMap.Add(index, boardSquare);

        return boardSquare;
    }

    public BoardSquare GetSquare(int x, int y) {
        return SquaresMap[new(x, y)];
    }

    public BoardSquare GetSquare(Vector2I coordinates)
    {
        return SquaresMap[coordinates];
    }

    public List<BoardSquare> SquaresToBoardSquares(List<Square> squares)
    {
        List<BoardSquare> boardSquares = new();
        foreach (Square square in squares)
        {
            boardSquares.Add(GetSquare(square.Coordinates));
        }
        return boardSquares;
    }

}

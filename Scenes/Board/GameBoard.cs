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

    private List<BoardSquare> Squares;

    public override void _Ready()
    {
        BuildBoard();
    }

    private void ClearBoard()
    {
        foreach (Node child in GetChildren()) {
            if (child is BoardSquare) {
                child.QueueFree();
            }
        }
    }

    private void BuildBoard()
    {
        Squares = new();
        ClearBoard();

        for (int j = 0; j < BoardEdgeCount; j++)
        {
            for (int i = 0; i < BoardEdgeCount; i++)
            {
                Vector2I index = new(j, i);
                BoardSquare boardSquare = CreateSquare(index);
                Squares.Add(boardSquare);
            }
        }
    }

    private BoardSquare CreateSquare(Vector2I index)
    {
        BoardSquare boardSquare = BoardSquareScene.Instantiate<BoardSquare>();
        AddChild(boardSquare);
        boardSquare.SquareSize = BoardSquareLength;
        var i = index.X;
        var j = index.Y;

        Vector3 position = new(x: i * BoardSquareLength, y: 0, z: j * BoardSquareLength);
        boardSquare.TeamColor = (i + j) % 2 == 0 ? Team.Black : Team.White;
        boardSquare.Index = index;
        boardSquare.BasePosition = position;

        return boardSquare;
    }

}

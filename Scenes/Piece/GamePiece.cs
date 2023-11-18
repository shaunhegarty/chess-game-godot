using Chess;
using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class GamePiece : Node3D
{
    private bool _setupComplete = false;

    // Children
	private MeshInstance3D _mesh;
	private Label3D _label;

    // Settings
    public Vector3 PositionOffset = new(0, 1f, 0);
    private Team _teamColor = Team.White;

    // Type
    public Piece ChessPiece;

    // State
    public BoardSquare CurrentSquare;
    private BoardSquare HighlightedSquare;
    private List<BoardSquare> AllowedSquares;

    [Export]
    public Team TeamColor
    {
        get { return _teamColor; }
        set
        {
            SetTeam(value);
        }
    }

    [Export] private Material WhiteMaterial;
	[Export] private Material BlackMaterial;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mesh = GetNode<MeshInstance3D>("%PieceMesh");
		_label = GetNode<Label3D>("%PieceLabel");
        _setupComplete = true;
	}

    private void SetTeam(Team team)
    {
        _teamColor = team;
        if (_setupComplete)
        {
            SetMaterialFromTeam();
            //_label.Text = $"{TeamColor}\n{CoordinateString()}";
        }
    }

    private void SetMaterialFromTeam()
    {
        Material material = TeamColor == Team.White ? WhiteMaterial : BlackMaterial;
        _mesh.SetSurfaceOverrideMaterial(0, material);
    }

    public void UpdatePosition()
    {
        if (ChessPiece.currentSquare != null)
        {
            int rowIndex = ChessPiece.currentSquare.position.X;
            int colIndex = ChessPiece.currentSquare.position.Y;
            //BoardSquare square = MainManager.Instance.ChessManager.Board.AllSquares[rowIndex][colIndex];
            //SetPositionToTargetSquare(square);
        }
        else
        {
            //PieceAttacked();
        }

    }

    public void SetSquare(BoardSquare square)
    {
        CurrentSquare = square;
    }

    public void SetPiece(Chess.Piece piece)
    {
        ChessPiece = piece;
        piece.SetOnMoveCallback(UpdatePosition);
    }

}

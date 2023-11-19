using Chess;
using Godot;
using System.Collections.Generic;

[Tool]
public partial class GamePiece : Node3D
{
    private bool _setupComplete = false;
    private GameManager Manager;

    // Children
	private MeshInstance3D _mesh;
	private Label3D _label;
    private DragAndDroppable _dragAndDroppable;

    // Settings
    public Vector3 PositionOffset = new(0, 0, 0);
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

    private ChessGame Game => Manager.ChessManager.Game;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mesh = GetNode<MeshInstance3D>("%PieceMesh");
		_label = GetNode<Label3D>("%PieceLabel");
        _dragAndDroppable = GetNode<DragAndDroppable>("%DragAndDroppable");
        _dragAndDroppable.Selected += OnPieceSelected;
        _dragAndDroppable.Dropped += OnPiecePlaced;
        Manager = Utils.GetManager(this);

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
        _mesh.SetSurfaceOverrideMaterial(0, Utils.TeamMaterial(TeamColor));
    }

    public void UpdatePosition()
    {
        if (ChessPiece.CurrentSquare != null)
        {
            GameManager manager = Utils.GetManager(this);
            BoardSquare boardSquare = manager.Board.GetSquare(ChessPiece.CurrentSquare.Coordinates);
            SetPositionToTargetSquare(boardSquare);
        }
        else
        {
            PieceAttacked();
        }
    }


    public void SetSquare(BoardSquare square)
    {
        CurrentSquare = square;
    }

    public void SetPiece(Chess.Piece piece)
    {
        ChessPiece = piece;
        _label.Text = $"{piece.type}";
        piece.SetOnMoveCallback(UpdatePosition);
    }

    public void SetPositionToTargetSquare(BoardSquare square)
    {
        CurrentSquare?.SetOccupant(null);
        SetSquare(square);
        square.Occupant?.PieceAttacked();
        square.SetOccupant(this);
        SetPositionToTargetSquare();
    }

    void SetPositionToTargetSquare()
    {
        Position = CurrentSquare.Position;                
    }

    public void PieceAttacked()
    {
        Visible = false;
    }

    private bool IsMyTurn => TeamColor == Manager.ChessManager.Game.TeamTurn && !Manager.ChessManager.Game.CheckMate;

    private List<BoardSquare> GetValidSquares()
    {
        var squares = ChessPiece.GetValidSquares(simulate: true);
        AllowedSquares = Utils.GetManager(this).Board.SquaresToBoardSquares(squares);
        return AllowedSquares;
    }

    private void OnPieceSelected()
    {
        if (IsMyTurn)
        {
            Manager.CurrentPiece = this;
            GetValidSquares();
            foreach (BoardSquare square in AllowedSquares)   {
                square.SetValid(true);
            }
        }
    }

    private void OnPiecePlaced(DropReceivable targetArea)
    {
        Manager.CurrentPiece = null;

        if (AllowedSquares != null)
        {
            foreach (BoardSquare square in AllowedSquares)
            {
                square.SetValid(false);
            }
        }

        if (IsMyTurn)
        {
            //HighlightCandidateSquares(false);
            GetValidSquares();

            var boardSquare = targetArea.GetParent<BoardSquare>();
            if (boardSquare != null && AllowedSquares.Contains(boardSquare))
            {
                HighlightedSquare = boardSquare;
                SetPositionToTargetSquare(HighlightedSquare);
                Game.MovePiece(ChessPiece, HighlightedSquare.Coordinates);
                return;

            } 
        }
        
        
        SetPositionToTargetSquare();
        
    }

}

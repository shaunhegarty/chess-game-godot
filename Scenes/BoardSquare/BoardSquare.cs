using Chess;
using Godot;

[Tool]
public partial class BoardSquare : Node3D
{
	// Settings
	private Team _teamColor = Team.White;
	private float _squareSize = 1;

	// Children
	private Label3D _label;
    private MeshInstance3D _mesh;

	// State
	private Vector3 _basePosition;
	public Vector2I Coordinates;
	public GamePiece Occupant;
	private bool _setupComplete = false;


	public Vector3 BasePosition
	{
		get { return _basePosition; }
		set { 
			_basePosition = value;
			Position = value;
		}
	}


    [Export] public Team TeamColor
    {
		get { return _teamColor; }
		set
		{
			SetTeam(value);
		}
	}

	[Export] public float SquareSize
	{
		get { return _squareSize; }
		set
		{
			if (_setupComplete)
			{
				_squareSize = value;
				var boxMesh = (BoxMesh)_mesh.Mesh;
				boxMesh.Size = new Vector3(_squareSize, 0.1f, _squareSize);
			}
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mesh = GetNode<MeshInstance3D>("SquareMesh");
		_label = GetNode<Label3D>("%Colour");
		_setupComplete = true;
	}

	private void SetTeam(Team team)
	{
		_teamColor = team;
		if (_setupComplete)
		{
			SetMaterialFromTeam();
			_label.Text = $"{CoordinateString()}";
		}
	}

	private void SetMaterialFromTeam()
	{
        _mesh.SetSurfaceOverrideMaterial(0, Utils.TeamMaterial(TeamColor));
    }

    public void SetOccupant(GamePiece piece)
    {
        Occupant = piece;
    }


    public string CoordinateString()
	{
        char letter = (char)('A' + Coordinates.X);
        return $"{letter}{Coordinates.Y + 1}";
    }
}
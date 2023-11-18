using Chess;
using Godot;

[Tool]
public partial class BoardSquare : Node3D
{
	private Team _teamColor = Team.White;
	private float _squareSize = 1;
	private Label3D _label;
    private MeshInstance3D _mesh;
	private Vector3 _basePosition;
	public Vector2I Index;

	private Material WhiteMaterial = ResourceLoader.Load<Material>("res://Materials/square_white.tres");
    private Material BlackMaterial = ResourceLoader.Load<Material>("res://Materials/square_black.tres");

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
			if (IsNodeReady())
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

	private void SetMaterial(Material material)
	{
		_mesh.SetSurfaceOverrideMaterial(0, material);
	}

	private void SetTeam(Team team)
	{
		_teamColor = team;
		if (_setupComplete)
		{
			SetMaterialFromTeam();
			_label.Text = $"{TeamColor}";
		}
	}

	private void SetMaterialFromTeam()
	{
        Material material = TeamColor == Team.White ? WhiteMaterial : BlackMaterial;
		SetMaterial(material);
    }
}
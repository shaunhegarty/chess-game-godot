using Chess;
using Godot;

[Tool]
public partial class BoardSquare : Node3D
{
	private Team _teamColor = Team.White;
	private float _squareSize = 1;
    private MeshInstance3D _mesh;
	private Vector3 _basePosition;
	public Vector2I Index;

	public Vector3 BasePosition
	{
		get { return _basePosition; }
		set { 
			_basePosition = value;
			Position = value;
		}
	}


	private Material WhiteMaterial = (Material)ResourceLoader.Load("res://Materials/square_white.tres");
    private Material BlackMaterial = (Material)ResourceLoader.Load("res://Materials/square_black.tres");

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
	}

	private void SetMaterial(Material material)
	{
		_mesh.SetSurfaceOverrideMaterial(0, material);
	}

	private void SetTeam(Team team)
	{
		_teamColor = team;
		if (IsNodeReady())
		{
			SetMaterialFromTeam();
		}
	}

	private void SetMaterialFromTeam()
	{
		GD.Print(TeamColor);
        Material material = TeamColor == Team.White ? WhiteMaterial : BlackMaterial;
		SetMaterial(material);
    }
}

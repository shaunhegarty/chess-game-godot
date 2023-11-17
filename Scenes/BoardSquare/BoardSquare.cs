using Chess;
using Godot;

[Tool]
public partial class BoardSquare : Node3D
{
	private Team _teamColor = Team.White;

	[Export]
	private Team TeamColor
    {
		get { return _teamColor; }
		set
		{
			SetTeam(value);
		}
	}
	[Export] public Material WhiteMaterial;
	[Export] public Material BlackMaterial;
	private MeshInstance3D _mesh;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_mesh = GetNode<MeshInstance3D>("SquareMesh");
	}

	public void SetMaterial(Material material)
	{
		_mesh.SetSurfaceOverrideMaterial(0, material);
	}

	public void SetTeam(Team team)
	{
		_teamColor = team;
		if (IsNodeReady())
		{
			Material material = team == Team.White ? WhiteMaterial : BlackMaterial;
			SetMaterial(material);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

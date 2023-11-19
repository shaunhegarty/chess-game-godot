using Godot;

public partial class ChessUI : CanvasLayer
{

	private Label _gameInfo;
	private GameManager Manager;

	public override void _Ready()
	{
		Manager = Utils.GetManager(this);
		_gameInfo = GetNode<Label>("%GameInfo");

	}

	public override void _Process(double delta)
	{
		_gameInfo.Text = Manager.ChessManager.Game.GameInfo();
	}
}

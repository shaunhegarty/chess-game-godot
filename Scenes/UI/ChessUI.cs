using Godot;

public partial class ChessUI : CanvasLayer
{

	private Label _gameInfo;
	private Button _restartButton;
	private GameManager Manager;

	public override void _Ready()
	{
		Manager = Utils.GetManager(this);
		_gameInfo = GetNode<Label>("%GameInfo");
		_restartButton = GetNode<Button>("%RestartButton");
		_restartButton.Pressed += Restart;

	}

	public override void _Process(double delta)
	{
		_gameInfo.Text = Manager.ChessManager.Game.GameInfo();
	}

	private void Restart()
	{
		GetTree().ChangeSceneToFile("res://main.tscn");
	}
}

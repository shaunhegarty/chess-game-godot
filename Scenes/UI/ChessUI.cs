using Chess;
using Godot;

public partial class ChessUI : CanvasLayer
{

	private Label _gameInfo;
	private Button _restartButton;
	private GameManager Manager;
	public PromotionUI PromotionUi;

	public override void _Ready()
	{
		Manager = Utils.GetManager(this);
		Manager.RegisterChessUI(this);

		_gameInfo = GetNode<Label>("%GameInfo");
		_restartButton = GetNode<Button>("%RestartButton");
		PromotionUi = GetNode<PromotionUI>("%PromotionContainer");

		_restartButton.Pressed += Restart;

        Manager.ChessManager.Game.SetPromotionCallback(Manager.ChessUI.SetPawnForPromotion);

    }

	public override void _Process(double delta)
	{
		_gameInfo.Text = Manager.ChessManager.Game.GameInfo();
	}

	private void Restart()
	{
		GetTree().ChangeSceneToFile("res://main.tscn");
	}

	public void SetPawnForPromotion(Piece piece)
	{
		GD.Print($"Is it a pawn? {piece.type == PieceType.Pawn} || Is it promotable: {piece.IsPromotablePawn()}");
		if (piece.type == PieceType.Pawn && piece.IsPromotablePawn()) {
			PromotionUi.PieceForPromotion = piece;
			PromotionUi.Visible = true;
		}
	}
}

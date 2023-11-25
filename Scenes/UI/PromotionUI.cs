using Chess;
using Godot;
using System;

public partial class PromotionUI : PanelContainer
{
    private Button _bishopButton;
    private Button _knightButton;
    private Button _rookButton;
    private Button _queenButton;
    private Button _cancelButton;
    public Piece PieceForPromotion;
    private GameManager Manager;

    public override void _Ready()
    {
        base._Ready();
        _bishopButton = GetNode<Button>("%Bishop");
        _knightButton = GetNode<Button>("%Knight");
        _rookButton = GetNode<Button>("%Rook");
        _queenButton = GetNode<Button>("%Queen");
        _cancelButton = GetNode<Button>("%Cancel");
        Manager = Utils.GetManager(this);

        _bishopButton.Pressed += () => PromoteTo(PieceType.Bishop);
        _knightButton.Pressed += () => PromoteTo(PieceType.Knight);
        _rookButton.Pressed += () => PromoteTo(PieceType.Rook);
        _queenButton.Pressed += () => PromoteTo(PieceType.Queen);
        _cancelButton.Pressed += () => Visible = false;
    }

    private void PromoteTo(PieceType pieceType)
    {
        var newPiece = Manager.ChessManager.Game.PromotePawn(PieceForPromotion, pieceType);
        Manager.ChessManager.SpawnPiece(newPiece);
        Visible = false;
    }
}

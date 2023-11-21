using Chess;
using Godot;
using System.Collections.Generic;

public partial class ChessManager : Node
{

    // Game Objects
    public GameBoard Board => Utils.GetManager(this).Board;
    public Transform3D pieceParent;
    private PackedScene _pieceScene = ResourceLoader.Load<PackedScene>("res://Scenes/Piece/GamePiece.tscn");

    private PackedScene _pawnScene = ResourceLoader.Load<PackedScene>("res://Scenes/Piece/Pawn.tscn");
    private PackedScene _rookScene = ResourceLoader.Load<PackedScene>("res://Scenes/Piece/Rook.tscn");
    private PackedScene _knightScene = ResourceLoader.Load<PackedScene>("res://Scenes/Piece/Knight.tscn");
    private PackedScene _bishopScene = ResourceLoader.Load<PackedScene>("res://Scenes/Piece/Bishop.tscn");
    private PackedScene _kingScene = ResourceLoader.Load<PackedScene>("res://Scenes/Piece/King.tscn");
    private PackedScene _queenScene = ResourceLoader.Load<PackedScene>("res://Scenes/Piece/Queen.tscn");

    // public GameUIManager UIManager;

    // Settings
    public int boardSize = 8;

    public ChessGame Game { get; private set; }

    public Dictionary<Team, List<GamePiece>> teams = new() {
        { Team.White, new()},
        { Team.Black, new()}
    };

    public Dictionary<Team, HashSet<BoardSquare>> teamCoverage;

    public override void _Ready()
    {
        Utils.GetManager(this).RegisterChessManager(this);
        Game = new ChessGame();

        Game.SetupBoard();
        // Game.SetupStalemateBoard();
        Game.SetNextTurnCallBack(UpdateInfo);
        //Game.SetPromotionCallback(UIManager.SetPawnForPromotion);

        UpdateInfo();
    }

    public void UpdateInfo()
    {
        //UIManager.UpdateInfoText(Game.GameInfo());
        if (Game.CheckMate)
        {
            //UIManager.OnCheckMate();
        }
    }

    public void SetupPieces()
    {
        foreach (Team team in new List<Team> { Team.White, Team.Black })
        {
            Game.pieces.TryGetValue(team, out List<Piece> pieces);
            foreach (Piece piece in pieces)
            {
                SpawnPiece(piece);
            }
        }
    }

    public void SpawnPiece(Piece chessPiece)
    {
        GamePiece piece = GetSceneForPiece(chessPiece).Instantiate<GamePiece>();
        AddChild(piece);
        piece.TeamColor = chessPiece.TeamColor;
        piece.SetPiece(chessPiece);

        //piece.transform.parent = new GameObject($"{prefab} {colIndex + 1}").transform;
        //piece.transform.parent.parent = pieceParent;

        // Add piece to appropriate team list. 
        teams.TryGetValue(piece.TeamColor, out List<GamePiece> teamPieces);
        teamPieces.Add(piece);
        GD.Print($"{chessPiece} | {chessPiece.CurrentSquare} | {chessPiece.CurrentSquare.Coordinates}");
        var targetSquare = Board.GetSquare(chessPiece.CurrentSquare.Coordinates);
        piece.SetPositionToTargetSquare(targetSquare);

    }

    private PackedScene GetSceneForPiece(Piece chessPiece)
    {
        switch(chessPiece.type)
        {
            case PieceType.Pawn: return _pawnScene;
            case PieceType.Rook: return _rookScene;
            case PieceType.Knight: return _knightScene;
            case PieceType.Bishop: return _bishopScene;
            case PieceType.Queen: return _queenScene;
            case PieceType.King: return _kingScene;
            default: return _pieceScene;
        }
    }
}
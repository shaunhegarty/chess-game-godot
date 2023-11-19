using Chess;
using Godot;
using System;

[Tool]
public partial class BoardSquare : Node3D
{
    // Settings
    private Team _teamColor = Team.White;
    private float _squareSize = 1;

    // Children
    private Label3D _label;
    private MeshInstance3D _mesh;
    private ShaderMaterial _shader;
    private ShaderMaterial _highlight;
    private DropReceivable _dropReceivable;

    // State
    private Vector3 _basePosition;
    public Vector2I Coordinates;
    public GamePiece Occupant;
    private bool _setupComplete = false;


    public Vector3 BasePosition
    {
        get { return _basePosition; }
        set
        {
            _basePosition = value;
            Position = value;
        }
    }


    [Export]
    public Team TeamColor
    {
        get { return _teamColor; }
        set
        {
            SetTeam(value);
        }
    }

    [Export]
    public float SquareSize
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
        _shader = ((ShaderMaterial)_mesh.MaterialOverride);
        _highlight = ((ShaderMaterial)_shader.NextPass);
        _label = GetNode<Label3D>("%Colour");

        try
        {
            _dropReceivable = GetNode<DropReceivable>("%DropReceivable");
            _dropReceivable.Highlighted += () => SetHover(true);
            _dropReceivable.Unhighlighted += () => SetHover(false);
        } catch (InvalidCastException e)
        {
            GD.PrintErr($"{Name} | {e}");
        }
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
        _shader.SetShaderParameter("base_color", Utils.TeamColor(TeamColor));
    }

    static Color Unhighlighted = new(0.1f, 0.1f, 0.1f);
    static Color HighlightedValid = new(0.2f, 0.8f, 0.2f);
    static Color HighlightedInvalid = new(0.8f, 0.2f, 0.2f);
    static Color HighlightedHover = new(1f, 1f, 0);
    const float HighlightDuration = 0.1f;
    bool Hovering = false;
    bool Valid = false;

    private Color GetColorForState()
    {
        if(Valid && !Hovering)
        {
            return HighlightedValid;
        } else if (Valid && Hovering)
        {
            return HighlightedHover;
        } else if (!Valid && Hovering)
        {
            return HighlightedInvalid;
        } else
        {
            return Unhighlighted;
        }
    }

    private void DoHighlight()
    {
        Color targetColor = GetColorForState();
        Tween tween = GetTree().CreateTween();
        tween.TweenMethod(Callable.From<Color>(SetHighlightColor), GetHighlightColor(), targetColor, HighlightDuration);
    }

    private void SetHover(bool highlighted)
    {
        Hovering = highlighted;
        DoHighlight();
        
    }

    public void SetValid(bool valid)
    {
        Valid = valid;
        DoHighlight();
    }

    private void SetHighlightColor(Color color)
    {
        _highlight.SetShaderParameter("highlight_color", color);
    }

    private Color GetHighlightColor()
    {
        return (Color)_highlight.GetShaderParameter("highlight_color");
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
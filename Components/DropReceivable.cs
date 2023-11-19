using Godot;

public partial class DropReceivable : Area3D
{
    [Signal]
    public delegate void HighlightedEventHandler();

    [Signal]
    public delegate void UnhighlightedEventHandler();

    [Signal]
    public delegate void DroppedEventHandler();

    public void Highlight()
    {
        EmitSignal(SignalName.Highlighted);
    }

    public void Unhighlight()
    {
        EmitSignal(SignalName.Unhighlighted);
    }
}

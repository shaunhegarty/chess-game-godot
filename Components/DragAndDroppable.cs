using Godot;
using System;

public partial class DragAndDroppable : Area3D
{
    [Signal]
    public delegate void DroppedEventHandler(DropReceivable target);
    [Signal]
    public delegate void HighlightingEventHandler(DropReceivable target);

    Node3D PickedObject;
    RayCast3D DropRay;
    Vector3 DragInitialPosition;
    bool isDragged = false;
    GameManager Manager;
    DropReceivable HighlightingObject;


    public override void _Ready()
    {
        PickedObject = GetParent<Node3D>();
        DropRay = MakeRay();
        Manager = Utils.GetManager(this);


    }

    private RayCast3D MakeRay()
    {
        var ray = new RayCast3D
        {
            Enabled = false,
            TargetPosition = new(0, -1f, 0),
            CollisionMask = 2,
            CollideWithBodies = false,
            CollideWithAreas = true
        };
        AddChild(ray);
        return ray;
    }

    public override void _InputEvent(Camera3D camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIdx)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
        {
            if(mouseEvent.IsPressed())
            {
                SetDragged(true);
            }
            if(mouseEvent.IsReleased())
            {
                SetDragged(false);
            }
        }
    }



    private void SetDragged(bool IsDragged)
    {
        isDragged = IsDragged;
        if(IsDragged)
        {
            DragInitialPosition = PickedObject.GlobalPosition;
            Manager.Dragger.PickedDraggable = this;
            DropRay.Enabled = true;
        }
    }

    public void Release()
    {
        var collider = DropRay.GetCollider();
        if (collider is not null and DropReceivable)
        {
            var receivable = (DropReceivable)collider;
            EmitSignal(SignalName.Dropped, receivable);
        }
        else
        {
            PickedObject.GlobalPosition = DragInitialPosition;
        }
        DropRay.Enabled = false;

    }

    public void UpdatePosition(Vector3 positionUpdate)
    {
        PickedObject.GlobalPosition = positionUpdate;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(DropRay.Enabled)
        {
            var collider = DropRay.GetCollider();
            if (collider != HighlightingObject)
            {
                
                HighlightingObject?.Unhighlight();
                HighlightingObject = (DropReceivable) collider;
                HighlightingObject.Highlight();
            } 

        }
    }

}

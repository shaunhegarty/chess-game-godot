using Godot;
using System;

public partial class DragAndDroppable : Area3D
{
    [Signal]
    public delegate void DroppedEventHandler(DropReceivable target);

    Node3D PickedObject;
    RayCast3D DropRay;
    Vector3 DragInitialPosition;
    bool isDragged = false;

    const float RAY_LENGTH = 100;
    const int DRAG_PLANE_LAYER = 4;  // Collision mask for the drag plane

    public override void _Ready()
    {
        PickedObject = GetParent<Node3D>();
        DropRay = MakeRay();
        
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
                GD.Print("Start Dragging");
                SetDragged(true);
            }
            if(mouseEvent.IsReleased())
            {
                GD.Print("Release Dragging");
                SetDragged(false);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("left_click")) {
            GD.Print("Dragging");
            var spaceState = GetWorld3D().DirectSpaceState;
            var camera = GetViewport().GetCamera3D();
            var mousePos = GetViewport().GetMousePosition();
            var origin = camera.ProjectRayOrigin(mousePos);
            var end = origin + camera.ProjectRayNormal(mousePos) * RAY_LENGTH;
            var query = PhysicsRayQueryParameters3D.Create(origin, end);
            query.CollideWithAreas = true;
            query.CollisionMask = DRAG_PLANE_LAYER;
            var result = spaceState.IntersectRay(query);
            bool gotCollisionPoint = result.TryGetValue("position", out var collisionPoint);
            if (gotCollisionPoint)
            {
                UpdatePosition((Vector3)collisionPoint);
            }
        }
    }

    private void SetDragged(bool IsDragged)
    {
        isDragged = IsDragged;
        if(IsDragged)
        {
            DragInitialPosition = PickedObject.GlobalPosition;
            DropRay.Enabled = true;
        }
    }

    private void Release()
    {
        var collider = DropRay.GetCollider();
        if (collider != null && collider is DropReceivable)
        {
            var receivable = (DropReceivable)collider;
            UpdatePosition(receivable.GlobalPosition);
        }
        else
        {
            PickedObject.GlobalPosition = DragInitialPosition;
        }
        DropRay.Enabled = false;

    }

    private void UpdatePosition(Vector3 positionUpdate)
    {
        PickedObject.GlobalPosition = positionUpdate;
    }

}

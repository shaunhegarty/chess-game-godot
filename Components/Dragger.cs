using Godot;
using System;

public partial class Dragger : Node3D
{

    const float RAY_LENGTH = 100;
    const int DRAG_PLANE_LAYER = 4;  // Collision mask for the drag plane
    public DragAndDroppable PickedDraggable;

    public override void _Ready()
    {
        Utils.GetManager(this).RegisterDragger(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (PickedDraggable != null && Input.IsActionPressed("left_click"))
        {
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
                PickedDraggable.UpdatePosition((Vector3)collisionPoint);
            }
        }
        if (PickedDraggable != null && Input.IsActionJustReleased("left_click"))
        {
            PickedDraggable.Release();
            PickedDraggable = null;
        }
    }
}

using Godot;

public class Utils
{
    public static float Distance(Vector2I originPosition, Vector2I targetPosition)
    {
        Vector2 oldPosition = (Vector2)originPosition;
        Vector2 newPosition = (Vector2)targetPosition;
        return oldPosition.DistanceTo(newPosition);
    }
}

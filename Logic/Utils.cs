using Chess;
using Godot;

public class Utils
{

    private static readonly Material WhiteMaterial = ResourceLoader.Load<Material>("res://Materials/square_white.tres");
    private static readonly Material BlackMaterial = ResourceLoader.Load<Material>("res://Materials/square_black.tres");
    private static readonly ShaderMaterial OutlineMaterial = ResourceLoader.Load<ShaderMaterial>("res://Materials/selected_highlight_material.tres");
    private static readonly StandardMaterial3D PieceMaterial = ResourceLoader.Load<StandardMaterial3D>("res://Materials/piece_material.tres");

    public static float Distance(Vector2I originPosition, Vector2I targetPosition)
    {
        Vector2 oldPosition = (Vector2)originPosition;
        Vector2 newPosition = (Vector2)targetPosition;
        return oldPosition.DistanceTo(newPosition);
    }

    public static GameManager GetManager(Node node)
    {
        return node.GetNode<GameManager>("/root/GameManager");
    }

    public static Material TeamMaterial(Team team)
    {
        Material material = team == Team.White ? WhiteMaterial : BlackMaterial;
        return material;
    }

    public static Color TeamColor(Team team)
    {
        return team == Team.White ? new(1, 1, 1) : new(0, 0, 0);
    }

    public static ShaderMaterial GetOutlineMaterial()
    {
        return OutlineMaterial;
    }

    public static StandardMaterial3D GetPieceMaterial()
    {
        return PieceMaterial;
    }
}

using Godot;

namespace Chess
{
    public class Square
    {
        public Vector2I Coordinates;
        public SquareState state;
        public Piece occupant;

        public string Label { get { return LabelFromPosition(Coordinates); } }

        public static string LabelFromPosition(Vector2I pos)
        {
            return $"{(char)('A' + pos.Y)}{pos.X + 1}";
        }

        public Square(int x, int y)
        {
            Coordinates = new Vector2I(x, y);
        }

        public void AddPiece(Piece piece)
        {
            occupant = piece;
            state = SquareState.Occupied;
            piece.CurrentSquare = this;
        }

        public void RemovePiece()
        {
            state = SquareState.Empty;
            if (occupant != null)
            {
                occupant.CurrentSquare = null;
                occupant = null;
            }
        }

        public override string ToString()
        {
            return Label;
        }
    }


}



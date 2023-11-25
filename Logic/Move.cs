using Godot;

namespace Chess
{
    public class Move
    {
        Vector2I from;
        Vector2I to;
        readonly int turn;

        public Move(Vector2I moveFrom, Vector2I moveTo, int moveTurn)
        {
            // Ensure it's a copy
            from = new(moveFrom.X, moveFrom.Y);
            to = new(moveTo.X, moveTo.Y);
            turn = moveTurn;
        }

        public Vector2I Direction => to - from;
        public float Distance => Utils.Distance(from, to);

        public Vector2I To { get => to; }
        public Vector2I From { get => from; }

        public int Turn => turn;

        public override string ToString()
        {
            return $"{Square.LabelFromPosition(from)} to {Square.LabelFromPosition(to)} on turn {turn}";
        }
    }


}



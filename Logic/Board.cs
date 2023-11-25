using Godot;
using System.Collections.Generic;

namespace Chess
{
    public class Board
    {
        private readonly List<List<Square>> internalBoard;
        public int size = 0;

        public Board(int boardSize)
        {
            internalBoard = new();
            for (int x = 0; x < boardSize; x++)
            {
                List<Square> row = new();
                for (int y = 0; y < boardSize; y++)
                {
                    row.Add(new Square(x, y));
                    size += 1;
                }
                internalBoard.Add(row);
            }
        }

        public Square GetSquare(Vector2I position)
        {
            return internalBoard[position.X][position.Y];
        }
    }


}



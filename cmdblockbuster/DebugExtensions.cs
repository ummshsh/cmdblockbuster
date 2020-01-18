using System;

namespace cmdblockbuster
{
    public static class DebugExtensions
    {
        public static void Print(this CellType[,] array)
        {
            var xDimLenght = array.GetLength(0);
            var yDimLenght = array.GetLength(1);
            for (int row = 0; row < xDimLenght; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < yDimLenght; rowItemIndex++)
                {
                    Console.Write((int)array[row, rowItemIndex] + " ");
                }
                Console.Write(Environment.NewLine);
            }
        }
    }
}

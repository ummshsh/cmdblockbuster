using System;
using System.Linq;

namespace cmdblockbuster.Common
{
    public static class ArrayExtensions
    {
        public static CellType[] GetColumn(this CellType[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public static CellType[] GetRow(this CellType[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

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

        public static int GetRowsLenght(this CellType[,] array) => array.GetLength(0);

        public static int GetColumnsLenght(this CellType[,] array) => array.GetLength(1);
    }
}

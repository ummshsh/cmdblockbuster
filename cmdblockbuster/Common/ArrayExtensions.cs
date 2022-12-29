using System.Linq;

namespace CMDblockbuster.Common
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

        public static int GetRowsLenght(this CellType[,] array) => array.GetLength(0);

        public static int GetColumnsLenght(this CellType[,] array) => array.GetLength(1);
    }
}

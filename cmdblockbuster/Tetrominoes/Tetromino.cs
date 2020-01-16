using System;

namespace cmdblockbuster.Tetrominoes
{
    public abstract class Tetromino
    {
        public CellType[,] Cells { get; set; }

        public int XDimLenght => Cells.GetLength(0);
        public int YDimLenght => Cells.GetLength(1);

        /// <summary>
        /// Zero based, true for most of the tetrominoes except I and O
        /// </summary>
        public Tuple<int, int> SpawnLocation = new Tuple<int, int>(4, 1);

        public TetrominoMoves[] TetrominoMoves;

        public bool landed = false;

        public void RotateRight()
        {
            Transpose();
            ReverseEachRow();
        }

        public void RotateLeft()
        {
            ReverseEachRow();
            Transpose();
        }

        private void ReverseEachRow()
        {
            // Iterate rows
            for (int row = 0; row < XDimLenght; row++)
            {
                // change each row
                var rowArray = new CellType[YDimLenght];
                for (int rowItemIndex = 0; rowItemIndex < YDimLenght; rowItemIndex++)
                {
                    rowArray[rowItemIndex] = Cells[row, rowItemIndex];
                }
                Array.Reverse(rowArray);

                for (int rowItemIndex = 0; rowItemIndex < YDimLenght; rowItemIndex++)
                {
                    Cells[row, rowItemIndex] = rowArray[rowItemIndex];
                }
            }
        }

        private void Transpose()
        {
            var result = new CellType[YDimLenght, XDimLenght];

            for (int i = 0; i < XDimLenght; i++)
            {
                for (int j = 0; j < YDimLenght; j++)
                {
                    result[j, i] = Cells[i, j];
                }
            }
            Cells = result;
        }
    }
}

using cmdblockbuster.Field;
using System;

namespace cmdblockbuster.Tetrominoes
{
    public abstract class Tetromino
    {
        public int[,] Cells { get; set; }

        public TetrominoMoves[] TetrominoMoves;

        // TODO: roatations: check field before rotation, 
        // looks like field should check it and kick if allowed
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
            var xDimLenght = Cells.GetLength(0);
            var yDimLenght = Cells.GetLength(1);

            // Iterate rows
            for (int row = 0; row < xDimLenght; row++)
            {
                // change each row
                var rowArray = new int[yDimLenght];
                for (int rowItemIndex = 0; rowItemIndex < yDimLenght; rowItemIndex++)
                {
                    rowArray[rowItemIndex] = Cells[row, rowItemIndex];
                }
                Array.Reverse(rowArray);

                for (int rowItemIndex = 0; rowItemIndex < yDimLenght; rowItemIndex++)
                {
                    Cells[row, rowItemIndex] = rowArray[rowItemIndex];
                }
            }
        }

        private void Transpose()
        {
            int width = Cells.GetLength(0);
            int height = Cells.GetLength(1);

            var result = new int[height, width];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result[j, i] = Cells[i, j];
                }
            }
            Cells = result;
        }
    }
}

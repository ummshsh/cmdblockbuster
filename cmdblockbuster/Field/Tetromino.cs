using cmbblockbuster.Field;

namespace cmbblockbuster
{
    public abstract class Tetromino
    {
        public Cell[,] Cells;

        public TetrominoMoves[] TetrominoMoves;

        public bool RotateRight()
        {
            Transpose();
            ReverseEachRow();
            return true; // TODO: check field before rotation
        }

        public bool RotateLeft()
        {
            ReverseEachRow();
            Transpose();
            return true; // TODO: check field before rotation
        }

        private void ReverseEachRow()
        {

        }

        private void Transpose()
        {
            int width = Cells.GetLength(0);
            int height = Cells.GetLength(1);

            Cell[,] result = new Cell[height, width];

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

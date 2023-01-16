using CMDblockbuster.Field;
using CMDblockbuster.Tetrominoes;

namespace cmdblockbuster.Field
{
    /// <summary>
    /// Passed to renderer and contains Tetromino and Ghost tetromino
    /// </summary>
    internal class VisiblePlayfield : IPlayefield
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public Cell[,] StaticCells { get; private set; }

        public Cell[,] StaticCellsWithTetrominoAndTetrominoGhost { get; private set; }

        public Cell this[int row, int rowItem]
        {
            get
            {
                return StaticCells[row, rowItem];
            }
            set
            {
                StaticCells[row, rowItem] = value;
            }
        }

        public Tetromino Tetromino { get; set; }

        public Tetromino TetrominoGhost { get; set; }


        public VisiblePlayfield(int width, int height)
        {
            Width = width;
            Height = height;

            // Create emty cells
            this.StaticCells = new Cell[Height, Width];

            var rowsCount = this.StaticCells.GetLength(0);
            var rowLength = this.StaticCells.GetLength(1);

            for (int row = 0; row < rowsCount; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < rowLength; rowItemIndex++)
                {
                    this.StaticCells[row, rowItemIndex] = Cell.EmptyCell;
                }
            }
        }
    }
}

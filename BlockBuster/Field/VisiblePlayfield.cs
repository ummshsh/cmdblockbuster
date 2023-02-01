using BlockBuster.Tetrominoes;

namespace BlockBuster.Field
{

    /// <summary>
    /// Passed to renderer and contains Tetromino and Ghost tetromino
    /// </summary>
    public class VisiblePlayfield : IPlayefield
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public Cell[,] Cells { get; private set; }

        public Cell[,] StaticCellsWithTetrominoAndTetrominoGhost { get; private set; }

        public Cell this[int row, int rowItem]
        {
            get
            {
                return Cells[row, rowItem];
            }
            set
            {
                Cells[row, rowItem] = value;
            }
        }

        public Tetromino Tetromino { get; set; }

        public Tetromino TetrominoGhost { get; set; }


        public VisiblePlayfield(int width, int height)
        {
            Width = width;
            Height = height;

            // Create emty cells
            this.Cells = new Cell[Height, Width];

            var rowsCount = this.Cells.GetLength(0);
            var rowLength = this.Cells.GetLength(1);

            for (int row = 0; row < rowsCount; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < rowLength; rowItemIndex++)
                {
                    this.Cells[row, rowItemIndex] = Cell.EmptyCell;
                }
            }
        }
    }
}
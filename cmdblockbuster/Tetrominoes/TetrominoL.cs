namespace cmdblockbuster.Tetrominoes
{
    class TetrominoL : Tetromino
    {
        public TetrominoL()
        {
            var empty = CellType.Empty;
            var filled = CellType.Orange;

            Cells = new CellType[,] {
                {empty,empty,filled},
                {filled,filled,filled},
                {empty,empty,empty}
            };
        }
    }
}

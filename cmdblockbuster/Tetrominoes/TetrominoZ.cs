namespace cmdblockbuster.Tetrominoes
{
    class TetrominoZ : Tetromino
    {
        public TetrominoZ()
        {
            var empty = CellType.Empty;
            var filled = CellType.Red;

            Cells = new CellType[,] {
                {filled,filled,empty},
                {empty,filled,filled},
                {empty,empty,empty}
            };
        }
    }
}

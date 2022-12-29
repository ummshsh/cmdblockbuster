namespace CMDblockbuster.Tetrominoes
{
    class TetrominoS : Tetromino
    {
        public TetrominoS()
        {
            var empty = CellType.Empty;
            var filled = CellType.Green;

            Cells = new CellType[,] {
                {empty,filled,filled},
                {filled,filled,empty},
                {empty,empty,empty}
            };
        }
    }
}

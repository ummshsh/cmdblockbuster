namespace CMDblockbuster.Tetrominoes
{
    class TetrominoT : Tetromino
    {
        public TetrominoT()
        {
            var empty = CellType.Empty;
            var filled = CellType.Purple;

            Cells = new CellType[,] {
                {empty,filled,empty},
                {filled,filled,filled},
                {empty,empty,empty}
            };
        }
    }
}

namespace cmdblockbuster.Tetrominoes
{
    class TetrominoJ : Tetromino
    {
        public TetrominoJ()
        {
            var empty = CellType.Empty;
            var filled = CellType.Blue;

            Cells = new CellType[,] {
                {filled,empty,empty},
                {filled,filled,filled},
                {empty,empty,empty}
            };
        }
    }
}

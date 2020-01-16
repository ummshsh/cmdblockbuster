namespace cmdblockbuster.Tetrominoes
{
    class TetrominoI : Tetromino
    {
        public TetrominoI()
        {
            SpawnLocation = new System.Tuple<int, int>(3, 1);

            var empty = CellType.Empty;
            var filled = CellType.Cyan;

            Cells = new CellType[,] {
                {empty,empty,empty,empty},
                {filled,filled,filled,filled},
                {empty,empty,empty,empty},
                {empty,empty,empty,empty}
            };
        }
    }
}

namespace cmdblockbuster.Tetrominoes
{
    class TetrominoO : Tetromino
    {
        public TetrominoO()
        {
            SpawnLocation = new System.Tuple<int, int>(3, 1);

            var empty = CellType.Empty;
            var filled = CellType.Yellow;

            Cells = new CellType[,] {
                {empty,filled,filled,empty},
                {empty,filled,filled,empty},
                {empty,empty,empty,empty}
            };
        }
    }
}

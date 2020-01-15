namespace cmdblockbuster.Tetrominoes
{
    class TetrominoS : Tetromino
    {
        public TetrominoS()
        {
            Cells = new int[,] {
                {0,1,1},
                {1,1,0},
                {0,0,0}
            };
        }
    }
}

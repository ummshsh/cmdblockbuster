namespace cmdblockbuster.Tetrominoes
{
    class TetrominoI : Tetromino
    {
        public TetrominoI()
        {
            Cells = new int[,] {
                {0,0,0,0},
                {1,1,1,1},
                {0,0,0,0},
                {0,0,0,0}
            };
        }
    }
}

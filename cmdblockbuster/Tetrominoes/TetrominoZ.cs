namespace cmdblockbuster.Tetrominoes
{
    class TetrominoZ : Tetromino
    {
        public TetrominoZ()
        {
            Cells = new int[,] {
                {1,1,0},
                {0,1,1},
                {0,0,0}
            };
        }
    }
}

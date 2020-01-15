namespace cmdblockbuster.Tetrominoes
{
    class TetrominoL : Tetromino
    {
        public TetrominoL()
        {
            Cells = new int[,] {
                {0,0,1},
                {1,1,1},
                {0,0,0}
            };
        }
    }
}

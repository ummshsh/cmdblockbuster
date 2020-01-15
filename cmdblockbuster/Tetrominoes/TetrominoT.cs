namespace cmdblockbuster.Tetrominoes
{
    class TetrominoT : Tetromino
    {
        public TetrominoT()
        {
            Cells = new int[,] {
                {0,1,0},
                {1,1,1},
                {0,0,0}
            };
        }
    }
}

using cmdblockbuster.Tetrominoes;

namespace CMDblockbuster.Tetrominoes
{
    class TetrominoJ : Tetromino
    {
        public TetrominoJ()
        {
            var empty = TetrominoCellType.Empty;
            var filled = TetrominoCellType.Blue;

            Cells = new TetrominoCellType[,] {
                {filled,empty,empty},
                {filled,filled,filled},
                {empty,empty,empty}
            };
        }
    }
}

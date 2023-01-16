using cmdblockbuster.Tetrominoes;
using System;

namespace CMDblockbuster.Tetrominoes
{
    [Serializable]
    class TetrominoL : Tetromino
    {
        public TetrominoL()
        {
            var empty = TetrominoCellType.Empty;
            var filled = TetrominoCellType.Orange;

            Cells = new TetrominoCellType[,] {
                {empty,empty,filled},
                {filled,filled,filled},
                {empty,empty,empty}
            };
        }
    }
}

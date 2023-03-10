using System;

namespace BlockBuster.Tetrominoes
{

    [Serializable]
    class TetrominoT : Tetromino
    {
        public TetrominoT()
        {
            var empty = TetrominoCellType.Empty;
            var filled = TetrominoCellType.Purple;

            Cells = new TetrominoCellType[,] {
            {empty,filled,empty},
            {filled,filled,filled},
            {empty,empty,empty}
        };
        }
    }
}
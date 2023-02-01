using System;

namespace BlockBuster.Tetrominoes
{

    [Serializable]
    class TetrominoZ : Tetromino
    {
        public TetrominoZ()
        {
            var empty = TetrominoCellType.Empty;
            var filled = TetrominoCellType.Red;

            Cells = new TetrominoCellType[,] {
            {filled,filled,empty},
            {empty,filled,filled},
            {empty,empty,empty}
        };
        }
    }
}
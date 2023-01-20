using System;

namespace BlockBuster.Tetrominoes;

[Serializable]
class TetrominoS : Tetromino
{
    public TetrominoS()
    {
        var empty = TetrominoCellType.Empty;
        var filled = TetrominoCellType.Green;

        Cells = new TetrominoCellType[,] {
            {empty,filled,filled},
            {filled,filled,empty},
            {empty,empty,empty}
        };
    }
}

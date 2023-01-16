using cmdblockbuster.Tetrominoes;
using System;

namespace CMDblockbuster.Tetrominoes
{
    [Serializable]
    public class TetrominoI : Tetromino
    {
        public TetrominoI()
        {
            SpawnLocation = new System.Tuple<int, int>(0, 3);

            var empty = TetrominoCellType.Empty;
            var filled = TetrominoCellType.Cyan;

            Cells = new TetrominoCellType[,] {
                {empty,empty,empty,empty},
                {filled,filled,filled,filled},
                {empty,empty,empty,empty},
                {empty,empty,empty,empty}
            };
        }
    }
}

using cmdblockbuster.Tetrominoes;
using System;

namespace CMDblockbuster.Tetrominoes
{
    [Serializable]
    class TetrominoO : Tetromino
    {
        public TetrominoO()
        {
            SpawnLocation = new System.Tuple<int, int>(0, 3);

            var empty = TetrominoCellType.Empty;
            var filled = TetrominoCellType.Yellow;

            Cells = new TetrominoCellType[,] {
                {empty,filled,filled,empty},
                {empty,filled,filled,empty},
                {empty,empty,empty,empty}
            };
        }

        public override MinoRotationTransition RotateLeft()
        {
            // In SRS O mino have only 1 rotation
            return MinoRotationTransition.Rotation_0_0;
        }

        public override MinoRotationTransition RotateRight()
        {
            // In SRS O mino have only 1 rotation
            return MinoRotationTransition.Rotation_0_0;
        }
    }
}

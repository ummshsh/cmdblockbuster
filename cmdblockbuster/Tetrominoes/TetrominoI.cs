using cmdblockbuster.Tetrominoes;
using System;
using System.Collections.Generic;

namespace CMDblockbuster.Tetrominoes
{
    [Serializable]
    public class TetrominoI : Tetromino
    {
        public TetrominoI()
        {
            SpawnLocation = new Tuple<int, int>(0, 3);

            var empty = TetrominoCellType.Empty;
            var filled = TetrominoCellType.Cyan;

            Cells = new TetrominoCellType[,] {
                {empty,empty,empty,empty},
                {filled,filled,filled,filled},
                {empty,empty,empty,empty},
                {empty,empty,empty,empty}
            };

            wallKicks = new Dictionary<MinoRotationTransition, List<Tuple<int, int>>>
            {
                { MinoRotationTransition.Rotation_0_R, new List<Tuple<int,int>>(){ new Tuple<int, int>(-2,0), new Tuple<int, int>(1,0), new Tuple<int,int>(-2,-1), new Tuple<int, int>(1,2) } },
                { MinoRotationTransition.Rotation_R_0, new List<Tuple<int,int>>(){ new Tuple<int, int>(2,0), new Tuple<int, int>(-1,0), new Tuple<int,int>(2,1), new Tuple<int, int>(-1,-2) } },
                { MinoRotationTransition.Rotation_R_2, new List<Tuple<int,int>>(){ new Tuple<int, int>(-1,0), new Tuple<int, int>(2,0), new Tuple<int,int>(-1,2), new Tuple<int, int>(2,-1) } },
                { MinoRotationTransition.Rotation_2_R, new List<Tuple<int,int>>(){ new Tuple<int, int>(1,0), new Tuple<int, int>(-2,1), new Tuple<int,int>(1,-2), new Tuple<int, int>(-2,1) } },
                { MinoRotationTransition.Rotation_2_L, new List<Tuple<int,int>>(){ new Tuple<int, int>(2,0), new Tuple<int, int>(-1,0), new Tuple<int,int>(2,1), new Tuple<int, int>(-1,-2) } },
                { MinoRotationTransition.Rotation_L_2, new List<Tuple<int,int>>(){ new Tuple<int, int>(-2,0), new Tuple<int, int>(1,0), new Tuple<int,int>(-2,-1), new Tuple<int, int>(1,2) } },
                { MinoRotationTransition.Rotation_L_0, new List<Tuple<int,int>>(){ new Tuple<int, int>(1,0), new Tuple<int, int>(-2,0), new Tuple<int,int>(1,-2), new Tuple<int, int>(-2,1) } },
                { MinoRotationTransition.Rotation_0_L, new List<Tuple<int,int>>(){ new Tuple<int, int>(-1, 0), new Tuple<int, int>(+2, 0), new Tuple<int, int>(-1, 2), new Tuple<int, int>(2, -1) } }
            };
        }
    }
}

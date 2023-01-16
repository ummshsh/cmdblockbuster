using System;

namespace CMDblockbuster.Tetrominoes
{
    [Serializable]
    public class TetrominoRotation
    {
        private MinoRotaion _rotation = MinoRotaion.Rotation_0;

        public void RotateRight() => _rotation = (int)_rotation + 1 > 3 ? MinoRotaion.Rotation_0 : _rotation + 1;

        public void RotateLeft() => _rotation = (int)_rotation - 1 < 0 ? MinoRotaion.Rotation_L : _rotation - 1;
    }
}

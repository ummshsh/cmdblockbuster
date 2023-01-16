using System;

namespace CMDblockbuster.Tetrominoes
{
    [Serializable]
    public class TetrominoRotation
    {
        public MinoRotation Rotation { get; private set; } = MinoRotation.Rotation_0;

        public MinoRotationTransition RotateRight()
        {
            var originalRotation = Rotation.ToString().Replace("Rotation_", "");
            Rotation = (int)Rotation + 1 > 3 ? MinoRotation.Rotation_0 : Rotation + 1;
            var newRotation = Rotation.ToString().Replace("Rotation_", "");


            return Enum.Parse<MinoRotationTransition>($"Rotation_{originalRotation}_{newRotation}");
        }

        public MinoRotationTransition RotateLeft()
        {
            var originalRotation = Rotation.ToString().Replace("Rotation_", "");
            Rotation = (int)Rotation - 1 < 0 ? MinoRotation.Rotation_L : Rotation - 1;
            var newRotation = Rotation.ToString().Replace("Rotation_", "");

            return Enum.Parse<MinoRotationTransition>($"Rotation_{originalRotation}_{newRotation}");
        }
    }
}

namespace cmdblockbuster.Field
{
    public class Playfield : IPlayefield
    {
        public static int Width => 10;
        public static int Height => 22; // Plus 2 invisible rows to spawn tetrominos

        public CellType[,] field = new CellType[Height, Width];

        public Playfield()
        {
            var xDimLenght = field.GetLength(0);
            var yDimLenght = field.GetLength(1);

            for (int row = 0; row < xDimLenght; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < yDimLenght; rowItemIndex++)
                {
                    field[row, rowItemIndex] = CellType.Empty;
                }
            }
        }
    }
}

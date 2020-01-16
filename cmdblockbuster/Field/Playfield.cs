namespace cmdblockbuster.Field
{
    public class Playfield
    {
        private const int Width = 10;
        private const int Height = 22; // Two extra invisible rows to spawn tetrominos

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

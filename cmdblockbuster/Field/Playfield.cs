namespace CMDblockbuster.Field
{
    public class Playfield : IPlayefield
    {
        public int Width { get; private set; }

        public int Height { get; private set; }

        public readonly CellType[,] field;

        public CellType this[int row, int rowItem]
        {
            get
            {
                return field[row, rowItem];
            }
            set
            {
                field[row, rowItem] = value;
            }
        }

        public Playfield()
        {
            Width = 10;
            Height = 22;

            this.field = ConsructField();
        }

        public Playfield(int width, int height)
        {
            Width = width;
            Height = height;

            this.field = ConsructField();
        }

        private CellType[,] ConsructField()
        {
            var field = new CellType[Height, Width];

            var xDimLenght = field.GetLength(0);
            var yDimLenght = field.GetLength(1);

            for (int row = 0; row < xDimLenght; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < yDimLenght; rowItemIndex++)
                {
                    field[row, rowItemIndex] = CellType.Empty;
                }
            }

            return field;
        }
    }
}

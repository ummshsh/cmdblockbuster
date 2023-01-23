using BlockBuster.Tetrominoes;

namespace BlockBuster.Field;

internal class InnerPlayfield : IPlayefield
{
    public int Width { get; private set; }

    public int Height { get; private set; }

    public TetrominoCellType[,] Cells { get; private set; }

    public TetrominoCellType this[int row, int rowItem]
    {
        get
        {
            return Cells[row, rowItem];
        }
        set
        {
            Cells[row, rowItem] = value;
        }
    }

    public bool IsEmpty
    {
        get
        {
            for (int row = 0; row < Height; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < Width; rowItemIndex++)
                {
                    if (Cells[row, rowItemIndex] != TetrominoCellType.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }

    public InnerPlayfield(int width, int height)
    {
        Width = width;
        Height = height;

        this.Cells = ConsructField();
    }

    private TetrominoCellType[,] ConsructField()
    {
        var field = new TetrominoCellType[Height, Width];

        var xDimLenght = field.GetLength(0);
        var yDimLenght = field.GetLength(1);

        for (int row = 0; row < xDimLenght; row++)
        {
            for (int rowItemIndex = 0; rowItemIndex < yDimLenght; rowItemIndex++)
            {
                field[row, rowItemIndex] = TetrominoCellType.Empty;
            }
        }

        return field;
    }
}

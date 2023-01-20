using BlockBusterXaml.Tetrominoes;
using System.Drawing;

namespace BlockBusterXaml.Field;

public class Cell : IPlayfieldCell
{
    public Color Color { get; set; } = Color.Empty;

    public bool Ghost { get; set; } = false;

    public bool IsEmpty { get; set; } = true;

    public static EmptyPlayfieldCell EmptyCell => new EmptyPlayfieldCell();

    public Cell(Color colorColor, bool ghost, bool isEmpty)
    {
        Color = colorColor;
        Ghost = ghost;
        IsEmpty = isEmpty;
    }

    public static Cell FromInnerCell(TetrominoCellType type, bool ghost)
    {
        var color = type switch
        {
            TetrominoCellType.Red => Color.Red,
            TetrominoCellType.Cyan => Color.Cyan,
            TetrominoCellType.Purple => Color.Purple,
            TetrominoCellType.Green => Color.Green,
            TetrominoCellType.Yellow => Color.Yellow,
            TetrominoCellType.Orange => Color.Orange,
            TetrominoCellType.Blue => Color.Blue,
            _ => Color.White,
        };

        return new Cell(color, ghost, type == 0);
    }
}

public class EmptyPlayfieldCell : Cell
{
    public EmptyPlayfieldCell() : base(Color.White, false, true)
    {
    }
}

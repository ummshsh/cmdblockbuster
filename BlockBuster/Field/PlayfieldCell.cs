using BlockBuster.Tetrominoes;
using System;
using System.Drawing;

namespace BlockBuster.Field
{

    public class Cell : IPlayfieldCell
    {
        public Color Color { get; set; } = Color.Empty;

        public TetrominoCellType TetrominoCellType { get; private set; }

        public bool Ghost { get; set; } = false;

        public bool IsEmpty { get; set; } = true;

        public static EmptyPlayfieldCell EmptyCell => new();

        public Cell(Color colorColor, bool ghost, bool isEmpty, TetrominoCellType tetrominoCellType)
        {
            Color = colorColor;
            Ghost = ghost;
            IsEmpty = isEmpty;
            TetrominoCellType = tetrominoCellType;
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

            return new Cell(color, ghost, type == 0, type);
        }

        public override bool Equals(object obj)
        {
            return obj is not null &
                (obj as Cell).Ghost == this.Ghost &
                (obj as Cell).TetrominoCellType.Equals(this.TetrominoCellType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, TetrominoCellType, Ghost, IsEmpty);
        }
    }

    public class EmptyPlayfieldCell : Cell
    {
        public EmptyPlayfieldCell() : base(Color.White, false, true, TetrominoCellType.Empty)
        {
        }
    }
}
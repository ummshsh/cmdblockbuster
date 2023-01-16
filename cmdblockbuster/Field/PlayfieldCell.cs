using System.Drawing;

namespace cmdblockbuster.Field
{
    internal class Cell : IPlayfieldCell
    {
        public Color ColorColor { get; set; } = Color.Empty;

        public bool Ghost { get; set; } = false;

        public bool IsEmpty { get; set; } = true;

        public static EmptyPlayfieldCell EmptyCell => new EmptyPlayfieldCell();
    }

    internal class EmptyPlayfieldCell : Cell
    {
        public EmptyPlayfieldCell()
        {
            ColorColor = Color.White;
            IsEmpty = true;
        }
    }
}

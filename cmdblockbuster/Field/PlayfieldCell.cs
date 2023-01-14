using System.Drawing;

namespace cmdblockbuster.Field
{
    internal class Cell
    {
        public Color ColorColor { get; set; } = Color.White;
        public bool Ghost { get; set; } = false;
    }

    internal class EmptyPlayfieldCell : Cell
    {

    }
}

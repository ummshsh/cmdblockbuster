using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace cmdblockbuster.Field
{
    internal class PlayfieldCell
    {
        public Color ColorColor { get; set; } = Color.White;
        public bool Ghost { get; set; } = false;
    }
}

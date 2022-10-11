using cmdblockbuster.Field;
using System;

namespace cmdblockbuster.Renderer
{
    internal class ConsoleRenderer : ITetrisRenderer
    {
        public ConsoleRenderer() => Console.CursorVisible = false;

        private void PrintToConsole(CellType[,] array)
        {
            var xDimLenght = array.GetLength(0);
            var yDimLenght = array.GetLength(1);
            for (int row = 0; row < xDimLenght; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < yDimLenght; rowItemIndex++)
                {
                    Console.Write((int)array[row, rowItemIndex] + " ");
                }
                Console.Write(Environment.NewLine);
            }
        }

        void ITetrisRenderer.RenderPlayfield(object sender, Playfield playfield)
        {
            Console.SetCursorPosition(0, 0);
            PrintToConsole(playfield.field);
            ////throw new NotImplementedException(); // TODO: pass playfield to renderer
        }
    }
}

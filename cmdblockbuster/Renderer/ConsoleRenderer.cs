using CMDblockbuster.Field;
using System;
using System.Linq;

namespace CMDblockbuster.Renderer
{
    [Obsolete]
    internal class ConsoleRenderer : ITetrisRenderer
    {
        public ConsoleRenderer()
        {
            Console.CursorVisible = false;
        }

        private CellType[,] lastUpdatedField;
        private bool DisplayFirstTime = true;

        public void PrintToConsole(CellType[,] array)
        {
            if (SequenceEquals(lastUpdatedField, array) && !DisplayFirstTime)
            {
                return; // Exit if playfield updated already
            }

            var height = array.GetLength(0);
            var width = array.GetLength(1);
            for (int row = 0; row < height; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < width; rowItemIndex++)
                {
                    if (lastUpdatedField[row, rowItemIndex].Equals(array[row, rowItemIndex]) && !DisplayFirstTime)
                    {
                        continue; // Do not update what is already up to date
                    }
                    else
                    {
                        Console.SetCursorPosition(rowItemIndex, row);

                        int value = (int)array[row, rowItemIndex];
                        if (value > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        Console.Write(value);
                    }

                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, height);

            // If field was updated, we should remember latest update
            Array.Copy(array, lastUpdatedField, array.LongLength);
            DisplayFirstTime = false;
        }

        void ITetrisRenderer.RenderPlayfield(object sender, Playfield playfield)
        {
            Console.SetCursorPosition(0, 0);
            lastUpdatedField = new CellType[playfield.Height, playfield.Width];

            PrintToConsole(playfield.field);
        }

        public static bool SequenceEquals<T>(T[,] a, T[,] b)
        {
            return a?.Rank == b?.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());
        }
    }
}

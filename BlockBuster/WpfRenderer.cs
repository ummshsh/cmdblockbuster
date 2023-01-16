using cmdblockbuster.Field;
using cmdblockbuster.Game;
using CMDblockbuster.Renderer;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlockBuster
{
    public class WpfRenderer : ITetrisRenderer
    {
        private StackPanel dockStats;
        private Grid playfieldGrid;

        private Cell[,] lastUpdatedField;
        private readonly bool DisplayFirstTime = true;

        public WpfRenderer(Grid playfieldGrid, StackPanel dockStats)
        {
            this.dockStats = dockStats;
            this.playfieldGrid = playfieldGrid;
            lastUpdatedField = new Cell[22, 10];

            this.lastUpdatedField = new Cell[22, 10];

            var rowsCount = this.lastUpdatedField.GetLength(0);
            var rowLength = this.lastUpdatedField.GetLength(1);

            for (int row = 0; row < rowsCount; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < rowLength; rowItemIndex++)
                {
                    this.lastUpdatedField[row, rowItemIndex] = Cell.EmptyCell;
                }
            }
        }

        public void RenderPlayfield(object sender, VisiblePlayfield e)
        {
            if (SequenceEquals(lastUpdatedField, e.Cells) && !DisplayFirstTime)
            {
                return; // Exit if playfield updated already
            }

            DispatcherExtensions.BeginInvoke(Application.Current.Dispatcher, () =>
            {
                this.playfieldGrid.Children.Clear();

                var height = e.Height;
                var width = e.Width;

                for (int row = 0; row < height; row++)
                {
                    for (int rowItemIndex = 0; rowItemIndex < width; rowItemIndex++)
                    {
                        if (lastUpdatedField[row, rowItemIndex].Equals(e[row, rowItemIndex]) && !DisplayFirstTime)
                        {
                            continue; // Do not update what is already up to date
                        }
                        else
                        {
                            Cell cell = e[row, rowItemIndex];
                            SolidColorBrush solidColorBrush = GetColor(cell);

                            var border = new Border
                            {
                                BorderBrush = Brushes.Black,
                                Background = cell.Ghost ? Brushes.LightGray : solidColorBrush,
                                BorderThickness = new Thickness(0.5)
                            };
                            Grid.SetRow(border, row);
                            Grid.SetColumn(border, rowItemIndex);
                            this.playfieldGrid.Children.Add(border);
                        }
                    }
                }
            });
        }

        public void RenderGameState(object sender, GameState e)
        {
            DispatcherExtensions.BeginInvoke(Application.Current.Dispatcher, () =>
            {
                var childred = dockStats.Children;
                (dockStats.Children[0] as TextBlock).Text = "Hold: " + e.Queue.HoldTetrominoType?.Name;
                (dockStats.Children[1] as TextBlock).Text = "Score: " + e.Score;
                (dockStats.Children[2] as TextBlock).Text = "Lines Cleared: " + e.LinesCleared;
                (dockStats.Children[3] as TextBlock).Text = "Level: " + e.Level;
                (dockStats.Children[4] as TextBlock).Text = "Next: " + e.Queue.NextTetromino.Name;
            });
        }

        private SolidColorBrush GetColor(Cell tetrominoCellType)
        {
            var color = new SolidColorBrush(
                System.Windows.Media.Color.FromArgb(
                    tetrominoCellType.Color.A,
                    tetrominoCellType.Color.R,
                    tetrominoCellType.Color.G,
                    tetrominoCellType.Color.B));

            return color;
        }

        private bool SequenceEquals<T>(T[,] a, T[,] b)
        {
            return a?.Rank == b?.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());
        }
    }
}

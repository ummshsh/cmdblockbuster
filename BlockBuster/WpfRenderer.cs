using cmdblockbuster.Field;
using cmdblockbuster.State;
using cmdblockbuster.Tetrominoes;
using CMDblockbuster.Renderer;
using CMDblockbuster.Tetrominoes;
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
        private Grid holdGrid;
        private readonly Grid nextGrid;
        private Cell[,] lastUpdatedField;
        private Type cachedHoldMino;
        private Type cachedNextMino;
        private string cachedTextStats;
        private readonly bool DisplayFirstTime = true;

        public WpfRenderer(Grid playfieldGrid, StackPanel dockStats, Grid holdGrid, Grid nextGrid)
        {
            this.dockStats = dockStats;
            this.playfieldGrid = playfieldGrid;
            this.holdGrid = holdGrid;
            this.nextGrid = nextGrid;
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
                if (cachedTextStats != "" + e.Score + e.LinesCleared + e.Level)
                {
                    var children = dockStats.Children;
                    (dockStats.Children[0] as TextBlock).Text = "Score: " + e.Score;
                    (dockStats.Children[1] as TextBlock).Text = "Lines Cleared: " + e.LinesCleared;
                    (dockStats.Children[2] as TextBlock).Text = "Level: " + (int)e.Level;
                    cachedTextStats = "" + e.Score + e.LinesCleared + e.Level;
                }

                if (cachedHoldMino != e.Queue.HoldTetrominoType)
                {
                    FillGridWithMinoImage(holdGrid, e.Queue.HoldTetrominoType);
                    cachedHoldMino = e.Queue.HoldTetrominoType;
                }

                if (cachedNextMino != e.Queue.NextTetromino)
                {
                    FillGridWithMinoImage(nextGrid, e.Queue.NextTetromino);
                    cachedNextMino = e.Queue.NextTetromino;
                }
            });
        }

        private void FillGridWithMinoImage(Grid grid, Type minoType)
        {
            if (minoType is null)
            {
                return;
            }

            grid.Children.Clear();
            var mino = Activator.CreateInstance(minoType) as Tetromino;

            for (int row = 0; row < 4; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < 4; rowItemIndex++)
                {
                    TetrominoCellType cell;
                    if (mino.ColumnsLenght - 1 < rowItemIndex || mino.RowsLenght - 1 < row)
                    {
                        cell = TetrominoCellType.Empty;
                    }
                    else
                    {
                        cell = mino.Cells[row, rowItemIndex];
                    }
                    var color = GetColor(Cell.FromInnerCell(cell, false));

                    var border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        Background = color,
                        BorderThickness = new Thickness(0.5)
                    };
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, rowItemIndex);
                    grid.Children.Add(border);
                }
            }
        }

        private SolidColorBrush GetColor(Cell tetrominoCellType)
        {
            var color = new SolidColorBrush(
                Color.FromArgb(
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

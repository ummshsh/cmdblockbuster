using BlockBuster.Field;
using BlockBuster.Renderer;
using BlockBuster.State;
using BlockBuster.Tetrominoes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlockBusterXaml;

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

                        Border borderToSet;
                        if (cell.Ghost)
                        {
                            borderToSet = new Border
                            {
                                //BorderBrush = Brushes.White,
                                Background = Brushes.White,
                                BorderThickness = new Thickness(0.5)
                            };
                        }
                        else
                        {
                            borderToSet = new Border
                            {
                                BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#365178"),
                                Background = solidColorBrush,
                                BorderThickness = new Thickness(0.5)
                            };
                        }

                        Grid.SetRow(borderToSet, row);
                        Grid.SetColumn(borderToSet, rowItemIndex);
                        this.playfieldGrid.Children.Add(borderToSet);
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
        return tetrominoCellType.TetrominoCellType switch
        {
            TetrominoCellType.Empty => (SolidColorBrush)new BrushConverter().ConvertFrom("#000000"),
            TetrominoCellType.Red => (SolidColorBrush)new BrushConverter().ConvertFrom("#eb3158"),
            TetrominoCellType.Cyan => (SolidColorBrush)new BrushConverter().ConvertFrom("#59bab7"),
            TetrominoCellType.Purple => (SolidColorBrush)new BrushConverter().ConvertFrom("#5d528f"),
            TetrominoCellType.Green => (SolidColorBrush)new BrushConverter().ConvertFrom("#00894b"),
            TetrominoCellType.Yellow => (SolidColorBrush)new BrushConverter().ConvertFrom("#937500"),
            TetrominoCellType.Orange => (SolidColorBrush)new BrushConverter().ConvertFrom("#ca590e"),
            TetrominoCellType.Blue => (SolidColorBrush)new BrushConverter().ConvertFrom("#365178"),
            _ => throw new NotImplementedException(),
        };
    }

    private bool SequenceEquals<T>(T[,] a, T[,] b)
    {
        return a?.Rank == b?.Rank
        && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
        && a.Cast<T>().SequenceEqual(b.Cast<T>());
    }
}

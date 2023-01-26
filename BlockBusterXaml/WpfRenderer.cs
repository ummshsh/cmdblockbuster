using BlockBuster.Field;
using BlockBuster.Renderer;
using BlockBuster.State;
using BlockBuster.Tetrominoes;
using BlockBuster.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlockBusterXaml;

public class WpfRenderer : ITetrisRenderer
{
    private readonly Canvas canvasGrid;
    private readonly StackPanel stackLeft;
    private readonly StackPanel stackRight;
    private Cell[,] lastFieldUpdate;
    private readonly System.Windows.Shapes.Rectangle[,] rectangleFiledMapping;
    private readonly double canvasHeightStep;
    private readonly double canvasWidthStep;
    private Type cachedHoldMino;
    private Type[] cachedNextMino;
    private string cachedTextStats;

    public WpfRenderer(Canvas canvas, StackPanel stackLeft, StackPanel stackRight)
    {
        this.stackLeft = stackLeft;
        this.stackRight = stackRight;

        this.canvasGrid = canvas;
        canvasHeightStep = this.canvasGrid.ActualHeight / 22;
        canvasWidthStep = this.canvasGrid.ActualWidth / 10;

        this.lastFieldUpdate = new Cell[22, 10];
        this.rectangleFiledMapping = new System.Windows.Shapes.Rectangle[22, 10];

    }

    public void RenderPlayfield(object sender, VisiblePlayfield e)
    {
        DispatcherExtensions.BeginInvoke(Application.Current.Dispatcher, () =>
        {
            var height = e.Height;
            var width = e.Width;

            for (int row = 0; row < height; row++)
            {
                for (int rowItemIndex = 0; rowItemIndex < width; rowItemIndex++)
                {
                    if (lastFieldUpdate[row, rowItemIndex] is not null && lastFieldUpdate[row, rowItemIndex].Equals(e[row, rowItemIndex]))
                    {
                        continue; // Do not update cells that are already up to date
                    }

                    canvasGrid.Children.Remove(rectangleFiledMapping[row, rowItemIndex]);
                    rectangleFiledMapping[row, rowItemIndex] = null;

                    Cell cell = e[row, rowItemIndex];

                    System.Windows.Shapes.Rectangle rect;
                    if (cell.Ghost)
                    {
                        rect = new System.Windows.Shapes.Rectangle
                        {
                            Stroke = GetColor(cell),
                            Fill = Brushes.White,
                            Width = canvasWidthStep,
                            Height = canvasHeightStep,
                            StrokeThickness = cell.IsEmpty ? 0.2 : 1
                        };
                    }
                    else
                    {
                        rect = new System.Windows.Shapes.Rectangle
                        {
                            Stroke = (SolidColorBrush)new BrushConverter().ConvertFrom("#365178"),
                            Fill = GetColor(cell),
                            Width = canvasWidthStep,
                            Height = canvasHeightStep,
                            StrokeThickness = cell.IsEmpty ? 0.2 : 1
                        };
                    }

                    rectangleFiledMapping[row, rowItemIndex] = rect;

                    Canvas.SetLeft(rect, rowItemIndex * canvasWidthStep);
                    Canvas.SetTop(rect, row * canvasHeightStep);
                    canvasGrid.Children.Add(rect);
                }
            }

            lastFieldUpdate = (Cell[,])e.Cells.Clone();
        });
    }

    public void RenderGameState(object sender, GameState e)
    {
        DispatcherExtensions.BeginInvoke(Application.Current.Dispatcher, () =>
        {

            if (cachedHoldMino != e.Queue.HoldTetrominoType)
            {
                FillGridWithMinoImage((stackLeft.Children[1] as Border).Child as Grid, e.Queue.HoldTetrominoType);
                cachedHoldMino = e.Queue.HoldTetrominoType;
            }

            if (cachedTextStats != "" + e.Score + e.LinesCleared + e.Level)
            {
                var children = stackLeft.Children;
                (stackLeft.Children[2] as TextBlock).Text = "Score: " + e.Score;
                (stackLeft.Children[3] as TextBlock).Text = "Lines Cleared: " + e.LinesCleared;
                (stackLeft.Children[4] as TextBlock).Text = "Level: " + (int)e.Level;
                cachedTextStats = "" + e.Score + e.LinesCleared + e.Level;
            }

            if (cachedNextMino is null || !cachedNextMino.SequenceEqual(e.Queue.NextQueuePreview))
            {
                var array = e.Queue.NextQueuePreview.ToArray();
                FillGridWithMinoImage((stackRight.Children[1] as Border).Child as Grid, array[0]);
                FillGridWithMinoImage((stackRight.Children[2] as Border).Child as Grid, array[1]);
                FillGridWithMinoImage((stackRight.Children[3] as Border).Child as Grid, array[2]);
                FillGridWithMinoImage((stackRight.Children[4] as Border).Child as Grid, array[3]);
                FillGridWithMinoImage((stackRight.Children[5] as Border).Child as Grid, array[4]);
                cachedNextMino = e.Queue.NextQueuePreview.ToArray();
            }
        });
    }

    private void FillGridWithMinoImage(Grid grid, Type minoType)
    {
        if (minoType is null || grid is null)
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
}

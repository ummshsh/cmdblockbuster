using cmdblockbuster.Field;
using cmdblockbuster.Game;
using CMDblockbuster.Game;
using CMDblockbuster.InputController;
using CMDblockbuster.Renderer;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace BlockBuster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WpfInputHandler wpfInputHandler;
        public MainWindow()
        {
            InitializeComponent();
        }

        async void OnLoad(object sender, RoutedEventArgs e)
        {
            wpfInputHandler = new WpfInputHandler();
            var Tetris = new Tetris(wpfInputHandler, new WpfRenderer(this.PlayfieldGrid, this.DockStats));
            await Tetris.Start();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            CMDblockbuster.InputController.InputType inputToReturn;

            if (e.Key == Key.J | e.Key == Key.RightAlt)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.RotateLeft;
            }
            else if (e.Key == Key.K | e.Key == Key.RightCtrl)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.RotateRight;
            }
            else if (e.Key == Key.S | e.Key == Key.Down)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.SoftDrop;
            }
            else if (e.Key == Key.RightShift)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.Hold;
            }
            else if (e.Key == Key.W | e.Key == Key.Up)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.HardDrop;
            }
            else if (e.Key == Key.A | e.Key == Key.Left)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.Left;
            }
            else if (e.Key == Key.D | e.Key == Key.Right)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.Right;
            }
            else
            {
                inputToReturn = CMDblockbuster.InputController.InputType.None;
            }

            wpfInputHandler.InputFromWpf(inputToReturn);
        }
    }

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
                (dockStats.Children[2] as TextBlock).Text = "Level: " + e.Level;
                (dockStats.Children[3] as TextBlock).Text = "Next: " + e.Queue.NextTetromino.Name;
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

    public class WpfInputHandler : IInputHandler
    {
        public event EventHandler<CMDblockbuster.InputController.InputType> InputProvided;

        public void InputFromWpf(CMDblockbuster.InputController.InputType input)
        {
            InputProvided?.Invoke(this, input);
        }
    }
}

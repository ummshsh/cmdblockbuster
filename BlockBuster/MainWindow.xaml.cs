using CMDblockbuster;
using CMDblockbuster.Field;
using CMDblockbuster.Game;
using CMDblockbuster.InputController;
using CMDblockbuster.Renderer;
using CMDblockbuster.Tetrominoes;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            var Tetris = new Tetris(wpfInputHandler, new WpfRenderer(this.PlayfieldGrid));
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
        private Grid playfieldGrid;

        private CellType[,] lastUpdatedField;
        private bool DisplayFirstTime = true;

        public WpfRenderer(Grid playfieldGrid)
        {
            this.playfieldGrid = playfieldGrid;
            lastUpdatedField = new CellType[22, 10];
        }

        public void RenderPlayfield(object sender, Playfield e)
        {
            if (SequenceEquals(lastUpdatedField, e.field) && !DisplayFirstTime)
            {
                return; // Exit if playfield updated already
            }

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                this.playfieldGrid.Children.Clear();

                var height = e.field.GetLength(0);
                var width = e.field.GetLength(1);

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
                            TextBlock cell = new TextBlock();
                            ////cell.Text = $"{row}:{rowItemIndex}"; // index
                            cell.Background = new SolidColorBrush(e.field[row, rowItemIndex] > 0 ? Colors.Green : Colors.White);
                            Grid.SetRow(cell, row);
                            Grid.SetColumn(cell, rowItemIndex);
                            this.playfieldGrid.Children.Add(cell);
                        }
                    }
                }
            });

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

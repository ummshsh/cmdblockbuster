using CMDblockbuster.Field;
using CMDblockbuster.Game;
using CMDblockbuster.InputController;
using CMDblockbuster.Renderer;
using System;
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
            var Tetris = new Tetris();
            wpfInputHandler = new WpfInputHandler();
            await Tetris.Start(wpfInputHandler, new WpfRenderer(this.PlayfieldGrid));
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            CMDblockbuster.InputController.InputType inputToReturn;

            if (e.Key == Key.J | e.Key == Key.PageUp)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.RotateLeft;
            }
            else if (e.Key == Key.K | e.Key == Key.PageDown)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.RotateRight;
            }
            else if (e.Key == Key.S | e.Key == Key.Down)
            {
                inputToReturn = CMDblockbuster.InputController.InputType.Down;
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

        public WpfRenderer(Grid playfieldGrid)
        {
            this.playfieldGrid = playfieldGrid;
        }

        public void RenderPlayfield(object sender, Playfield e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                this.playfieldGrid.Children.Clear();

                var height = e.field.GetLength(0);
                var width = e.field.GetLength(1);

                for (int row = 0; row < height; row++)
                {
                    for (int rowItemIndex = 0; rowItemIndex < width; rowItemIndex++)
                    {
                        TextBlock cell = new TextBlock();
                        cell.Background = new SolidColorBrush(e.field[row, rowItemIndex] > 0 ? Colors.Green : Colors.White);
                        Grid.SetRow(cell, row);
                        Grid.SetColumn(cell, rowItemIndex);
                        this.playfieldGrid.Children.Add(cell);
                    }
                }
            });
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

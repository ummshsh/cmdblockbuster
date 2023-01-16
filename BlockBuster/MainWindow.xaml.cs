using CMDblockbuster.Game;
using System.Windows;
using System.Windows.Input;

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
            var Tetris = new Tetris(wpfInputHandler, new WpfRenderer(this.PlayfieldGrid, this.DockStats), new WpfSoundPlayer());
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
}

using BlockBuster.Game;
using BlockBusterXaml;
using System.Windows;
using System.Windows.Input;

namespace BlockBuster;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private WpfInputHandler wpfInputHandler;
    private Tetris tetris;
    private bool paused = false;
    private bool gameStarted = false;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void NewGame_Click(object sender, RoutedEventArgs e)
    {
        tetris?.Stop();
        wpfInputHandler = new WpfInputHandler();
        tetris = new Tetris(wpfInputHandler, new WpfRenderer(this.PlayfieldGrid, this.DockStats, this.HoldGrid, this.NextGrid), new WpfSoundPlayer());
        paused = false;
        gameStarted = true;
        MenuStack.Visibility = Visibility.Collapsed;
        await tetris.Start();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        tetris.Stop();
    }

    private void OnKeyDownHandler(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape & gameStarted)
        {
            paused = !paused;
            MenuStack.Visibility = paused ? Visibility.Visible : Visibility.Collapsed;
            Unpause.Visibility = paused ? Visibility.Visible : Visibility.Collapsed;
            GameOver.Visibility = paused ? Visibility.Collapsed : Visibility.Visible;
            tetris.Pause(paused);
            return;
        }

        if (MenuStack.Visibility == Visibility.Visible)
        {
            return;
        }

        BlockBuster.InputHandler.InputType inputToReturn;

        if (e.Key == Key.J | e.Key == Key.RightAlt)
        {
            inputToReturn = InputHandler.InputType.RotateLeft;
        }
        else if (e.Key == Key.K | e.Key == Key.RightCtrl)
        {
            inputToReturn = InputHandler.InputType.RotateRight;
        }
        else if (e.Key == Key.S | e.Key == Key.Down)
        {
            inputToReturn = InputHandler.InputType.SoftDrop;
        }
        else if (e.Key == Key.RightShift)
        {
            inputToReturn = InputHandler.InputType.Hold;
        }
        else if (e.Key == Key.W | e.Key == Key.Up)
        {
            inputToReturn = InputHandler.InputType.HardDrop;
        }
        else if (e.Key == Key.A | e.Key == Key.Left)
        {
            inputToReturn = InputHandler.InputType.Left;
        }
        else if (e.Key == Key.D | e.Key == Key.Right)
        {
            inputToReturn = InputHandler.InputType.Right;
        }
        else
        {
            inputToReturn = InputHandler.InputType.None;
        }

        wpfInputHandler.InputFromWpf(inputToReturn);
    }

    private void Canvas_Loaded(object sender, RoutedEventArgs e)
    {
        Unpause.Visibility = Visibility.Collapsed;
    }

    private void Unpause_Click(object sender, RoutedEventArgs e)
    {
        paused = !paused;
        MenuStack.Visibility = paused ? Visibility.Visible : Visibility.Collapsed;
        GameOver.Visibility = paused ? Visibility.Visible : Visibility.Collapsed;
        tetris.Pause(paused);
    }

    private void Window_Deactivated(object sender, System.EventArgs e)
    {
        ////if (gameStarted)
        ////{
        ////    paused = true;
        ////    MenuStack.Visibility = paused ? Visibility.Visible : Visibility.Collapsed;
        ////    Unpause.Visibility = paused ? Visibility.Visible : Visibility.Collapsed;
        ////    tetris.Pause(paused);
        ////}
    }
}

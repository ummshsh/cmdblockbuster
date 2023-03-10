using BlockBuster.Game;
using BlockBuster.Settings;
using BlockBusterXaml;
using System.Threading;
using System.Threading.Tasks;
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
        tetris = new Tetris(wpfInputHandler, new WpfRenderer(this.CanvasGrid, this.StackLeft, this.StackRight), new WpfSoundPlayer());
        paused = false;
        gameStarted = true;
        MenuStack.Visibility = Visibility.Collapsed;

        await tetris.Start();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => tetris.Stop();

    private void Canvas_Loaded(object sender, RoutedEventArgs e)
    {
        Unpause.Visibility = Visibility.Collapsed;

        if (Config.SkipMenuInXaml)
        {
            NewGame_Click(null, null);
        }
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

        InputHandler.InputType inputToReturn;

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
        else if (e.Key == Key.RightShift | e.Key == Key.LeftShift)
        {
            inputToReturn = InputHandler.InputType.Hold;
        }
        else if (e.Key == Key.W | e.Key == Key.Up | e.Key == Key.Space)
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

        wpfInputHandler.InputStarted(inputToReturn);
    }

    private void OnKeyUpHandler(object sender, KeyEventArgs e)
    {
        InputHandler.InputType inputToReturn;

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
        else if (e.Key == Key.RightShift | e.Key == Key.LeftShift)
        {
            inputToReturn = InputHandler.InputType.Hold;
        }
        else if (e.Key == Key.W | e.Key == Key.Up | e.Key == Key.Space)
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

        wpfInputHandler.InputEnded(inputToReturn);
    }
}

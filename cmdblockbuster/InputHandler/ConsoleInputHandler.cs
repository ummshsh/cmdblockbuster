using System;
using System.Threading;
using System.Threading.Tasks;

namespace CMDblockbuster.InputController
{
    public class ConsoleInputHandler : IInputHandler
    {
        public event EventHandler<InputType> InputProvided;

        public void ReadInput()
        {
            InputType inputToReturn;

            if (!Console.KeyAvailable)
            {
                inputToReturn = InputType.None;
            }

            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.J)
            {
                inputToReturn = InputType.RotateLeft;
            }
            else if (key.Key == ConsoleKey.K)
            {
                inputToReturn = InputType.RotateRight;
            }
            else if (key.Key == ConsoleKey.S)
            {
                inputToReturn = InputType.Down;
            }
            else if (key.Key == ConsoleKey.A)
            {
                inputToReturn = InputType.Left;
            }
            else if (key.Key == ConsoleKey.D)
            {
                inputToReturn = InputType.Right;
            }
            else
            {
                Console.WriteLine("unhandled:" + key.Key);
                inputToReturn = InputType.None;
            }

            InputProvided?.Invoke(this, inputToReturn);
        }

        public void BeginReadingInput(CancellationToken token)
        {
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    ReadInput();
                    ////await Task.Delay(TimeSpan.FromMilliseconds(100), token);
                }
            }, token);
        }
    }
}

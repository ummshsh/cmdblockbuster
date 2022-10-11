using cmdblockbuster.Field;
using System;
using System.Threading;

namespace cmdblockbuster.InputController
{
    public interface IInputHandler
    {
        public event EventHandler<InputType> InputProvided;

        void BeginReadingInput(CancellationToken token);
        public void ReadInput();
    }
}
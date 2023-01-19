using CMDblockbuster.InputController;
using System;

namespace BlockBuster
{
    public class WpfInputHandler : IInputHandler
    {
        public event EventHandler<InputType> InputProvided;

        public void InputFromWpf(InputType input)
        {
            InputProvided?.Invoke(this, input);
        }
    }
}

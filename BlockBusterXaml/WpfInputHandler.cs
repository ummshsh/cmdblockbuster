using BlockBusterXaml.InputHandler;
using System;

namespace BlockBusterXaml
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

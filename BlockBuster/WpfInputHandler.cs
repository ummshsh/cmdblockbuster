using CMDblockbuster.InputController;
using System;

namespace BlockBuster
{
    public class WpfInputHandler : IInputHandler
    {
        public event EventHandler<CMDblockbuster.InputController.InputType> InputProvided;

        public void InputFromWpf(CMDblockbuster.InputController.InputType input)
        {
            InputProvided?.Invoke(this, input);
        }
    }
}

using System;

namespace BlockBuster.InputHandler
{
    internal class ReplayInputHandler : IInputHandler
    {
        public event EventHandler<InputType> InputProvided;
    }
}
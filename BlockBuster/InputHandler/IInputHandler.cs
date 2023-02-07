using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BlockBuster.InputHandler
{
    public interface IInputHandler
    {
        public void InputStarted(InputType input);

        public void InputEnded(InputType input);

        public Dictionary<InputType, InputInfo> CurrentInputs { get; }
    }
}
using System.Collections.Concurrent;

namespace BlockBuster.InputHandler
{
    public interface IInputHandler
    {
        public void InputStarted(InputType input);

        public void InputEnded(InputType input);

        public ConcurrentBag<InputInfo> CurrentInputs { get; }
    }
}
using BlockBuster.InputHandler;
using System.Collections.Concurrent;
using System.Linq;

namespace BlockBusterXaml;

public class WpfInputHandler : IInputHandler
{
    public ConcurrentBag<InputInfo> CurrentInputs { get; } = new();

    public void InputStarted(InputType input)
    {
        var inputToActivate = CurrentInputs.FirstOrDefault(i => i.Type == input);

        if (inputToActivate == null)
        {
            CurrentInputs.Add(new InputInfo(input, true));
        }
        else
        {
            inputToActivate.Activated = true;
        }
    }

    public void InputEnded(InputType input)
    {
        var inputToDeactivate = CurrentInputs.FirstOrDefault(i => i.Type == input);

        if (inputToDeactivate == null)
        {
            CurrentInputs.Add(new InputInfo(input, false));
        }
        else
        {
            inputToDeactivate.Activated = false;
        }
    }
}

using BlockBuster.InputHandler;
using System;
using System.Collections.Generic;

namespace BlockBusterXaml;

public class WpfInputHandler : IInputHandler
{
    public Dictionary<InputType, InputInfo> CurrentInputs { get; }

    public WpfInputHandler()
    {
        CurrentInputs = new();
        foreach (var input in Enum.GetValues(typeof(InputType)))
        {
            CurrentInputs.Add((InputType)input, new InputInfo((InputType)input, false));
        }
    }

    public void InputStarted(InputType input)
    {
        var inputToActivate = CurrentInputs[input];
        inputToActivate.Activated = true;
    }

    public void InputEnded(InputType input)
    {
        var inputToDeactivate = CurrentInputs[input];
        inputToDeactivate.Activated = false;
    }
}

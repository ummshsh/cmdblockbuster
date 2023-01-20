using System;

namespace BlockBusterXaml.InputHandler;

public interface IInputHandler
{
    public event EventHandler<InputType> InputProvided;
}
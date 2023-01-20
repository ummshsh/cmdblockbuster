using System;

namespace BlockBuster.InputHandler;

public interface IInputHandler
{
    public event EventHandler<InputType> InputProvided;
}
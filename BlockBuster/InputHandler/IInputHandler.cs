using BlockBusterXaml.Field;
using System;
using System.Threading;

namespace BlockBusterXaml.InputHandler;

public interface IInputHandler
{
    public event EventHandler<InputType> InputProvided;
}
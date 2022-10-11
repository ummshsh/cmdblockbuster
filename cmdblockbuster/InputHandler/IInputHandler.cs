using cmdblockbuster.Field;
using System;

namespace cmdblockbuster.InputController
{
    public interface IInputHandler
    {
        public event EventHandler<InputType> InputProvided;

        public void ReadInput();
    }
}
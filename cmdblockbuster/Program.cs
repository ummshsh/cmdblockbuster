using CMDblockbuster.Game;
using CMDblockbuster.InputController;
using CMDblockbuster.Renderer;

namespace CMDblockbuster
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Tetris();
            game.Start(new ConsoleInputHandler(), new ConsoleRenderer());
        }
    }

    public abstract class Abstract
    {
        public abstract void MethodFromAbstract();
    }

    public class A : Abstract
    {
        public static void MethodStatic()
        {

        }

        public override void MethodFromAbstract()
        {
            throw new System.NotImplementedException();
        }
    }
}

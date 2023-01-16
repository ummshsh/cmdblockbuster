using cmdblockbuster.Game;
using CMDblockbuster.InputController;
using CMDblockbuster.Renderer;
using System.Threading.Tasks;

namespace CMDblockbuster.Game
{
    public class Tetris
    {
        internal SRSRulesEngine rulesEngine;
        internal ITetrisRenderer tetrisRenderer;
        internal IInputHandler inputController;

        public Tetris(IInputHandler inputController, ITetrisRenderer tetrisRenderer)
        {
            this.inputController = inputController;
            this.tetrisRenderer = tetrisRenderer;
        }

        public Task Start()
        {
            this.rulesEngine = new SRSRulesEngine(this.inputController);
            this.rulesEngine.PlayFieldUpdated += this.tetrisRenderer.RenderPlayfield;
            this.rulesEngine.GameStateUpdated += this.tetrisRenderer.RenderGameState;

            return rulesEngine.Start();
        }
    }
}

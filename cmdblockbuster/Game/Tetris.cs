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

        internal State GameState { get; set; } = State.Stopped;

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

            this.GameState = State.Running;
            return rulesEngine.Start();
        }

        private void GameOver() => this.GameState = State.GameOver;

        public void Stop() => this.GameState = State.Stopped;

        public void Pause() => this.GameState = State.Paused;
    }
}

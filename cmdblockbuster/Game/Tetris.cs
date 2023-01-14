using cmdblockbuster.Game;
using CMDblockbuster.Field;
using CMDblockbuster.InputController;
using CMDblockbuster.Renderer;
using System.Threading;
using System.Threading.Tasks;

namespace CMDblockbuster.Game
{
    public class Tetris
    {
        internal SRSRulesEngine rulesEngine;
        internal ITetrisRenderer tetrisRenderer;
        internal IInputHandler inputController;

        internal GameState GameState { get; set; } = GameState.Stopped;

        public Tetris(IInputHandler inputController, ITetrisRenderer tetrisRenderer)
        {
            this.inputController = inputController;
            this.tetrisRenderer = tetrisRenderer;
        }

        public Task Start()
        {
            this.rulesEngine = new SRSRulesEngine(this.inputController);
            this.rulesEngine.PlayFieldUpdated += this.tetrisRenderer.RenderPlayfield;

            this.GameState = GameState.Running;
            return rulesEngine.Start();
        }

        private void GameOver() => this.GameState = GameState.GameOver;

        public void Stop() => this.GameState = GameState.Stopped;

        public void Pause() => this.GameState = GameState.Paused;
    }
}

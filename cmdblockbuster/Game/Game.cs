using CMDblockbuster.Field;
using CMDblockbuster.InputController;
using CMDblockbuster.Renderer;
using CMDblockbuster.Rules;
using System;
using System.Threading;

namespace CMDblockbuster.Game
{
    public class Tetris
    {
        internal SRSRulesEngine rulesEngine;
        internal ITetrisRenderer tetrisRenderer;
        internal IInputHandler inputController;

        internal GameState GameState { get; set; } = GameState.Stopped;

        internal void Start(IInputHandler inputController, ITetrisRenderer tetrisRenderer)
        {
            this.inputController = inputController;
            var cts = new CancellationTokenSource();
            this.inputController.BeginReadingInput(cts.Token);
            this.tetrisRenderer = tetrisRenderer;

            this.rulesEngine = new SRSRulesEngine(this.inputController, new Playfield());
            this.rulesEngine.PlayFieldUpdated += this.tetrisRenderer.RenderPlayfield;

            this.GameState = GameState.Running;
            rulesEngine.Start();

            while(this.GameState == GameState.Running) // TODO: to replace with thread waiting with cancellation token
            {

            }
        }

        private void GameOver() => this.GameState = GameState.GameOver;

        public void Stop() => this.GameState = GameState.Stopped;

        public void Pause() => this.GameState = GameState.Paused;
    }
}

using cmdblockbuster.Game;
using cmdblockbuster.Score;
using System;

namespace cmdblockbuster.State
{
    public class GameState
    {
        public State State { get; set; } = State.Stopped;

        public int Score => ScoreCounter.Score;

        public int LinesCleared => ScoreCounter.LinesCleared;

        public Level Level => ScoreCounter.Level;

        public TetrominoQueue Queue { get; } = new TetrominoQueue();

        internal DateTime LastTimePlayfieldWasUpdated { get; set; } = DateTime.Now;

        internal DateTime LastTimeTetrominoMovedDown { get; set; } = DateTime.Now;

        internal DateTime TimeInfinityTriggered { get; set; } = DateTime.Now;

        internal bool ThisMinoInfinityAvailable { get; set; } = true;

        internal TimeSpan CurrentPerRowInterval => TimeSpan.FromSeconds(Math.Pow(0.8 - ((int)Level - 1) * 0.007, (int)Level - 1));

        internal bool CanUseHold { get; set; } = true;

        internal ScoreCounter ScoreCounter { get; } = new ScoreCounter();
    }
}

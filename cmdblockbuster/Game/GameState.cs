using System;

namespace cmdblockbuster.Game
{
    public enum State
    {
        Running,
        Paused,
        Stopped,
        GameOver
    }

    internal class GameState
    {
        public State State { get; set; } = State.Stopped;

        public DateTime LastTimePlayfieldWasUpdated { get; set; } = DateTime.Now;
        public DateTime LastTimeTetrominoMovedDown { get; set; } = DateTime.Now;

        public int Score { get; internal set; } = 0;

        /// <summary>
        /// 1-15
        /// </summary>
        public int Level { get; internal set; } = 1;

        public TimeSpan CurrentPerRowInterval => TimeSpan.FromSeconds(Math.Pow((0.8 - ((Level - 1) * 0.007)), Level - 1));
    }
}

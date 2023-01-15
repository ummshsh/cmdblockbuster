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

    public enum Level
    {
        Level_1 = 1,
        Level_2 = 2,
        Level_3 = 3,
        Level_4 = 4,
        Level_5 = 5,
        Level_6 = 6,
        Level_7 = 7,
        Level_8 = 8,
        Level_9 = 9,
        Level_10 = 10,
        Level_11 = 11,
        Level_12 = 12,
        Level_13 = 13,
        Level_14 = 14,
        Level_15 = 15
    }

    internal class GameState
    {
        public State State { get; set; } = State.Stopped;
        public DateTime LastTimePlayfieldWasUpdated { get; set; } = DateTime.Now;
        public DateTime LastTimeTetrominoMovedDown { get; set; } = DateTime.Now;

        public int Score { get; internal set; } = 0;

        public Level Level { get; internal set; } = Level.Level_1;

        public TimeSpan CurrentPerRowInterval => TimeSpan.FromSeconds(Math.Pow((0.8 - (((int)Level - 1) * 0.007)), (int)Level - 1));

        public bool CanUseHold { get; internal set; } = true;

        public TetrominoQueue Queue { get; } = new TetrominoQueue();
    }
}

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

        public int Score { get; internal set; } = 0;

        /// <summary>
        /// 1-15
        /// </summary>
        public int Level { get; internal set; } = 1;

        public GameState() { }
    }

}

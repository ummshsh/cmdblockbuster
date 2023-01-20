using BlockBuster.State;

namespace BlockBuster.Score;

// Increase score on soft drop
// T-Spin
// Tetris
// Back to back
// Perfect clear
internal class ScoreCounter
{
    public int Score { get; set; } = 0;

    public int LinesCleared { get; private set; } = 0;

    public Level Level => (Level)((LinesCleared / 10) + 1 > 15 ? 15 : (LinesCleared / 10) + 1);

    internal void AddLinesCleared(int count) => LinesCleared += count;

    internal void RegisterAction()
    {

    }
}

public enum ScoreAction // https://tetris.wiki/Scoring
{

}

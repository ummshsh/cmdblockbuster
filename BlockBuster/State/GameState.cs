using BlockBuster.Queue;
using BlockBuster.Score;
using System;
using BlockBuster.Common;

namespace BlockBuster.State;

public class GameState
{
    public EngineState State { get; set; } = EngineState.Stopped;

    public int Score => ScoreCounter.Score;

    public int LinesCleared => ScoreCounter.LinesCleared;

    public Level Level => ScoreCounter.Level;

    public TetrominoQueue Queue { get; } = new TetrominoQueue();

    internal DateTime LastTimePlayfieldWasUpdated { get; set; } = DateTime.Now;

    internal DateTime LastTimeTetrominoMovedDown { get; set; } = DateTime.Now;

    internal DateTime TimeInfinityTriggered { get; set; } = DateTime.Now;

    internal bool ThisMinoInfinityTriggered { get; set; } = true;

    internal TimeSpan CurrentPerRowInterval => TimeSpan.FromSeconds(Math.Pow(0.8 - ((int)Level - 1) * 0.007, (int)Level - 1));

    internal bool CanUseHold { get; set; } = true;

    public ScoreCounter ScoreCounter { get; } = new ScoreCounter();
}

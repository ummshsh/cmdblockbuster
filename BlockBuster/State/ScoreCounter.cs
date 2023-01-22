using BlockBuster.State;
using BlockBuster.Tetrominoes;
using BlockBuster.Utils;
using System;
using System.Linq;

namespace BlockBuster.Score;

/// <summary>
/// Impemented according to <see cref="https://tetris.wiki/Scoring"/>
/// </summary>
public class ScoreCounter
{
    public int Score { get; set; } = 0;

    public int LinesCleared { get; private set; } = 0;

    private readonly HistoryStack<ScoreablePlayfieldAction> actionsHistory = new(100);

    /// <summary>
    /// https://tetris.wiki/Combo
    /// </summary>
    private int ComboCounter { get; set; } = -1; // Default value

    /// <summary>
    /// Level is always considered to be the level before the line clear. 
    /// All level multipliers for <see cref="ScoreAction"/> and <see cref="ScoreActionWithHistory"/>
    /// </summary>
    public Level Level => (Level)((LinesCleared / 10) + 1 > 15 ? 15 : (LinesCleared / 10) + 1);

    internal void AddLinesCleared(int count) => LinesCleared += count;

    /// <summary>
    /// Registers multiple mino actions<para/>
    /// Populates <see cref="actionsHistory"/> with actions, and empties <see cref="actionsHistory"/> if breaker registered
    /// </summary>
    /// <param name="playfieldMinoAction"></param>
    internal void RegisterAction(ScoreablePlayfieldAction action)
    {
        if ((int)action.Action < 0)
        {
            // Ignore trivial move like love down, left, right as they not impact scoring for B2B and Combos
            // They can be used for futere(maybe) Finesse mode
            return;
        }
        else
        {
            actionsHistory.Push(action);
        }

        // Go back inhistory and check for Combo
        var indexHistoryForCombo = 0;
        while (true)
        {
            var scoreAction = actionsHistory.Peek(indexHistoryForCombo);

            if (scoreAction == null)
            {
                break;
            }

            if (scoreAction.LinesCleared > 0)
            {
                Score += GetScore(scoreAction.Action, Level, action.LinesCleared);
                ComboCounter++;
                indexHistoryForCombo++;
            }
            else
            {
                ComboCounter = -1;
                break;
            }
        }

        // Go back inhistory and check for BTB
        var indexHistoryForBtb = 0;
        while (true)
        {
            var scoreAction = actionsHistory.Peek(indexHistoryForBtb);

            if (scoreAction == null)
            {
                break;
            }

            if ((int)scoreAction.Action >= 100)
            {
                Score += GetScore(scoreAction.Action, Level, action.LinesCleared);
                indexHistoryForBtb++;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action">Action to reward</param>
    /// <param name="level">Level multiplier</param>
    /// <param name="linesDropped">Used to calculate hard and soft drops only</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private int GetScore(ScoreAction action, Level level, int linesDropped = 0)
    {
        return action switch
        {
            ScoreAction.None => 0,
            ScoreAction.Single => 100 * (int)level,
            ScoreAction.Double => 300 * (int)level,
            ScoreAction.Triple => 500 * (int)level,
            ScoreAction.Tetris => 800 * (int)level,
            ScoreAction.TSpinMiniNoLines => 100 * (int)level,
            ScoreAction.TSpinNoLines => 400 * (int)level,
            ScoreAction.TSpinMiniSingle => 200 * (int)level,
            ScoreAction.TSpinSingle => 800 * (int)level,
            ScoreAction.TSpinMiniDouble => 400 * (int)level,
            ScoreAction.TSpinDouble => 1200 * (int)level,
            ScoreAction.TSpinTriple => 1600 * (int)level,
            ScoreAction.SoftDrop => linesDropped * 1,
            ScoreAction.HardDrop => linesDropped * 2,
            ScoreAction.PerfectClearSingleLine => 800 * (int)level,
            ScoreAction.PerfectClearDoubleLine => 1200 * (int)level,
            ScoreAction.PerfectClearTripleLine => 1800 * (int)level,
            ScoreAction.PerfectClearTetris => 2000 * (int)level,
            ScoreAction.BackToBack => CalculateBackToBackScore(),
            ScoreAction.Combo => ComboCounter * 50 * (int)level,
            ScoreAction.BackToBackPerfectClearTetris => 3200 * (int)level,
            _ => throw new ArgumentException("Score not defined for action: " + action),
        };
    }

    private int CalculateBackToBackScore()
    {
        // Check that last action is either Tetris or TSpin
        if (!actionsHistory.Items.Last().IsDifficult)
        {
            return 0;
        }

        // Go back in history until found breaker(non difficult move) (ignoring soft\hard drops, moves to left\right)
        // and if found another difficult action, then take it's score and award by formula
        // Action score × 1.5
        var reversedActionsList = actionsHistory.Items.ReverseList();
        foreach (var item in reversedActionsList)
        {
            if (item.IsDifficult)
            {
                return (int)(item.ScoreAwarded * 1.5);
            }
            else if ((int)item.Action < 0)
            {
                continue;
            }
            else
            {
                return 0;
            }
        }

        return 0;
    }
}

public class ScoreablePlayfieldAction
{
    public Tetromino Tetromino { get; private set; }

    public ScoreAction Action { get; private set; } = ScoreAction.None;

    public bool IsDifficult => (int)Action >= 100;

    public int LinesCleared { get; set; } = 0;

    public TSpin TSpin { get; set; } = TSpin.Not;

    public int DroppedLines { get; set; } = 0; // Hard and Soft

    public int ScoreAwarded { get; set; } = 0;

    public ScoreablePlayfieldAction(Tetromino tetromino, ScoreAction playfieldActionsEnum)
    {
        this.Tetromino = tetromino;
        this.Action = playfieldActionsEnum;
    }
}

public enum TSpin
{
    Not,
    Mini,
    Full
}

/// <summary>
/// Also, This enum will be reported to outside world by event <para/>
/// Entries with value >= 100 are considered difficult and will impact scoring of back to backs <para/>
/// Entries with value < 0 is considered generic and and will not impact scoring of back to backs anc combos <para/>
/// </summary>
public enum ScoreAction
{
    // Simple moves that are not awarded
    MovedDown = -1,
    MovedLeft = -2,
    MovedRight = -3,
    RotatedLeft = -4,
    RotatedRight = -5,
    Landed = -6, // not sure if I need this one

    // Will be used to break combo counters:
    None = 0,

    // By cleared lines count:
    Single = 1,
    Double = 2,
    Triple = 3,
    Tetris = 100,

    // By lines cleared + T-Spin:
    TSpinMiniNoLines = 5,
    TSpinNoLines = 6,
    TSpinMiniSingle = 101,
    TSpinSingle = 102,
    TSpinMiniDouble = 103,
    TSpinDouble = 104,
    TSpinTriple = 105,

    // By single action, ignored by Combo and BTB counters:
    SoftDrop = 7,
    HardDrop = 8,

    // By playfield state:
    PerfectClearSingleLine = 106,
    PerfectClearDoubleLine = 107,
    PerfectClearTripleLine = 108,
    PerfectClearTetris = 109,

    /// <summary>
    /// By mino history, no value assigned, actions consists of actions above
    /// Back to back registered only if previous move was difficult and then another difficult move was made  <para/>
    /// Only a Single, Double, or Triple line clear can break a Back-to-Back chain, T-Spin with no lines will not break the chain. <para/>
    /// Regular mino landings(without clear) don't break BTB
    /// </summary>
    BackToBack,

    /// <summary>
    /// By mino history, no value assigned, actions consists of actions above <para/>
    /// Achived through any clears <para/>
    /// Breaked by landing without line clear
    /// </summary>
    Combo,

    /// <summary>
    /// By mino history, no value assigned, actions consists of actions above <para/>
    /// By several tetromino consecutive actions and playfield state
    /// </summary>
    BackToBackPerfectClearTetris
}

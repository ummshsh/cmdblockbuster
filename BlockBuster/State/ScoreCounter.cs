using BlockBuster.State;
using BlockBuster.Utils;
using System;
using System.Linq;

namespace BlockBuster.Score;

/// <summary>
/// Impemented according to <see cref="https://tetris.wiki/Scoring"/> <para/>
/// https://tetris.wiki/Combo
/// </summary>
public class ScoreCounter
{
    public int Score { get; set; } = 0;

    public int LinesCleared { get; private set; } = 0;

    private readonly HistoryStack<ScoreablePlayfieldAction> actionsHistory = new(100);

    public int ComboCounter { get; set; } = -1; // Default value

    /// <summary>
    /// Level is always considered to be the level before the line clear. 
    /// All level multipliers for <see cref="ScoreAction"/> and <see cref="ScoreActionWithHistory"/>
    /// </summary>
    public Level Level => (Level)((LinesCleared / 10) + 1 > 15 ? 15 : (LinesCleared / 10) + 1);

    /// <summary>
    /// Registers multiple mino actions<para/>
    /// Populates <see cref="actionsHistory"/> with actions, and empties <see cref="actionsHistory"/> if breaker registered
    /// </summary>
    /// <param name="playfieldMinoAction"></param>
    internal void RegisterAction(ScoreablePlayfieldAction action)
    {
        // Add action to history
        if ((int)action.Action < 0 && action.Action != ScoreAction.Landed)
        {
            // Ignore trivial move like love down, left, right as they not impact scoring for B2B and Combos
            return;
        }
        else
        {
            if (action.Action == ScoreAction.Landed)
            {
                ComboCounter = -1;
            }

            actionsHistory.Push(action);
            LinesCleared += action.LinesCleared;
        }

        // Go back inhistory and check for Combo
        var indexHistoryForCombo = 0;
        while (true)
        {
            var scoreActionFromHistory = actionsHistory.Peek(indexHistoryForCombo);

            if (scoreActionFromHistory == null)
            {
                break;
            }

            if (!scoreActionFromHistory.ScoreAddedAlready && scoreActionFromHistory.Action != ScoreAction.Landed)
            {
                scoreActionFromHistory.ScoreAddedAlready = true;
                Score += GetScore(scoreActionFromHistory.Action, Level, action.DroppedLines);

                if (scoreActionFromHistory.Action != ScoreAction.SoftDrop &&
                    scoreActionFromHistory.Action != ScoreAction.HardDrop)
                {
                    ComboCounter++;
                }
                indexHistoryForCombo++;
            }
            else
            {
                break;
            }
        }

        if (ComboCounter > 0)
        {
            Score += GetScore(ScoreAction.Combo, Level);
        }

        // Go back inhistory and check for BTB
        var indexHistoryForBtb = 0;
        var lastScoreActionFromHistory = actionsHistory.Peek(indexHistoryForBtb);

        if (lastScoreActionFromHistory != null && !lastScoreActionFromHistory.IsDifficult)
        {
            return;
        }

        indexHistoryForBtb++;

        while (true)
        {
            var scoreActionFromHistory = actionsHistory.Peek(indexHistoryForBtb);

            if (scoreActionFromHistory is null)
            {
                return;
            }

            // if not difficult and not breaks the BTB, then search again
            if (scoreActionFromHistory.LinesCleared > 0 && !scoreActionFromHistory.IsDifficult)
            {
                return; // Broken
            }
            else if (scoreActionFromHistory.IsDifficult)
            {
                Score += GetScore(actionsHistory.Peek(indexHistoryForBtb - 1).Action, Level, action.LinesCleared);
                return;
            }
            else
            {
                indexHistoryForBtb++; // Check next
                continue;
            }
        }
    }

    /// <summary>
    /// Returns score for action
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

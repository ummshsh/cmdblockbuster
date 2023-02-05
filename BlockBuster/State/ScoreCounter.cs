using BlockBuster.Settings;
using BlockBuster.State;
using System;
using System.Diagnostics;

namespace BlockBuster.Score
{
    /// <summary>
    /// Impemented according to <see cref="https://tetris.wiki/Scoring"/> <para/>
    /// https://tetris.wiki/Combo
    /// </summary>
    public class ScoreCounter
    {
        public double Score { get; set; } = 0;

        public int LinesCleared { get; private set; } = 0;

        private readonly HistoryStack<ScoreablePlayfieldAction> actionsHistory = new(900);

        private int comboCounter = -1;
        public int ComboCounter
        {
            get => comboCounter;
            set
            {
                comboCounter = value;
                Debug.WriteLineIf(Config.EnableDebugOutput, "Combo counter was set to:" + comboCounter);
            }
        }

        private int difficultMoveCounter = 0;
        public int DifficultMoveCounter
        {
            get => difficultMoveCounter;
            set
            {
                difficultMoveCounter = value;
                Debug.WriteLineIf(Config.EnableDebugOutput, "Difficult move counter was set to:" + difficultMoveCounter);
            }
        }

        /// <summary>
        /// Level is always considered to be the level before the line clear. 
        /// All level multipliers for <see cref="ScoreAction"/> and <see cref="ScoreActionWithHistory"/>
        /// </summary>
        public Level Level => (Level)((LinesCleared / 10) + 1 > 15 ? 15 : (LinesCleared / 10) + 1);

        private readonly HistoryStack<Tuple<string, string>> textualFeedback = new(10);
        public delegate void ScoreTextFeedbackHandler(HistoryStack<Tuple<string, string>> historyStack);
        public event ScoreTextFeedbackHandler ScoreTextFeedbackUpdated;

        /// <summary>
        /// Registers multiple mino actions<para/>
        /// Populates <see cref="actionsHistory"/> with actions, and empties <see cref="actionsHistory"/> if breaker registered
        /// </summary>
        /// <param name="playfieldMinoAction"></param>
        internal void RegisterAction(ScoreablePlayfieldAction action)
        {
            var levelBefroreRegisteringAction = Level;
            var btbAwardedAlready = false;

            if (action.Action == ScoreAction.None)
            {
                return;
            }

            // Add action to history
            actionsHistory.Push(action);
            LinesCleared += action.LinesCleared;
            if (action.Action == ScoreAction.Landed && action.LinesCleared == 0)
            {
                // If mino landed without line clear, break combo
                ComboCounter = -1;
            }

            var lastScoreActionFromHistory = actionsHistory.Peek(0);

            // Reset BTB counter if latest action is not difficult, but ignore all that do not clear lines
            // Ignore including T-Spin mini, they are not breaking BTB
            if (lastScoreActionFromHistory.LinesCleared > 0 && !lastScoreActionFromHistory.IsDifficult)
            {
                // Break BTB
                DifficultMoveCounter = 0;
            }
            else if (lastScoreActionFromHistory.IsDifficult)
            {
                // Increase BTB counter
                DifficultMoveCounter++;

                // Reward BTB
                if (DifficultMoveCounter > 1)
                {
                    var points = GetScore(ScoreAction.BackToBack, levelBefroreRegisteringAction, action.LinesCleared, lastScoreActionFromHistory.Action);
                    Score += points;
                    btbAwardedAlready = true;
                    textualFeedback.Push(new Tuple<string, string>($"BTB {ComboCounter}", points.ToString()));
                    ScoreTextFeedbackUpdated?.Invoke(textualFeedback);
                }
            }
            else
            {
                // Ignore action in BTB calculation
                Debug.WriteLineIf(
                    Config.EnableDebugOutput && lastScoreActionFromHistory.Action != ScoreAction.SoftDrop,
                    "Ignoring action in BTB calcualtion:" + lastScoreActionFromHistory.Action);
            }

            // Get score for just added move if it was not awarded by BTB
            if (!btbAwardedAlready)
            {
                var pointsLatestMove = GetScore(lastScoreActionFromHistory.Action, levelBefroreRegisteringAction, lastScoreActionFromHistory.DroppedLines);
                Score += pointsLatestMove;
                lastScoreActionFromHistory.ScoreAwarded = pointsLatestMove;
                textualFeedback.Push(new Tuple<string, string>(lastScoreActionFromHistory.Action.ToString(), pointsLatestMove.ToString()));
                ScoreTextFeedbackUpdated?.Invoke(textualFeedback);
            }

            // Increase Combo if lines were cleared
            if (lastScoreActionFromHistory.LinesCleared > 0)
            {
                ComboCounter++;
            }

            // Reward Combo
            if (ComboCounter > 0)
            {
                var points = GetScore(ScoreAction.Combo, levelBefroreRegisteringAction);
                Score += points;
                textualFeedback.Push(new Tuple<string, string>($"Combo {ComboCounter}", points.ToString()));
                ScoreTextFeedbackUpdated?.Invoke(textualFeedback);
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
        private double GetScore(ScoreAction action, Level level, int linesDropped = 0, ScoreAction actionToRewardInBtb = ScoreAction.None)
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
                ScoreAction.BackToBack => 1.5 * GetScore(actionToRewardInBtb, level),
                ScoreAction.Combo => ComboCounter * 50 * (int)level,
                ScoreAction.BackToBackPerfectClearTetris => 3200 * (int)level,
                _ => 0,
            };
        }
    }
}
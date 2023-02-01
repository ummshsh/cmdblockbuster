using BlockBuster.Tetrominoes;

namespace BlockBuster.Score
{

    public class ScoreablePlayfieldAction
    {
        public Tetromino Tetromino { get; private set; }

        public ScoreAction Action { get; private set; } = ScoreAction.None;

        public bool IsDifficult => (int)Action >= 100;

        public int LinesCleared { get; set; } = 0;

        public int DroppedLines { get; set; } = 0; // Hard and Soft

        public double ScoreAwarded { get; set; } = 0;

        public bool PerfectClear { get; set; } = false;

        public bool WithLastKick { get; set; } = false;

        public TSpin TSpin { get; set; } = TSpin.None;


        public ScoreablePlayfieldAction(Tetromino tetromino, ScoreAction playfieldActionsEnum)
        {
            this.Tetromino = tetromino;
            this.Action = playfieldActionsEnum;
        }
    }

    public enum TSpin
    {
        None,
        Mini,
        Normal
    }
}
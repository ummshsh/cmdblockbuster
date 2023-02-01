namespace BlockBuster.Score
{

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

        // Default value:
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

        // By mino history, no value assigned, actions consists of actions above:

        /// <summary>
        /// Back to back registered only if previous move was difficult and then another difficult move was made  <para/>
        /// Only a Single, Double, or Triple line clear can break a Back-to-Back chain, T-Spin with no lines will not break the chain. <para/>
        /// Regular mino landings(without clear) don't break BTB
        /// </summary>
        BackToBack,

        /// <summary>
        /// Achived through any clears <para/>
        /// Breaked by landing without line clear
        /// </summary>
        Combo,

        /// <summary>
        /// By several tetromino consecutive actions and playfield state
        /// </summary>
        BackToBackPerfectClearTetris
    }
}
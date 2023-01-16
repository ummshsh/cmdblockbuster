using System;

namespace cmdblockbuster.Game
{
    public static class Variables
    {
        /// <summary>
        /// Logic update interval
        /// </summary>
        public static TimeSpan TickRate { get; } = TimeSpan.FromMilliseconds(1);

        /// <summary>
        /// 60 updates per second
        /// </summary>
        public static TimeSpan RenderUpdateRate { get; } = TimeSpan.FromMilliseconds(16.7);

        /// <summary>
        /// Timeout to during which tetromino still can be moved if it is in motion even if it touched another tetromino or foundation bellow
        /// </summary>
        public static TimeSpan LockDelayTimeout = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// Time during which tetromino locks after rotation
        /// </summary>
        public static TimeSpan InfinityTime { get; set; } = TimeSpan.FromMilliseconds(1500);

        public static TimeSpan GetFallRate(int gameLevel)
        {
            // 15 levels of fall rate. Fall rates are expressed in G <para/>
            // 1G = 1 cell per frame <para/>
            // Values are true as long as RenderUpdateRate equals 16.7
            var FallRatesCurve = new[]
            {
                0.01667,
                0.021017,
                0.026977,
                0.035256,
                0.04693,
                0.06361,
                0.0879,
                0.1236,
                0.1775,
                0.2598,
                0.388,
                0.59,
                0.92,
                1.46,
                2.36
            };

            return TimeSpan.FromSeconds(FallRatesCurve[gameLevel]);
        }
    }
}

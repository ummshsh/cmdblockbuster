using System;

namespace cmdblockbuster.Game
{
    public static class TimeConstants
    {
        /// <summary>
        /// 60 updates per second
        /// </summary>
        public static TimeSpan TickRate { get; } = TimeSpan.FromMilliseconds(16.7);

        /// <summary>
        /// Timeout to during which tetromino still can be moved if it is in motion even if it touched another tetromino or foundation bellow
        /// </summary>
        public static TimeSpan LockTimeout = TimeSpan.FromMilliseconds(500);

        public static TimeSpan GetFallRate(int gameLevel)
        {
            return FallRatesCurve[gameLevel];
        }

        // 1 Cell per frame
        private static readonly TimeSpan[] FallRatesCurve = new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(1),
        };
    }
}

using BlockBuster.Field;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlockBuster.Settings
{
    public static class Config
    {
        // Gameplay Settings
        public static bool EnableWallKick => TryGetValueFromTextConfigOrSetDefalut("EnableWallKick", true);

        public static long DelayedAutoShiftRate => int.Parse(TryGetValueFromTextConfigOrSetDefalut("DelayedAutoShiftRate", "150"));

        // Debug Settings
        public static bool EnableDebugPlayfieldState => TryGetValueFromTextConfigOrSetDefalut("EnableDebugPlayfieldState", false);

        public static string DebugPlayfieldStateType => TryGetValueFromTextConfigOrSetDefalut("DebugPlayfieldStateType", "tspinsingle");

        public static bool SkipMenuInXaml => TryGetValueFromTextConfigOrSetDefalut("SkipMenuInXaml", false);

        public static bool EnableDebugOutput { get; internal set; } = true;

        #region Helpers

        private static Dictionary<string, string> _settings = null;
        private static Dictionary<string, string> Settings
        {
            get
            {
                _settings ??= ReadValuesFromConfig("Settings/config.txt");

                return _settings;
            }
        }

        internal static InnerPlayfield DebugPlayfieldState => DebugPlayfields.GetPlayfield(DebugPlayfieldStateType);


        private static bool TryGetValueFromTextConfigOrSetDefalut(string valueToGetFromConfig, bool defalutValue)
        {
            if (Settings.TryGetValue(valueToGetFromConfig, out string value))
            {
                return GetBoolFromString(value);
            }

            return defalutValue;
        }

        private static string TryGetValueFromTextConfigOrSetDefalut(string valueToGetFromConfig, string defalutValue)
        {
            if (Settings.TryGetValue(valueToGetFromConfig, out string value))
            {
                return value;
            }

            return defalutValue;
        }

        private static Dictionary<string, string> ReadValuesFromConfig(string configPath)
        {
            Dictionary<string, string> pairs = new();
            var lines = File.ReadAllLines(configPath);
            foreach (var line in lines)
            {
                if (!line.StartsWith("#") && line != string.Empty)
                {
                    string[] strings = line.Split('=');
                    pairs.Add(strings[0].Trim(), strings[1].Trim());
                }
            }

            return pairs;
        }

        private static bool GetBoolFromString(string input)
        {
            return input.Equals("Y", StringComparison.InvariantCultureIgnoreCase) ||
                input.Equals("true", StringComparison.InvariantCultureIgnoreCase) ||
                input.Equals("yes", StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
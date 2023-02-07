using System;

namespace BlockBuster.InputHandler
{
    public class InputInfo
    {
        public InputType Type { get; set; }

        private bool activated;

        public bool Activated
        {
            get
            {
                return activated;
            }
            set
            {
                if (activated != value)
                {
                    activated = value;
                    IsRepeat = false;
                    LastTimeTriggered = DateTime.Now;
                }
            }
        }

        public bool IsRepeat { get; set; }

        public DateTime LastTimeTriggered { get; set; }

        public InputInfo(InputType type, bool enabled, DateTime lastTimeTriggered)
        {
            Type = type;
            Activated = enabled;
            LastTimeTriggered = lastTimeTriggered;
        }

        public InputInfo(InputType type, bool enabled)
        {
            Type = type;
            Activated = enabled;
            LastTimeTriggered = DateTime.Now;
        }
    }
}

using DungeonJanitor.Core;
using UnityEngine;

namespace DungeonJanitor.Run
{
    /// <summary>
    /// Pure bookkeeping for one shift: the countdown and the mess tally.
    /// The run ends when time runs out or the last mess is cleaned.
    /// </summary>
    public sealed class RunState
    {
        public RunState(float duration)
        {
            Duration = Mathf.Max(0f, duration);
            TimeLeft = Duration;
        }

        public float Duration { get; }
        public float TimeLeft { get; private set; }
        public int Total { get; private set; }
        public int Cleaned { get; private set; }
        public int Remaining => Total - Cleaned;
        public bool Ended { get; private set; }
        public RunEndReason EndReason { get; private set; }

        public void AddMess()
        {
            if (!Ended)
                Total++;
        }

        /// <summary>Returns true if this was the last mess and the run just ended.</summary>
        public bool CleanMess()
        {
            if (Ended)
                return false;
            Cleaned = Mathf.Min(Cleaned + 1, Total);
            if (Total > 0 && Remaining == 0)
                return End(RunEndReason.AllClean);
            return false;
        }

        /// <summary>Counts down. Returns true if time just ran out.</summary>
        public bool Tick(float deltaTime)
        {
            if (Ended)
                return false;
            TimeLeft = Mathf.Max(0f, TimeLeft - Mathf.Max(0f, deltaTime));
            return TimeLeft <= 0f && End(RunEndReason.TimeUp);
        }

        bool End(RunEndReason reason)
        {
            Ended = true;
            EndReason = reason;
            return true;
        }
    }
}

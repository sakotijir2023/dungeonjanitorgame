using System.Collections.Generic;
using DungeonJanitor.Core;
using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Run
{
    /// <summary>
    /// Pure bookkeeping for one shift: the countdown and the mess tally.
    /// Only messes your current tools can handle count toward "all clean";
    /// tougher ones are tracked separately (the Inspector will notice them).
    /// The run ends when time runs out or the last cleanable mess is cleaned.
    /// </summary>
    public sealed class RunState
    {
        struct Entry
        {
            public MessFamily Family;
            public float Hardness;
            public bool Cleaned;
        }

        readonly List<Entry> messes = new List<Entry>();
        readonly Dictionary<MessFamily, float> capability = new Dictionary<MessFamily, float>();

        public RunState(float duration)
        {
            Duration = Mathf.Max(0f, duration);
            TimeLeft = Duration;
        }

        public float Duration { get; }
        public float TimeLeft { get; private set; }
        /// <summary>Messes your tools can handle.</summary>
        public int Total { get; private set; }
        /// <summary>Of those, how many are cleaned.</summary>
        public int Cleaned { get; private set; }
        /// <summary>Uncleaned messes beyond your tools.</summary>
        public int TooTough { get; private set; }
        public int Remaining => Total - Cleaned;
        public bool Ended { get; private set; }
        public RunEndReason EndReason { get; private set; }

        public bool CanClean(MessFamily family, float hardness) =>
            capability.TryGetValue(family, out float max) && hardness <= max;

        /// <summary>Replace what the tools can clean, and recount.</summary>
        public void SetCapabilities(IReadOnlyList<MessFamily> families, IReadOnlyList<float> maxHardness)
        {
            capability.Clear();
            for (int i = 0; i < families.Count && i < maxHardness.Count; i++)
                capability[families[i]] = maxHardness[i];
            Recount();
        }

        public void AddMess(MessFamily family, float hardness)
        {
            if (Ended)
                return;
            messes.Add(new Entry { Family = family, Hardness = hardness });
            Recount();
        }

        /// <summary>Returns true if this was the last cleanable mess and the run just ended.</summary>
        public bool CleanMess(MessFamily family, float hardness)
        {
            if (Ended)
                return false;
            int index = messes.FindIndex(e => !e.Cleaned && e.Family == family && Mathf.Approximately(e.Hardness, hardness));
            if (index < 0)
                return false;
            Entry entry = messes[index];
            entry.Cleaned = true;
            messes[index] = entry;
            Recount();
            return Total > 0 && Remaining == 0 && End(RunEndReason.AllClean);
        }

        /// <summary>Counts down. Returns true if time just ran out.</summary>
        public bool Tick(float deltaTime)
        {
            if (Ended)
                return false;
            TimeLeft = Mathf.Max(0f, TimeLeft - Mathf.Max(0f, deltaTime));
            return TimeLeft <= 0f && End(RunEndReason.TimeUp);
        }

        /// <summary>Families of the uncleaned messes that were too tough, one entry each.</summary>
        public MessFamily[] TooToughFamilies()
        {
            var families = new List<MessFamily>();
            foreach (Entry entry in messes)
            {
                if (!entry.Cleaned && !CanClean(entry.Family, entry.Hardness) && !families.Contains(entry.Family))
                    families.Add(entry.Family);
            }
            return families.ToArray();
        }

        void Recount()
        {
            Total = Cleaned = TooTough = 0;
            foreach (Entry entry in messes)
            {
                // A mess you managed to clean always counts, even if it was marginal.
                if (entry.Cleaned || CanClean(entry.Family, entry.Hardness))
                {
                    Total++;
                    if (entry.Cleaned)
                        Cleaned++;
                }
                else
                {
                    TooTough++;
                }
            }
        }

        bool End(RunEndReason reason)
        {
            Ended = true;
            EndReason = reason;
            return true;
        }
    }
}

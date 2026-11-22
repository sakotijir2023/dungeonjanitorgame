using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>Pure cleaning formulas shared by every tool. Upgrades just change the inputs.</summary>
    public static class CleaningMath
    {
        /// <summary>Hardness below this counts as this, so soft messes never divide by zero.</summary>
        public const float MinHardness = 0.01f;

        public static bool CanHandle(float hardness, float maxHardness) => hardness <= maxHardness;

        /// <summary>
        /// Progress per second (0..1 scale) toward cleaning or loosening a mess.
        /// A mess of hardness H takes H / cleaningSpeed seconds. Zero if the tool can't handle it.
        /// </summary>
        public static float ProgressRate(float cleaningSpeed, float hardness, float maxHardness)
        {
            if (!CanHandle(hardness, maxHardness))
                return 0f;
            return Mathf.Max(0f, cleaningSpeed) / Mathf.Max(hardness, MinHardness);
        }
    }
}

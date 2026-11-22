using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Scoring
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Scoring", fileName = "ScoringConfig")]
    public sealed class ScoringConfig : TuningConfig
    {
        [Header("Points")]
        [Tooltip("Points for cleaning any mess.")]
        [Range(0f, 100f)] public float pointsPerMess = 10f;
        [Tooltip("Extra points per point of hardness (tough messes are worth more).")]
        [Range(0f, 100f)] public float pointsPerHardness = 10f;
        [Tooltip("Bonus points per second left on the clock, only when every mess you could clean is clean.")]
        [Range(0f, 20f)] public float earlyBonusPerSecond = 1f;

        [Header("Grades (share of available points)")]
        [Tooltip("S also needs nothing left at all, too-tough messes included.")]
        [Range(0f, 1f)] public float gradeA = 0.9f;
        [Range(0f, 1f)] public float gradeB = 0.75f;
        [Range(0f, 1f)] public float gradeC = 0.55f;
        [Range(0f, 1f)] public float gradeD = 0.35f;

        [Header("Cash")]
        [Tooltip("Cash per point before the grade multiplier.")]
        [Range(0f, 5f)] public float cashPerPoint = 0.5f;
        [Range(0f, 5f)] public float cashMultiplierS = 1.5f;
        [Range(0f, 5f)] public float cashMultiplierA = 1.2f;
        [Range(0f, 5f)] public float cashMultiplierB = 1f;
        [Range(0f, 5f)] public float cashMultiplierC = 0.8f;
        [Range(0f, 5f)] public float cashMultiplierD = 0.6f;
        [Tooltip("Even a terrible shift pays something.")]
        [Range(0f, 5f)] public float cashMultiplierF = 0.4f;
    }
}

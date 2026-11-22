using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Tools.Vacuum
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Vacuum", fileName = "VacuumConfig")]
    public sealed class VacuumConfig : TuningConfig
    {
        [Header("Reach")]
        [Tooltip("How far the suction reaches from your eyes (m).")]
        [Range(1f, 15f)] public float range = 5f;
        [Tooltip("Half-angle of the suction cone (degrees). Bigger = less precise aiming needed.")]
        [Range(2f, 60f)] public float coneAngle = 22f;
        [Tooltip("How suction weakens with distance. 0 = full power everywhere, 1 = linear, 2+ = strong only up close.")]
        [Range(0f, 3f)] public float distanceFalloff = 0.8f;

        [Header("Pull")]
        [Tooltip("How hard loose goo is pulled toward the nozzle (m/s per second at full strength).")]
        [Range(5f, 200f)] public float pullAcceleration = 55f;
        [Tooltip("Kills sideways drift so goo flies straight at the nozzle instead of orbiting.")]
        [Range(0f, 30f)] public float lateralDamping = 8f;
        [Tooltip("Goo this close to the nozzle gets swallowed (m).")]
        [Range(0.1f, 2f)] public float swallowRadius = 0.6f;

        [Header("Power (upgrades)")]
        [Tooltip("Loosening speed. Goo of hardness H takes H / this seconds to break free at full strength.")]
        [Range(0.1f, 10f)] public float cleaningSpeed = 1f;
        [Tooltip("Hardest goo the vacuum can loosen at all. Harder goo just wobbles.")]
        [Range(0f, 10f)] public float maxHardness = 3f;

        [Header("Motor")]
        [Tooltip("Time to reach full suction after pressing Use (s).")]
        [Range(0f, 1f)] public float spinUpTime = 0.15f;
        [Tooltip("Time for suction to die off after releasing Use (s).")]
        [Range(0f, 1f)] public float spinDownTime = 0.25f;

        [Header("Feedback")]
        [Tooltip("How much the vacuum shakes in your hands while sucking (m).")]
        [Range(0f, 0.05f)] public float shakeAmount = 0.006f;
        [Tooltip("Shake speed (cycles per second).")]
        [Range(1f, 60f)] public float shakeFrequency = 28f;
        [Tooltip("How far the vacuum kicks back on each gulp (m).")]
        [Range(0f, 0.3f)] public float gulpKick = 0.06f;
        [Tooltip("Time for the gulp kick to settle (s).")]
        [Range(0.01f, 0.5f)] public float gulpRecoverTime = 0.08f;
        [Tooltip("Nozzle bulge on each gulp (scale multiplier).")]
        [Range(1f, 2f)] public float gulpBulge = 1.25f;
        [Tooltip("Air particles per second at full suction.")]
        [Range(0f, 300f)] public float particleRate = 80f;
    }
}

using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Tools.Mop
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Mop", fileName = "MopConfig")]
    public sealed class MopConfig : TuningConfig
    {
        [Header("Reach")]
        [Tooltip("How far away you can put the mop head on the floor (m).")]
        [Range(1f, 6f)] public float reach = 3.2f;
        [Tooltip("Mop head cleaning radius (m).")]
        [Range(0.1f, 1.2f)] public float headRadius = 0.35f;
        [Tooltip("How much the head trails your aim (s). Higher = heavier, draggier mop.")]
        [Range(0f, 0.3f)] public float headFollowTime = 0.07f;
        [Tooltip("Surfaces flatter than this count as floor (1 = perfectly flat only).")]
        [Range(0f, 1f)] public float minFloorFlatness = 0.6f;

        [Header("Scrubbing")]
        [Tooltip("Cleaning speed. A stain of hardness H takes about H / this seconds under the head while scrubbing hard.")]
        [Range(0.1f, 10f)] public float cleaningSpeed = 1f;
        [Tooltip("Hardest stain the mop can clean at all.")]
        [Range(0f, 10f)] public float maxHardness = 3f;
        [Tooltip("Cleaning while holding the mop still, as a fraction of full scrubbing.")]
        [Range(0f, 1f)] public float idleScrubFraction = 0.3f;
        [Tooltip("Head speed that counts as full-power scrubbing (m/s).")]
        [Range(0.2f, 8f)] public float speedForFullScrub = 2.5f;
        [Tooltip("Brush edge softness. Low = hard-edged wipe, high = soft, cleans mostly at the center.")]
        [Range(0.1f, 3f)] public float brushFalloff = 0.6f;

        [Header("Feedback")]
        [Tooltip("How far the head leans into its movement (degrees per m/s).")]
        [Range(0f, 30f)] public float leanPerSpeed = 8f;
        [Tooltip("Maximum head lean (degrees).")]
        [Range(0f, 60f)] public float maxLean = 25f;
        [Tooltip("Water droplets per second while scrubbing a stain at full power.")]
        [Range(0f, 200f)] public float splashRate = 45f;
        [Tooltip("Droplets in the burst when a stain is finished.")]
        [Range(0, 100)] public int cleanBurstCount = 30;
        [Tooltip("How far the hands kick when a stain is finished (m).")]
        [Range(0f, 0.2f)] public float cleanKick = 0.04f;
        [Tooltip("Time for the kick to settle (s).")]
        [Range(0.01f, 0.5f)] public float kickRecoverTime = 0.1f;
    }
}

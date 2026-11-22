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
        [Tooltip("How much the head trails its target (s). Higher = heavier, draggier mop.")]
        [Range(0f, 0.3f)] public float headFollowTime = 0.04f;
        [Tooltip("Surfaces flatter than this count as floor (1 = perfectly flat only).")]
        [Range(0f, 1f)] public float minFloorFlatness = 0.6f;

        [Header("Cleaning")]
        [Tooltip("Base cleaning speed. Sweep and stroke power multiply this. Upgrades raise it.")]
        [Range(0.1f, 10f)] public float cleaningSpeed = 1f;
        [Tooltip("Hardest stain the mop can clean at all.")]
        [Range(0f, 10f)] public float maxHardness = 3f;
        [Tooltip("Brush edge softness. Low = hard-edged wipe, high = soft, cleans mostly at the center.")]
        [Range(0.1f, 3f)] public float brushFalloff = 0.6f;

        [Header("Sweep (hold)")]
        [Tooltip("How far the head swings to each side of where you aim (m).")]
        [Range(0f, 2f)] public float sweepHalfWidth = 0.7f;
        [Tooltip("Full left-right-left swings per second.")]
        [Range(0.2f, 4f)] public float sweepFrequency = 1.4f;
        [Tooltip("How much the swing curves back toward you at the ends (m).")]
        [Range(0f, 0.6f)] public float sweepArcDepth = 0.15f;
        [Tooltip("Head cleaning radius while sweeping (m).")]
        [Range(0.1f, 1.2f)] public float sweepRadius = 0.4f;
        [Tooltip("Cleaning power while sweeping, times base cleaning speed.")]
        [Range(0f, 5f)] public float sweepPower = 0.8f;
        [Tooltip("Splash amount while sweeping, 0..1 of full splash.")]
        [Range(0f, 1f)] public float sweepSplash = 0.5f;

        [Header("Stroke (tap)")]
        [Tooltip("Length of one stroke, push out and back (s). Taps faster than this wait their turn.")]
        [Range(0.08f, 0.6f)] public float strokeTime = 0.22f;
        [Tooltip("How far a stroke pushes the head forward (m).")]
        [Range(0f, 1.2f)] public float strokeLength = 0.45f;
        [Tooltip("Strokes alternate left and right of straight ahead by this angle, scrubbing an X (degrees).")]
        [Range(0f, 60f)] public float strokeAlternateAngle = 20f;
        [Tooltip("Head cleaning radius during a stroke (m).")]
        [Range(0.1f, 1.2f)] public float strokeRadius = 0.3f;
        [Tooltip("Peak cleaning power of a stroke, times base cleaning speed.")]
        [Range(0f, 10f)] public float strokePower = 3.5f;
        [Tooltip("Extra taps remembered while a stroke is playing.")]
        [Range(0, 3)] public int strokeQueue = 1;

        [Header("Feedback")]
        [Tooltip("How far the head leans into its movement (degrees per m/s).")]
        [Range(0f, 30f)] public float leanPerSpeed = 8f;
        [Tooltip("Maximum head lean (degrees).")]
        [Range(0f, 60f)] public float maxLean = 25f;
        [Tooltip("Water droplets per second at full splash over a stain.")]
        [Range(0f, 200f)] public float splashRate = 45f;
        [Tooltip("Droplets thrown at the peak of each stroke.")]
        [Range(0, 60)] public int strokeBurstCount = 10;
        [Tooltip("How far the hands jolt at the peak of each stroke (m).")]
        [Range(0f, 0.2f)] public float strokeKick = 0.03f;
        [Tooltip("Droplets in the burst when a stain is finished.")]
        [Range(0, 100)] public int cleanBurstCount = 30;
        [Tooltip("How far the hands kick when a stain is finished (m).")]
        [Range(0f, 0.2f)] public float cleanKick = 0.04f;
        [Tooltip("Time for a kick to settle (s).")]
        [Range(0.01f, 0.5f)] public float kickRecoverTime = 0.1f;
    }
}

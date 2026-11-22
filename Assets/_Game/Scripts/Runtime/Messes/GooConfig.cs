using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Goo", fileName = "GooConfig")]
    public sealed class GooConfig : TuningConfig
    {
        [Header("Body")]
        [Tooltip("Goo blob mass (kg). Heavier goo resists the pull more.")]
        [Range(0.05f, 5f)] public float mass = 0.4f;
        [Tooltip("Air drag on loose goo.")]
        [Range(0f, 5f)] public float drag = 0.6f;

        [Header("Stuck to the floor")]
        [Tooltip("Stretch toward the nozzle when you first start sucking on stuck goo.")]
        [Range(0f, 1f)] public float tugStretchAtStart = 0.15f;
        [Tooltip("Stretch just before stuck goo breaks free.")]
        [Range(0f, 2f)] public float tugStretchAtBreak = 0.7f;
        [Tooltip("Size of the nervous wobble while being tugged.")]
        [Range(0f, 0.5f)] public float tugWobbleAmount = 0.08f;
        [Tooltip("Wobble speed (cycles per second).")]
        [Range(1f, 30f)] public float tugWobbleFrequency = 9f;
        [Tooltip("How fast loosening progress drains away when you stop sucking (per second).")]
        [Range(0f, 3f)] public float loosenDecay = 0.4f;
        [Tooltip("Speed of the pop toward the nozzle when goo breaks free (m/s).")]
        [Range(0f, 10f)] public float breakFreeImpulse = 2.5f;
        [Tooltip("Jiggle kick when goo breaks free.")]
        [Range(0f, 20f)] public float breakFreeJiggle = 6f;

        [Header("Squash and stretch")]
        [Tooltip("Stretch per m/s of flying speed.")]
        [Range(0f, 0.5f)] public float stretchPerSpeed = 0.07f;
        [Tooltip("Most a flying blob can stretch.")]
        [Range(0f, 2f)] public float maxStretch = 0.8f;
        [Tooltip("Jelly spring stiffness. Higher = faster, tighter wobble.")]
        [Range(10f, 600f)] public float springStiffness = 180f;
        [Tooltip("Jelly spring damping. Lower = wobbles for longer.")]
        [Range(0f, 40f)] public float springDamping = 9f;
        [Tooltip("Squash on impacts, per m/s of impact speed.")]
        [Range(0f, 2f)] public float impactJiggle = 0.6f;

        [Header("Swallow")]
        [Tooltip("Time for goo to shrink into the nozzle (s).")]
        [Range(0.01f, 0.5f)] public float swallowTime = 0.1f;

        [Header("Look")]
        [Tooltip("Hardness at which goo shows its darkest color.")]
        [Range(0.1f, 10f)] public float hardnessForDarkest = 3f;
        public Color softColor = new Color(0.45f, 0.9f, 0.35f);
        public Color hardColor = new Color(0.12f, 0.35f, 0.15f);
    }
}

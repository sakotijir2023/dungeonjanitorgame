using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Inspection
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Inspector", fileName = "InspectorConfig")]
    public sealed class InspectorConfig : TuningConfig
    {
        [Header("Report card reveal (real seconds)")]
        [Tooltip("Time for the report card to fade in.")]
        [Range(0f, 1.5f)] public float cardFadeTime = 0.3f;
        [Tooltip("Pause before the grade stamps down.")]
        [Range(0f, 3f)] public float gradeDelay = 0.7f;
        [Tooltip("How long the stamp takes to land.")]
        [Range(0.02f, 1f)] public float stampTime = 0.18f;
        [Tooltip("Grade size at the start of the stamp (1 = final size).")]
        [Range(1f, 6f)] public float stampStartScale = 3f;
        [Tooltip("Pause after the stamp before the Inspector starts writing.")]
        [Range(0f, 3f)] public float remarkDelay = 0.5f;
        [Tooltip("Letters per second as the remark is written out (0 = instantly).")]
        [Range(0f, 200f)] public float typeSpeed = 55f;
        [Tooltip("Pause after the remark before the hint appears.")]
        [Range(0f, 3f)] public float hintDelay = 0.4f;
    }
}

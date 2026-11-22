using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Hud
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Hints", fileName = "HintConfig")]
    public sealed class HintConfig : TuningConfig
    {
        [Header("Too tough popup")]
        [Tooltip("Seconds of working a too-tough mess before the popup appears. Stops it flashing when you just brush past.")]
        [Range(0f, 2f)] public float toughShowDelay = 0.35f;
        [Tooltip("A gap in contact shorter than this (s) still counts as 'still trying'.")]
        [Range(0.02f, 0.5f)] public float toughContactGrace = 0.15f;
        [Tooltip("Seconds the popup stays after you stop.")]
        [Range(0f, 5f)] public float toughHoldTime = 1.2f;
        [Tooltip("Seconds to fade in and out.")]
        [Range(0.01f, 1f)] public float toughFadeTime = 0.25f;
    }
}

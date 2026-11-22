using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Run
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Run", fileName = "RunConfig")]
    public sealed class RunConfig : TuningConfig
    {
        [Header("Shift")]
        [Tooltip("Length of a shift in seconds (takes effect on the next run; F5 restarts).")]
        [Range(30f, 600f)] public float runDuration = 240f;
        [Tooltip("The clock turns to warning colours when this many seconds are left.")]
        [Range(0f, 120f)] public float warningTime = 30f;
        [Tooltip("Stop the clock while the tuning panel is open (handy for testing).")]
        public bool pauseWhileTuning = true;
    }
}

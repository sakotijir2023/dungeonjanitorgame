using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Core
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Slow Motion", fileName = "SlowMotionConfig")]
    public sealed class SlowMotionConfig : TuningConfig
    {
        [Header("Tool wheel")]
        [Tooltip("Game speed while the tool wheel is open. 1 = normal, 0.2 = one fifth speed.")]
        [Range(0.02f, 1f)] public float wheelTimeScale = 0.2f;
        [Tooltip("Real seconds to ease into slow-motion.")]
        [Range(0f, 0.5f)] public float easeInTime = 0.08f;
        [Tooltip("Real seconds to ease back to normal speed.")]
        [Range(0f, 0.5f)] public float easeOutTime = 0.15f;
        [Tooltip("Run physics steps more often in slow-motion, so flying goo stays smooth instead of stuttering.")]
        public bool smoothPhysics = true;
    }
}

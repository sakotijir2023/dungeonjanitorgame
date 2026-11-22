using DungeonJanitor.Core;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>
    /// Anything that needs cleaning. Hardness decides how long a tool takes and
    /// whether the tool can handle it at all.
    /// </summary>
    public sealed class Mess : MonoBehaviour
    {
        [SerializeField] MessFamily family;
        [SerializeField, Min(0f)] float hardness;

        bool cleaned;

        public MessFamily Family => family;
        public float Hardness => hardness;
        public bool IsCleaned => cleaned;

        /// <summary>Set by the spawner right after Instantiate, before Start.</summary>
        public void Configure(float newHardness)
        {
            hardness = Mathf.Max(0f, newHardness);
        }

        void Start()
        {
            EventBus<MessSpawned>.Raise(new MessSpawned { Family = family, Hardness = hardness });
        }

        public void Clean()
        {
            if (cleaned)
                return;
            cleaned = true;
            EventBus<MessCleaned>.Raise(new MessCleaned { Family = family, Hardness = hardness, Position = transform.position });
        }
    }
}

using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Tools.Mop
{
    /// <summary>One frame of mopping as seen by a stain.</summary>
    public readonly struct ScrubContext
    {
        public readonly IScrubSource Source;
        public readonly Vector3 Position;
        public readonly float Radius;
        /// <summary>Cleaning speed already scaled by how vigorously you are scrubbing.</summary>
        public readonly float CleaningSpeed;
        public readonly float MaxHardness;
        public readonly float BrushFalloff;
        public readonly float DeltaTime;

        public ScrubContext(IScrubSource source, Vector3 position, float radius, float cleaningSpeed,
            float maxHardness, float brushFalloff, float deltaTime)
        {
            Source = source;
            Position = position;
            Radius = radius;
            CleaningSpeed = cleaningSpeed;
            MaxHardness = maxHardness;
            BrushFalloff = brushFalloff;
            DeltaTime = deltaTime;
        }
    }

    /// <summary>Something the mop can wipe.</summary>
    public interface IScrubTarget
    {
        void Scrub(in ScrubContext context);
    }

    /// <summary>The mop; told when a stain is finished.</summary>
    public interface IScrubSource
    {
        void NotifyCleaned(Mess mess, Vector3 position);
    }
}

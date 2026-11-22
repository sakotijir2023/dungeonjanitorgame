using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Core
{
    /// <summary>A mess appeared in the room.</summary>
    public struct MessSpawned
    {
        public MessFamily Family;
        public float Hardness;
    }

    /// <summary>A mess was fully cleaned up.</summary>
    public struct MessCleaned
    {
        public MessFamily Family;
        public float Hardness;
        public Vector3 Position;
    }

    /// <summary>A stuck mess was loosened enough to break free.</summary>
    public struct MessBrokeFree
    {
        public MessFamily Family;
        public Vector3 Position;
    }

    /// <summary>The player switched tools.</summary>
    public struct ToolEquipped
    {
        public string ToolId;
    }

    /// <summary>The vacuum started or stopped sucking (for audio and effects).</summary>
    public struct VacuumSuctionChanged
    {
        public bool Active;
    }

    /// <summary>Pressed the tuning panel toggle (F1 / gamepad View button).</summary>
    public struct TuningPanelToggleRequested { }

    /// <summary>The tuning panel opened or closed.</summary>
    public struct TuningPanelVisibilityChanged
    {
        public bool Visible;
    }

    /// <summary>The player left the ground by jumping.</summary>
    public struct PlayerJumped { }

    /// <summary>The player touched down. FallSpeed is the downward speed at impact (m/s).</summary>
    public struct PlayerLanded
    {
        public float FallSpeed;
    }
}

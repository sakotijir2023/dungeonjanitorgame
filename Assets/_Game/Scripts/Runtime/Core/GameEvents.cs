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

    /// <summary>A tool is working on a mess it can't handle (raised every frame of contact).</summary>
    public struct TooToughTouched
    {
        public string ToolName;
        public MessFamily Family;
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

    /// <summary>The tool wheel opened or closed (drives slow-motion and freezes look).</summary>
    public struct ToolWheelVisibilityChanged
    {
        public bool Visible;
    }

    /// <summary>Something (the wheel) asks for a tool by its definition id.</summary>
    public struct ToolSelectRequested
    {
        public string ToolId;
    }

    /// <summary>The tools the player actually owns right now, by definition id.</summary>
    public struct ToolsAvailable
    {
        public string[] ToolIds;
    }

    /// <summary>
    /// What the player's tools can clean: for each mess family, the hardest
    /// mess any owned tool handles. Families not listed can't be cleaned at all.
    /// </summary>
    public struct CleaningCapabilities
    {
        public MessFamily[] Families;
        public float[] MaxHardness;
    }

    /// <summary>Why a run ended.</summary>
    public enum RunEndReason
    {
        TimeUp,
        AllClean,
    }

    /// <summary>Raised every frame while a run is going, for the HUD.</summary>
    public struct RunStatusChanged
    {
        public float TimeLeft;
        public float Duration;
        /// <summary>Cleaned messes your tools can handle.</summary>
        public int Cleaned;
        /// <summary>All messes your tools can handle.</summary>
        public int Total;
        /// <summary>Messes still too tough for your current tools.</summary>
        public int TooTough;
    }

    /// <summary>The run is over. TimeLeft above zero means an early finish.</summary>
    public struct RunEnded
    {
        public RunEndReason Reason;
        public float TimeLeft;
        public float Duration;
        public int Cleaned;
        public int Total;
        /// <summary>Messes left behind because your tools couldn't handle them.</summary>
        public int TooTough;
        /// <summary>Families of those messes (one entry per family), for the Inspector's hints.</summary>
        public MessFamily[] TooToughFamilies;
        /// <summary>Hardness of each mess cleaned (for scoring).</summary>
        public float[] CleanedHardness;
        /// <summary>Hardness of each mess you could have cleaned but didn't.</summary>
        public float[] MissedHardness;
    }

    /// <summary>Pressed the restart key (F5 / Start).</summary>
    public struct RestartRunRequested { }

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

namespace DungeonJanitor.Core
{
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

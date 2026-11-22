using DungeonJanitor.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonJanitor.Run
{
    /// <summary>
    /// Runs one shift: counts messes as they spawn and get cleaned (only those
    /// the player's tools can handle count toward "all clean"), counts the
    /// clock down (game time, so slow-motion slows it too), and announces the
    /// end. Restart reloads the scene.
    /// </summary>
    public sealed class RunController : MonoBehaviour
    {
        [SerializeField] RunConfig config;

        RunState state;
        bool tuningOpen;

        void Awake() => state = new RunState(config.runDuration);

        void OnEnable()
        {
            EventBus<MessSpawned>.Subscribe(OnMessSpawned);
            EventBus<MessCleaned>.Subscribe(OnMessCleaned);
            EventBus<CleaningCapabilities>.Subscribe(OnCapabilities);
            EventBus<TuningPanelVisibilityChanged>.Subscribe(OnTuningPanel);
            EventBus<RestartRunRequested>.Subscribe(OnRestart);
        }

        void OnDisable()
        {
            EventBus<MessSpawned>.Unsubscribe(OnMessSpawned);
            EventBus<MessCleaned>.Unsubscribe(OnMessCleaned);
            EventBus<CleaningCapabilities>.Unsubscribe(OnCapabilities);
            EventBus<TuningPanelVisibilityChanged>.Unsubscribe(OnTuningPanel);
            EventBus<RestartRunRequested>.Unsubscribe(OnRestart);
        }

        void OnMessSpawned(MessSpawned evt) => state.AddMess(evt.Family, evt.Hardness);

        void OnMessCleaned(MessCleaned evt)
        {
            if (state.CleanMess(evt.Family, evt.Hardness))
                AnnounceEnd();
        }

        void OnCapabilities(CleaningCapabilities evt) => state.SetCapabilities(evt.Families, evt.MaxHardness);

        void OnTuningPanel(TuningPanelVisibilityChanged evt) => tuningOpen = evt.Visible;

        void OnRestart(RestartRunRequested _) => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        void Update()
        {
            if (state.Ended)
                return;
            bool paused = tuningOpen && config.pauseWhileTuning;
            if (!paused && state.Tick(Time.deltaTime))
            {
                AnnounceEnd();
                return;
            }
            EventBus<RunStatusChanged>.Raise(new RunStatusChanged
            {
                TimeLeft = state.TimeLeft, Duration = state.Duration, Cleaned = state.Cleaned, Total = state.Total,
                TooTough = state.TooTough,
            });
        }

        void AnnounceEnd()
        {
            EventBus<RunEnded>.Raise(new RunEnded
            {
                Reason = state.EndReason, TimeLeft = state.TimeLeft, Duration = state.Duration,
                Cleaned = state.Cleaned, Total = state.Total,
                TooTough = state.TooTough, TooToughFamilies = state.TooToughFamilies(),
                CleanedHardness = state.CleanedHardness(), MissedHardness = state.MissedHardness(),
            });
        }
    }
}

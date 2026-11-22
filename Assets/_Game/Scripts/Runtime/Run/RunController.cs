using DungeonJanitor.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DungeonJanitor.Run
{
    /// <summary>
    /// Runs one shift: counts messes as they spawn and get cleaned, counts the
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
            EventBus<TuningPanelVisibilityChanged>.Subscribe(OnTuningPanel);
            EventBus<RestartRunRequested>.Subscribe(OnRestart);
        }

        void OnDisable()
        {
            EventBus<MessSpawned>.Unsubscribe(OnMessSpawned);
            EventBus<MessCleaned>.Unsubscribe(OnMessCleaned);
            EventBus<TuningPanelVisibilityChanged>.Unsubscribe(OnTuningPanel);
            EventBus<RestartRunRequested>.Unsubscribe(OnRestart);
        }

        void OnMessSpawned(MessSpawned _) => state.AddMess();

        void OnMessCleaned(MessCleaned _)
        {
            if (state.CleanMess())
                AnnounceEnd();
        }

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
            });
        }

        void AnnounceEnd()
        {
            EventBus<RunEnded>.Raise(new RunEnded
            {
                Reason = state.EndReason, TimeLeft = state.TimeLeft, Duration = state.Duration,
                Cleaned = state.Cleaned, Total = state.Total,
            });
        }
    }
}

using DungeonJanitor.Core;
using UnityEngine;

namespace DungeonJanitor.Scoring
{
    /// <summary>Scores the shift when it ends and announces the result (RunScored).</summary>
    public sealed class ScoreKeeper : MonoBehaviour
    {
        [SerializeField] ScoringConfig config;

        void OnEnable() => EventBus<RunEnded>.Subscribe(OnRunEnded);
        void OnDisable() => EventBus<RunEnded>.Unsubscribe(OnRunEnded);

        void OnRunEnded(RunEnded run)
        {
            ScoreResult score = ScoreCalculator.Score(
                run.CleanedHardness ?? System.Array.Empty<float>(),
                run.MissedHardness ?? System.Array.Empty<float>(),
                run.TooTough, run.Reason, run.TimeLeft, config);
            EventBus<RunScored>.Raise(new RunScored { Run = run, Score = score });
        }
    }
}

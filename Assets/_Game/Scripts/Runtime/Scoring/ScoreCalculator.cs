using System.Collections.Generic;
using DungeonJanitor.Core;
using UnityEngine;

namespace DungeonJanitor.Scoring
{
    public enum Grade { S, A, B, C, D, F }

    /// <summary>What a shift was worth.</summary>
    public readonly struct ScoreResult
    {
        public readonly float Points;
        public readonly float PossiblePoints;
        public readonly float EarlyBonus;
        /// <summary>0..1 share of available points earned (drives the grade).</summary>
        public readonly float Share;
        public readonly Grade Grade;
        public readonly int Cash;

        public ScoreResult(float points, float possible, float earlyBonus, float share, Grade grade, int cash)
        {
            Points = points;
            PossiblePoints = possible;
            EarlyBonus = earlyBonus;
            Share = share;
            Grade = grade;
            Cash = cash;
        }

        public float Total => Points + EarlyBonus;
    }

    /// <summary>A shift was scored. Raised right after RunEnded.</summary>
    public struct RunScored
    {
        public RunEnded Run;
        public ScoreResult Score;
    }

    /// <summary>Pure scoring math: points, grade and cash for one shift.</summary>
    public static class ScoreCalculator
    {
        public static float PointsFor(float hardness, ScoringConfig config) =>
            config.pointsPerMess + Mathf.Max(0f, hardness) * config.pointsPerHardness;

        /// <param name="cleanedHardness">Hardness of every mess you cleaned.</param>
        /// <param name="missedHardness">Hardness of every mess you could have cleaned but didn't.</param>
        /// <param name="tooTough">Messes left because your tools couldn't handle them.</param>
        public static ScoreResult Score(IReadOnlyList<float> cleanedHardness, IReadOnlyList<float> missedHardness,
            int tooTough, RunEndReason reason, float timeLeft, ScoringConfig config)
        {
            float points = Sum(cleanedHardness, config);
            float possible = points + Sum(missedHardness, config);
            float share = possible > 0f ? points / possible : 0f;

            bool allYouCould = reason == RunEndReason.AllClean;
            float earlyBonus = allYouCould ? Mathf.Max(0f, timeLeft) * config.earlyBonusPerSecond : 0f;

            bool perfect = possible > 0f && missedHardness.Count == 0 && tooTough == 0;
            Grade grade = GradeFor(share, perfect, config);
            int cash = Mathf.RoundToInt((points + earlyBonus) * config.cashPerPoint * CashMultiplier(grade, config));
            return new ScoreResult(points, possible, earlyBonus, share, grade, cash);
        }

        public static Grade GradeFor(float share, bool perfect, ScoringConfig config)
        {
            if (perfect)
                return Grade.S;
            if (share >= config.gradeA)
                return Grade.A;
            if (share >= config.gradeB)
                return Grade.B;
            if (share >= config.gradeC)
                return Grade.C;
            if (share >= config.gradeD)
                return Grade.D;
            return Grade.F;
        }

        public static float CashMultiplier(Grade grade, ScoringConfig config)
        {
            switch (grade)
            {
                case Grade.S: return config.cashMultiplierS;
                case Grade.A: return config.cashMultiplierA;
                case Grade.B: return config.cashMultiplierB;
                case Grade.C: return config.cashMultiplierC;
                case Grade.D: return config.cashMultiplierD;
                default: return config.cashMultiplierF;
            }
        }

        static float Sum(IReadOnlyList<float> hardness, ScoringConfig config)
        {
            float total = 0f;
            foreach (float h in hardness)
                total += PointsFor(h, config);
            return total;
        }
    }
}

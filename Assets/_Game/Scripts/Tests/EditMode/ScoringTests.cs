using DungeonJanitor.Core;
using DungeonJanitor.Scoring;
using NUnit.Framework;
using UnityEngine;

namespace DungeonJanitor.Tests
{
    public sealed class ScoringTests
    {
        const float Tolerance = 1e-3f;
        static readonly float[] None = new float[0];

        ScoringConfig config;

        [SetUp]
        public void SetUp()
        {
            config = ScriptableObject.CreateInstance<ScoringConfig>();
            config.pointsPerMess = 10f;
            config.pointsPerHardness = 10f;
            config.earlyBonusPerSecond = 1f;
            config.cashPerPoint = 0.5f;
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(config);

        [Test]
        public void ToughMesses_AreWorthMore()
        {
            Assert.That(ScoreCalculator.PointsFor(0f, config), Is.EqualTo(10f).Within(Tolerance));
            Assert.That(ScoreCalculator.PointsFor(3f, config), Is.EqualTo(40f).Within(Tolerance));
        }

        [Test]
        public void CleaningEverything_WithNothingTooTough_IsS_WithEarlyBonus()
        {
            ScoreResult score = ScoreCalculator.Score(new[] { 0f, 1f }, None, 0, RunEndReason.AllClean, 30f, config);
            Assert.That(score.Grade, Is.EqualTo(Grade.S));
            Assert.That(score.Points, Is.EqualTo(30f).Within(Tolerance));
            Assert.That(score.EarlyBonus, Is.EqualTo(30f).Within(Tolerance));
            Assert.That(score.Cash, Is.EqualTo(Mathf.RoundToInt(60f * 0.5f * config.cashMultiplierS)));
        }

        [Test]
        public void TooToughLeftBehind_CapsAtA()
        {
            ScoreResult score = ScoreCalculator.Score(new[] { 0f, 1f }, None, 2, RunEndReason.AllClean, 30f, config);
            Assert.That(score.Share, Is.EqualTo(1f).Within(Tolerance));
            Assert.That(score.Grade, Is.EqualTo(Grade.A), "Nothing is perfect while too-tough messes remain.");
            Assert.That(score.EarlyBonus, Is.GreaterThan(0f), "Still earns the early finish bonus.");
        }

        [Test]
        public void TimeUp_GivesNoEarlyBonus()
        {
            ScoreResult score = ScoreCalculator.Score(new[] { 1f }, new[] { 1f }, 0, RunEndReason.TimeUp, 0f, config);
            Assert.That(score.EarlyBonus, Is.EqualTo(0f));
            Assert.That(score.Share, Is.EqualTo(0.5f).Within(Tolerance));
            Assert.That(score.Grade, Is.EqualTo(Grade.D));
        }

        [Test]
        public void GradeThresholds()
        {
            Assert.That(ScoreCalculator.GradeFor(0.95f, false, config), Is.EqualTo(Grade.A));
            Assert.That(ScoreCalculator.GradeFor(0.8f, false, config), Is.EqualTo(Grade.B));
            Assert.That(ScoreCalculator.GradeFor(0.6f, false, config), Is.EqualTo(Grade.C));
            Assert.That(ScoreCalculator.GradeFor(0.4f, false, config), Is.EqualTo(Grade.D));
            Assert.That(ScoreCalculator.GradeFor(0.1f, false, config), Is.EqualTo(Grade.F));
        }

        [Test]
        public void TerribleShift_StillPaysSomething()
        {
            ScoreResult score = ScoreCalculator.Score(new[] { 0f }, new[] { 2f, 2f, 2f }, 0, RunEndReason.TimeUp, 0f, config);
            Assert.That(score.Grade, Is.EqualTo(Grade.F));
            Assert.That(score.Cash, Is.GreaterThan(0));
        }

        [Test]
        public void NothingCleaned_ScoresZeroWithoutErrors()
        {
            ScoreResult score = ScoreCalculator.Score(None, None, 2, RunEndReason.TimeUp, 0f, config);
            Assert.That(score.Total, Is.EqualTo(0f));
            Assert.That(score.Grade, Is.EqualTo(Grade.F));
            Assert.That(score.Cash, Is.EqualTo(0));
        }
    }
}

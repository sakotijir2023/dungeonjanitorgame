using System.Collections.Generic;
using DungeonJanitor.Messes;
using DungeonJanitor.Tools.Vacuum;
using NUnit.Framework;

namespace DungeonJanitor.Tests
{
    public sealed class CleaningTests
    {
        const float Tolerance = 1e-4f;

        [Test]
        public void ProgressRate_TakesHardnessOverSpeedSeconds()
        {
            float rate = CleaningMath.ProgressRate(cleaningSpeed: 2f, hardness: 3f, maxHardness: 5f);
            Assert.That(1f / rate, Is.EqualTo(1.5f).Within(Tolerance));
        }

        [Test]
        public void ProgressRate_IsZeroWhenTooHard()
        {
            Assert.That(CleaningMath.ProgressRate(1f, hardness: 5f, maxHardness: 3f), Is.EqualTo(0f));
        }

        [Test]
        public void ProgressRate_AtExactlyMaxHardnessStillWorks()
        {
            Assert.That(CleaningMath.ProgressRate(1f, hardness: 3f, maxHardness: 3f), Is.GreaterThan(0f));
        }

        [Test]
        public void ProgressRate_SoftMessIsFastButFinite()
        {
            float rate = CleaningMath.ProgressRate(1f, hardness: 0f, maxHardness: 3f);
            Assert.That(rate, Is.GreaterThan(1f));
            Assert.That(float.IsInfinity(rate), Is.False);
        }

        [Test]
        public void DistanceFalloff_FullAtNozzleZeroAtRange()
        {
            Assert.That(SuctionMath.DistanceFalloff(0f, 5f, 1f), Is.EqualTo(1f).Within(Tolerance));
            Assert.That(SuctionMath.DistanceFalloff(5f, 5f, 1f), Is.EqualTo(0f));
            Assert.That(SuctionMath.DistanceFalloff(2.5f, 5f, 1f), Is.EqualTo(0.5f).Within(Tolerance));
        }

        [Test]
        public void DistanceFalloff_ZeroExponentMeansNoFalloff()
        {
            Assert.That(SuctionMath.DistanceFalloff(4.9f, 5f, 0f), Is.EqualTo(1f).Within(Tolerance));
        }

        [Test]
        public void SeededLayout_SameSeedSameRoom()
        {
            var choices = new List<int> { 1, 2, 3, 1, 1, 2, 1, 1 };
            var a = MessLayoutPlanner.PlanSeeded(choices, 42, 0.6f);
            var b = MessLayoutPlanner.PlanSeeded(choices, 42, 0.6f);
            Assert.That(a, Is.EqualTo(b));
        }

        [Test]
        public void SeededLayout_FillChanceExtremes()
        {
            var choices = new List<int> { 1, 1, 1, 1, 1 };
            Assert.That(MessLayoutPlanner.PlanSeeded(choices, 7, 1f).Count, Is.EqualTo(5));
            Assert.That(MessLayoutPlanner.PlanSeeded(choices, 7, 0f).Count, Is.EqualTo(0));
        }

        [Test]
        public void SeededLayout_SkipsPointsWithNoChoicesAndStaysInRange()
        {
            var choices = new List<int> { 0, 3, 0, 2 };
            foreach (PlannedSpawn spawn in MessLayoutPlanner.PlanSeeded(choices, 3, 1f))
            {
                Assert.That(choices[spawn.PointIndex], Is.GreaterThan(0));
                Assert.That(spawn.ChoiceIndex, Is.LessThan(choices[spawn.PointIndex]));
                Assert.That(spawn.HardnessRoll, Is.InRange(0f, 1f));
            }
        }
    }
}

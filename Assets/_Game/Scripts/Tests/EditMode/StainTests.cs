using DungeonJanitor.Messes;
using DungeonJanitor.Tools.Mop;
using NUnit.Framework;
using UnityEngine;

namespace DungeonJanitor.Tests
{
    public sealed class StainTests
    {
        const int Resolution = 32;
        const float Tolerance = 1e-4f;

        static float[] Solid(float value = 1f)
        {
            var coverage = new float[Resolution * Resolution];
            for (int i = 0; i < coverage.Length; i++)
                coverage[i] = value;
            return coverage;
        }

        [Test]
        public void Scrub_RemovesDirtOnlyUnderTheBrush()
        {
            var mask = new StainMask(Resolution, Solid());
            mask.Scrub(new Vector2(0.25f, 0.25f), 0.1f, 1f, 1f);

            Assert.That(mask[8, 8], Is.LessThan(1f), "under the brush");
            Assert.That(mask[24, 24], Is.EqualTo(1f), "far from the brush");
            Assert.That(mask.RemainingFraction, Is.LessThan(1f).And.GreaterThan(0.9f));
        }

        [Test]
        public void Scrub_NeverGoesBelowZero()
        {
            var mask = new StainMask(Resolution, Solid());
            for (int i = 0; i < 50; i++)
                mask.Scrub(new Vector2(0.5f, 0.5f), 0.2f, 10f, 0.1f);
            Assert.That(mask[16, 16], Is.EqualTo(0f));
            Assert.That(mask.RemainingFraction, Is.GreaterThanOrEqualTo(0f));
        }

        [Test]
        public void Scrub_WholeStainReachesZeroRemaining()
        {
            var mask = new StainMask(Resolution, Solid(0.5f));
            mask.Scrub(new Vector2(0.5f, 0.5f), 2f, 10f, 0.1f);
            Assert.That(mask.RemainingFraction, Is.EqualTo(0f).Within(Tolerance));
        }

        [Test]
        public void Scrub_ZeroAmountDoesNothing()
        {
            var mask = new StainMask(Resolution, Solid());
            Assert.That(mask.Scrub(new Vector2(0.5f, 0.5f), 0.3f, 0f, 1f), Is.EqualTo(0f));
            Assert.That(mask.RemainingFraction, Is.EqualTo(1f));
        }

        [Test]
        public void Shape_IsDeterministicAndDirtyInTheMiddleCleanAtCorners()
        {
            float[] a = StainShape.Generate(Resolution, 5, 0.36f, 0.1f, 0.05f, 0.35f);
            float[] b = StainShape.Generate(Resolution, 5, 0.36f, 0.1f, 0.05f, 0.35f);
            Assert.That(a, Is.EqualTo(b));
            Assert.That(a[(Resolution / 2) * Resolution + Resolution / 2], Is.GreaterThan(0.2f), "center");
            Assert.That(a[0], Is.EqualTo(0f), "corner");
        }
    }
}

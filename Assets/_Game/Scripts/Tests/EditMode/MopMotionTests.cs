using DungeonJanitor.Tools.Mop;
using NUnit.Framework;
using UnityEngine;

namespace DungeonJanitor.Tests
{
    public sealed class MopMotionTests
    {
        const float Frame = 0.01f;
        const float Tolerance = 1e-4f;

        MopConfig config;
        MopMotion motion;

        [SetUp]
        public void SetUp()
        {
            config = ScriptableObject.CreateInstance<MopConfig>();
            config.strokeTime = 0.2f;
            config.strokeQueue = 1;
            motion = new MopMotion(config);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(config);

        /// <summary>Runs frames; returns how many strokes peaked.</summary>
        int Run(bool held, float seconds)
        {
            int peaks = 0;
            for (float t = 0f; t < seconds - Frame * 0.5f; t += Frame)
            {
                motion.Tick(held, Frame);
                if (motion.StrokePeakedThisTick)
                    peaks++;
            }
            return peaks;
        }

        [Test]
        public void Tap_PlaysOneStrokeThenRests()
        {
            motion.Press();
            Assert.That(motion.Current, Is.EqualTo(MopMotion.Mode.Stroke));
            int peaks = Run(false, 0.5f);
            Assert.That(peaks, Is.EqualTo(1));
            Assert.That(motion.Current, Is.EqualTo(MopMotion.Mode.Idle));
            Assert.That(motion.Power, Is.EqualTo(0f));
        }

        [Test]
        public void Hold_StrokesFirstThenSweeps()
        {
            motion.Press();
            Run(true, 0.1f);
            Assert.That(motion.Current, Is.EqualTo(MopMotion.Mode.Stroke), "Pressing always strokes at once.");
            Run(true, 0.3f);
            Assert.That(motion.Current, Is.EqualTo(MopMotion.Mode.Sweep));
            Assert.That(motion.Power, Is.EqualTo(config.sweepPower).Within(Tolerance));

            Run(false, Frame);
            Assert.That(motion.Current, Is.EqualTo(MopMotion.Mode.Idle), "Letting go stops sweeping.");
        }

        [Test]
        public void Spamming_IsCappedByStrokeTime()
        {
            // Ten taps inside one stroke only add one queued stroke.
            motion.Press();
            for (int i = 0; i < 10; i++)
                motion.Press();
            int peaks = Run(false, 1f);
            Assert.That(peaks, Is.EqualTo(2));
        }

        [Test]
        public void StrokesAlternateSides()
        {
            motion.Press();
            Run(false, 0.1f);
            float firstSide = Mathf.Sign(motion.Offset.x);
            Run(false, 0.2f);
            motion.Press();
            Run(false, 0.1f);
            Assert.That(Mathf.Sign(motion.Offset.x), Is.EqualTo(-firstSide));
            Assert.That(motion.Offset.y, Is.GreaterThan(0f), "Strokes push forward.");
        }

        [Test]
        public void SweepOffset_SwingsSideToSideAndCurvesBack()
        {
            Assert.That(MopMotion.SweepOffset(0f, 0.7f, 0.15f).magnitude, Is.EqualTo(0f).Within(Tolerance));
            Vector2 right = MopMotion.SweepOffset(Mathf.PI * 0.5f, 0.7f, 0.15f);
            Vector2 left = MopMotion.SweepOffset(Mathf.PI * 1.5f, 0.7f, 0.15f);
            Assert.That(right.x, Is.EqualTo(0.7f).Within(Tolerance));
            Assert.That(left.x, Is.EqualTo(-0.7f).Within(Tolerance));
            Assert.That(right.y, Is.EqualTo(-0.15f).Within(Tolerance));
        }

        [Test]
        public void StrokeEnvelope_PeaksHalfway()
        {
            Assert.That(MopMotion.StrokeEnvelope(0f), Is.EqualTo(0f).Within(Tolerance));
            Assert.That(MopMotion.StrokeEnvelope(0.5f), Is.EqualTo(1f).Within(Tolerance));
            Assert.That(MopMotion.StrokeEnvelope(1f), Is.EqualTo(0f).Within(Tolerance));
        }
    }
}

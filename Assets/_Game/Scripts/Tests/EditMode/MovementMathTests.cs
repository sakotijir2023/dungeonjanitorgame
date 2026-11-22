using DungeonJanitor.Player;
using NUnit.Framework;

namespace DungeonJanitor.Tests
{
    public sealed class MovementMathTests
    {
        const float Tolerance = 1e-4f;

        [Test]
        public void JumpVelocity_ReachesRequestedHeight()
        {
            const float gravity = 25f;
            const float height = 1.2f;
            float v = MovementMath.JumpVelocity(gravity, height);
            float peak = v * v / (2f * gravity);
            Assert.That(peak, Is.EqualTo(height).Within(Tolerance));
        }

        [Test]
        public void JumpVelocity_IsZeroForNonPositiveInputs()
        {
            Assert.That(MovementMath.JumpVelocity(-5f, 1f), Is.EqualTo(0f));
            Assert.That(MovementMath.JumpVelocity(25f, -1f), Is.EqualTo(0f));
        }

        [Test]
        public void ResponseCurve_KeepsSignAndEndpoints()
        {
            Assert.That(MovementMath.ApplyResponseCurve(-1f, 2f), Is.EqualTo(-1f).Within(Tolerance));
            Assert.That(MovementMath.ApplyResponseCurve(0.5f, 2f), Is.EqualTo(0.25f).Within(Tolerance));
            Assert.That(MovementMath.ApplyResponseCurve(-0.5f, 2f), Is.EqualTo(-0.25f).Within(Tolerance));
        }

        [Test]
        public void SmoothingBlend_ZeroTimeIsInstant()
        {
            Assert.That(MovementMath.SmoothingBlend(0f, 0.016f), Is.EqualTo(1f));
        }

        [Test]
        public void SmoothingBlend_IsBetweenZeroAndOne()
        {
            float blend = MovementMath.SmoothingBlend(0.05f, 0.016f);
            Assert.That(blend, Is.GreaterThan(0f).And.LessThan(1f));
        }
    }
}

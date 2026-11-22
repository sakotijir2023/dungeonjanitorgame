using UnityEngine;

namespace DungeonJanitor.Player
{
    /// <summary>Pure movement formulas, kept separate so they can be unit tested.</summary>
    public static class MovementMath
    {
        /// <summary>Launch speed needed to reach a given peak height under constant gravity.</summary>
        public static float JumpVelocity(float gravity, float jumpHeight)
        {
            return Mathf.Sqrt(2f * Mathf.Max(0f, gravity) * Mathf.Max(0f, jumpHeight));
        }

        /// <summary>Shapes a stick axis: keeps the sign, raises the magnitude to a power.</summary>
        public static float ApplyResponseCurve(float axis, float exponent)
        {
            return Mathf.Sign(axis) * Mathf.Pow(Mathf.Abs(axis), exponent);
        }

        /// <summary>Frame-rate independent blend factor for exponential smoothing.</summary>
        public static float SmoothingBlend(float smoothingTime, float deltaTime)
        {
            if (smoothingTime <= 0f)
                return 1f;
            return 1f - Mathf.Exp(-deltaTime / smoothingTime);
        }
    }
}

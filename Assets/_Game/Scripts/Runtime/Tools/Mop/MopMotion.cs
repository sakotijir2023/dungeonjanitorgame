using UnityEngine;

namespace DungeonJanitor.Tools.Mop
{
    /// <summary>
    /// The mop's two techniques, as pure logic. Every press plays a quick
    /// stroke at once; taps during a stroke wait their turn (up to the queue
    /// size), so spamming never beats the stroke rhythm. Keep holding after a
    /// stroke and the mop starts sweeping side to side until you let go.
    /// Offsets are in metres around the aim point: x = right, y = forward.
    /// </summary>
    public sealed class MopMotion
    {
        const float TwoPi = Mathf.PI * 2f;

        public enum Mode { Idle, Stroke, Sweep }

        readonly MopConfig config;
        float strokeElapsed;
        int queued;
        int strokeCount;
        float sweepPhase;

        public MopMotion(MopConfig config) => this.config = config;

        public Mode Current { get; private set; }
        /// <summary>Head offset from the aim point (x right, y forward), metres.</summary>
        public Vector2 Offset { get; private set; }
        /// <summary>Cleaning power right now, times the base cleaning speed.</summary>
        public float Power { get; private set; }
        /// <summary>Cleaning radius right now (m).</summary>
        public float Radius { get; private set; }
        /// <summary>0..1 how much water should fly (for effects).</summary>
        public float Splash { get; private set; }
        /// <summary>True on the one frame a stroke reaches full reach.</summary>
        public bool StrokePeakedThisTick { get; private set; }

        public void Reset()
        {
            Current = Mode.Idle;
            queued = 0;
            strokeElapsed = 0f;
            Offset = Vector2.zero;
            Power = Splash = 0f;
            StrokePeakedThisTick = false;
        }

        /// <summary>The button went down.</summary>
        public void Press()
        {
            if (Current == Mode.Stroke)
                queued = Mathf.Min(queued + 1, config.strokeQueue);
            else
                StartStroke();
        }

        void StartStroke()
        {
            Current = Mode.Stroke;
            strokeElapsed = 0f;
            strokeCount++;
        }

        public void Tick(bool held, float deltaTime)
        {
            StrokePeakedThisTick = false;
            switch (Current)
            {
                case Mode.Stroke:
                    TickStroke(held, deltaTime);
                    break;
                case Mode.Sweep:
                    if (held)
                        TickSweep(deltaTime);
                    else
                        Reset();
                    break;
                default:
                    Offset = Vector2.zero;
                    Power = Splash = 0f;
                    break;
            }
        }

        void TickStroke(bool held, float deltaTime)
        {
            float duration = Mathf.Max(config.strokeTime, Mathf.Epsilon);
            float before = strokeElapsed / duration;
            strokeElapsed += deltaTime;
            float t = strokeElapsed / duration;
            if (before < 0.5f && t >= 0.5f)
                StrokePeakedThisTick = true;

            if (t >= 1f)
            {
                if (queued > 0)
                {
                    queued--;
                    StartStroke();
                    t = 0f;
                }
                else if (held)
                {
                    Current = Mode.Sweep;
                    sweepPhase = 0f;
                    TickSweep(0f);
                    return;
                }
                else
                {
                    Reset();
                    return;
                }
            }

            float envelope = StrokeEnvelope(t);
            float side = (strokeCount % 2 == 0 ? 1f : -1f) * config.strokeAlternateAngle * Mathf.Deg2Rad;
            Offset = new Vector2(Mathf.Sin(side), Mathf.Cos(side)) * (envelope * config.strokeLength);
            Power = config.strokePower * envelope;
            Radius = config.strokeRadius;
            Splash = envelope;
        }

        void TickSweep(float deltaTime)
        {
            sweepPhase = Mathf.Repeat(sweepPhase + TwoPi * config.sweepFrequency * deltaTime, TwoPi);
            Offset = SweepOffset(sweepPhase, config.sweepHalfWidth, config.sweepArcDepth);
            Power = config.sweepPower;
            Radius = config.sweepRadius;
            Splash = config.sweepSplash;
        }

        /// <summary>Out-and-back push: 0 at the start and end, 1 at the halfway peak.</summary>
        public static float StrokeEnvelope(float t) => Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);

        /// <summary>Side-to-side swing that curves back toward you at each end.</summary>
        public static Vector2 SweepOffset(float phase, float halfWidth, float arcDepth)
        {
            float side = Mathf.Sin(phase);
            return new Vector2(side * halfWidth, -side * side * arcDepth);
        }
    }
}

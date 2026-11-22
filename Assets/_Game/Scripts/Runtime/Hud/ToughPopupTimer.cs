using UnityEngine;

namespace DungeonJanitor.Hud
{
    /// <summary>
    /// When to show the "too tough" popup, as pure timing logic. Contact must
    /// last showDelay before it appears; it stays holdTime after contact stops,
    /// and fades in and out over fadeTime.
    /// </summary>
    public sealed class ToughPopupTimer
    {
        float contactStart = float.NegativeInfinity;
        float lastContact = float.NegativeInfinity;
        float alpha;

        public float Alpha => alpha;

        /// <summary>Contact with a too-tough mess at time <paramref name="now"/>.</summary>
        public void Touch(float now, float contactGrace)
        {
            if (now - lastContact > contactGrace)
                contactStart = now;
            lastContact = now;
        }

        /// <summary>Advance; returns the popup's opacity 0..1.</summary>
        public float Update(float now, float deltaTime, HintConfig config)
        {
            bool trying = now - lastContact <= config.toughContactGrace;
            bool earned = lastContact - contactStart >= config.toughShowDelay;
            bool lingering = now - lastContact <= config.toughHoldTime;
            bool visible = earned && (trying || lingering);

            float step = config.toughFadeTime > 0f ? deltaTime / config.toughFadeTime : 1f;
            alpha = Mathf.MoveTowards(alpha, visible ? 1f : 0f, step);
            return alpha;
        }
    }
}

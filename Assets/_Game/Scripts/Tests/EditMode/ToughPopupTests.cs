using DungeonJanitor.Hud;
using NUnit.Framework;
using UnityEngine;

namespace DungeonJanitor.Tests
{
    public sealed class ToughPopupTests
    {
        const float Frame = 0.02f;

        HintConfig config;
        ToughPopupTimer timer;
        float now;

        [SetUp]
        public void SetUp()
        {
            config = ScriptableObject.CreateInstance<HintConfig>();
            config.toughShowDelay = 0.3f;
            config.toughContactGrace = 0.1f;
            config.toughHoldTime = 1f;
            config.toughFadeTime = 0.1f;
            timer = new ToughPopupTimer();
            now = 0f;
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(config);

        void Run(float seconds, bool touching)
        {
            for (float t = 0f; t < seconds - Frame * 0.5f; t += Frame)
            {
                now += Frame;
                if (touching)
                    timer.Touch(now, config.toughContactGrace);
                timer.Update(now, Frame, config);
            }
        }

        [Test]
        public void BrushingPast_NeverShows()
        {
            Run(0.2f, true);
            Run(2f, false);
            Assert.That(timer.Alpha, Is.EqualTo(0f));
        }

        [Test]
        public void KeepTrying_ShowsAfterDelay()
        {
            Run(0.25f, true);
            Assert.That(timer.Alpha, Is.EqualTo(0f));
            Run(0.3f, true);
            Assert.That(timer.Alpha, Is.EqualTo(1f));
        }

        [Test]
        public void AfterStopping_LingersThenFades()
        {
            Run(0.6f, true);
            Run(0.5f, false);
            Assert.That(timer.Alpha, Is.EqualTo(1f), "Still showing during the hold time.");
            Run(1f, false);
            Assert.That(timer.Alpha, Is.EqualTo(0f));
        }

        [Test]
        public void ShortGaps_StillCountAsTrying()
        {
            // Touch every other frame (like physics ticks): should still reach the delay.
            for (int i = 0; i < 40; i++)
            {
                now += Frame;
                if (i % 2 == 0)
                    timer.Touch(now, config.toughContactGrace);
                timer.Update(now, Frame, config);
            }
            Assert.That(timer.Alpha, Is.GreaterThan(0f));
        }
    }
}

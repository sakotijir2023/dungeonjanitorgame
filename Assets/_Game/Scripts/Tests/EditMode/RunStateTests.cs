using DungeonJanitor.Core;
using DungeonJanitor.Run;
using NUnit.Framework;

namespace DungeonJanitor.Tests
{
    public sealed class RunStateTests
    {
        const float Tolerance = 1e-4f;

        [Test]
        public void Clock_CountsDownAndEndsOnTimeUp()
        {
            var run = new RunState(10f);
            run.AddMess();
            Assert.That(run.Tick(4f), Is.False);
            Assert.That(run.TimeLeft, Is.EqualTo(6f).Within(Tolerance));
            Assert.That(run.Tick(7f), Is.True, "Reports the end exactly once.");
            Assert.That(run.TimeLeft, Is.EqualTo(0f));
            Assert.That(run.EndReason, Is.EqualTo(RunEndReason.TimeUp));
            Assert.That(run.Tick(1f), Is.False);
        }

        [Test]
        public void CleaningEverything_EndsEarlyWithTimeLeft()
        {
            var run = new RunState(60f);
            run.AddMess();
            run.AddMess();
            run.Tick(20f);
            Assert.That(run.CleanMess(), Is.False);
            Assert.That(run.CleanMess(), Is.True);
            Assert.That(run.EndReason, Is.EqualTo(RunEndReason.AllClean));
            Assert.That(run.TimeLeft, Is.EqualTo(40f).Within(Tolerance));
        }

        [Test]
        public void AfterTheEnd_NothingChanges()
        {
            var run = new RunState(5f);
            run.AddMess();
            run.AddMess();
            run.Tick(10f);
            run.AddMess();
            Assert.That(run.CleanMess(), Is.False);
            Assert.That(run.Total, Is.EqualTo(2));
            Assert.That(run.Cleaned, Is.EqualTo(0));
        }

        [Test]
        public void EmptyRoom_DoesNotEndByItself()
        {
            var run = new RunState(5f);
            Assert.That(run.Tick(1f), Is.False);
            Assert.That(run.Ended, Is.False);
        }

        [Test]
        public void NegativeTime_IsIgnored()
        {
            var run = new RunState(5f);
            run.Tick(-3f);
            Assert.That(run.TimeLeft, Is.EqualTo(5f).Within(Tolerance));
        }
    }
}

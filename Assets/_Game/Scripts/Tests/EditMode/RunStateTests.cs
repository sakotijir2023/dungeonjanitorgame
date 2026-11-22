using DungeonJanitor.Core;
using DungeonJanitor.Messes;
using DungeonJanitor.Run;
using NUnit.Framework;

namespace DungeonJanitor.Tests
{
    public sealed class RunStateTests
    {
        const float Tolerance = 1e-4f;
        const float StartingMax = 3f;

        /// <summary>A run where the tools handle goo and stains up to hardness 3.</summary>
        static RunState NewRun(float duration)
        {
            var run = new RunState(duration);
            run.SetCapabilities(new[] { MessFamily.Goo, MessFamily.Stain }, new[] { StartingMax, StartingMax });
            return run;
        }

        [Test]
        public void Clock_CountsDownAndEndsOnTimeUp()
        {
            RunState run = NewRun(10f);
            run.AddMess(MessFamily.Goo, 1f);
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
            RunState run = NewRun(60f);
            run.AddMess(MessFamily.Goo, 0f);
            run.AddMess(MessFamily.Stain, 2f);
            run.Tick(20f);
            Assert.That(run.CleanMess(MessFamily.Goo, 0f), Is.False);
            Assert.That(run.CleanMess(MessFamily.Stain, 2f), Is.True);
            Assert.That(run.EndReason, Is.EqualTo(RunEndReason.AllClean));
            Assert.That(run.TimeLeft, Is.EqualTo(40f).Within(Tolerance));
        }

        [Test]
        public void TooToughMesses_DoNotBlockAllClean()
        {
            RunState run = NewRun(60f);
            run.AddMess(MessFamily.Goo, 1f);
            run.AddMess(MessFamily.Goo, 5f);
            run.AddMess(MessFamily.Stain, 4f);
            Assert.That(run.Total, Is.EqualTo(1));
            Assert.That(run.TooTough, Is.EqualTo(2));

            Assert.That(run.CleanMess(MessFamily.Goo, 1f), Is.True);
            Assert.That(run.TooToughFamilies(), Is.EquivalentTo(new[] { MessFamily.Goo, MessFamily.Stain }));
        }

        [Test]
        public void FamiliesWithNoTool_AreTooTough()
        {
            RunState run = NewRun(60f);
            run.AddMess(MessFamily.Cobweb, 0f);
            Assert.That(run.TooTough, Is.EqualTo(1));
            Assert.That(run.Total, Is.EqualTo(0));
        }

        [Test]
        public void ToolsReportedLate_RecountsMessesSpawnedBefore()
        {
            var run = new RunState(60f);
            run.AddMess(MessFamily.Goo, 2f);
            Assert.That(run.Total, Is.EqualTo(0), "Nothing counts until the tools report in.");
            run.SetCapabilities(new[] { MessFamily.Goo }, new[] { StartingMax });
            Assert.That(run.Total, Is.EqualTo(1));
        }

        [Test]
        public void HardnessExactlyAtTheLimit_IsCleanable()
        {
            RunState run = NewRun(60f);
            run.AddMess(MessFamily.Stain, StartingMax);
            Assert.That(run.Total, Is.EqualTo(1));
        }

        [Test]
        public void AfterTheEnd_NothingChanges()
        {
            RunState run = NewRun(5f);
            run.AddMess(MessFamily.Goo, 0f);
            run.AddMess(MessFamily.Goo, 0f);
            run.Tick(10f);
            run.AddMess(MessFamily.Goo, 0f);
            Assert.That(run.CleanMess(MessFamily.Goo, 0f), Is.False);
            Assert.That(run.Total, Is.EqualTo(2));
            Assert.That(run.Cleaned, Is.EqualTo(0));
        }

        [Test]
        public void EmptyRoom_DoesNotEndByItself()
        {
            RunState run = NewRun(5f);
            Assert.That(run.Tick(1f), Is.False);
            Assert.That(run.Ended, Is.False);
        }

        [Test]
        public void NegativeTime_IsIgnored()
        {
            RunState run = NewRun(5f);
            run.Tick(-3f);
            Assert.That(run.TimeLeft, Is.EqualTo(5f).Within(Tolerance));
        }
    }
}

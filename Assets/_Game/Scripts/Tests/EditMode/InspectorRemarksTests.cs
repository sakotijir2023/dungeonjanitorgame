using System.Collections.Generic;
using DungeonJanitor.Inspection;
using DungeonJanitor.Messes;
using DungeonJanitor.Scoring;
using NUnit.Framework;
using UnityEngine;

namespace DungeonJanitor.Tests
{
    public sealed class InspectorRemarksTests
    {
        InspectorLines lines;

        static int First(int count) => 0;
        static int Last(int count) => count - 1;

        [SetUp]
        public void SetUp()
        {
            lines = ScriptableObject.CreateInstance<InspectorLines>();
            lines.byGrade = new List<InspectorLines.GradeLines>
            {
                new InspectorLines.GradeLines { grade = Grade.S, lines = new[] { "s1" } },
                new InspectorLines.GradeLines { grade = Grade.A, lines = new[] { "a1", "a2" } },
            };
            lines.nothingCleaned = new[] { "nothing" };
            lines.tooToughHints = new List<InspectorLines.FamilyHints>
            {
                new InspectorLines.FamilyHints { family = MessFamily.Stain, lines = new[] { "stain hint" } },
            };
            lines.genericTooToughHints = new[] { "generic hint" };
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(lines);

        [Test]
        public void PicksFromTheRightGrade()
        {
            Assert.That(InspectorRemarks.PickRemark(lines, Grade.S, 5, First), Is.EqualTo("s1"));
            Assert.That(InspectorRemarks.PickRemark(lines, Grade.A, 5, Last), Is.EqualTo("a2"));
        }

        [Test]
        public void NothingCleaned_OverridesGrade()
        {
            Assert.That(InspectorRemarks.PickRemark(lines, Grade.A, 0, First), Is.EqualTo("nothing"));
        }

        [Test]
        public void MissingGrade_ReturnsEmptyInsteadOfFailing()
        {
            Assert.That(InspectorRemarks.PickRemark(lines, Grade.F, 3, First), Is.Empty);
        }

        [Test]
        public void Hint_UsesFamilyLineOrGenericFallback()
        {
            Assert.That(InspectorRemarks.PickHint(lines, new[] { MessFamily.Stain }, First), Is.EqualTo("stain hint"));
            Assert.That(InspectorRemarks.PickHint(lines, new[] { MessFamily.Cobweb }, First), Is.EqualTo("generic hint"));
        }

        [Test]
        public void NoTooToughMesses_NoHint()
        {
            Assert.That(InspectorRemarks.PickHint(lines, new MessFamily[0], First), Is.Empty);
            Assert.That(InspectorRemarks.PickHint(lines, null, First), Is.Empty);
        }
    }
}

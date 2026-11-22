using System;
using System.Collections.Generic;
using DungeonJanitor.Messes;
using DungeonJanitor.Scoring;

namespace DungeonJanitor.Inspection
{
    /// <summary>
    /// Picks what the Inspector says. Pure: the random source is passed in
    /// (returns 0..count-1), so tests can control it.
    /// </summary>
    public static class InspectorRemarks
    {
        public static string PickRemark(InspectorLines lines, Grade grade, int cleanedCount, Func<int, int> random)
        {
            if (cleanedCount == 0 && lines.nothingCleaned.Length > 0)
                return Pick(lines.nothingCleaned, random);
            GradeLines(lines, grade, out string[] options);
            return Pick(options, random);
        }

        /// <summary>A hint about one of the too-tough mess types left behind, or empty if none.</summary>
        public static string PickHint(InspectorLines lines, IReadOnlyList<MessFamily> tooToughFamilies, Func<int, int> random)
        {
            if (tooToughFamilies == null || tooToughFamilies.Count == 0)
                return string.Empty;
            MessFamily family = tooToughFamilies[random(tooToughFamilies.Count)];
            foreach (InspectorLines.FamilyHints hints in lines.tooToughHints)
            {
                if (hints.family == family && hints.lines.Length > 0)
                    return Pick(hints.lines, random);
            }
            return Pick(lines.genericTooToughHints, random);
        }

        static void GradeLines(InspectorLines lines, Grade grade, out string[] options)
        {
            foreach (InspectorLines.GradeLines entry in lines.byGrade)
            {
                if (entry.grade == grade && entry.lines.Length > 0)
                {
                    options = entry.lines;
                    return;
                }
            }
            options = Array.Empty<string>();
        }

        static string Pick(string[] options, Func<int, int> random) =>
            options == null || options.Length == 0 ? string.Empty : options[random(options.Length)];
    }
}

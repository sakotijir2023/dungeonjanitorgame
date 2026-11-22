using System;
using System.Collections.Generic;
using DungeonJanitor.Messes;
using DungeonJanitor.Scoring;
using UnityEngine;

namespace DungeonJanitor.Inspection
{
    /// <summary>
    /// Everything the Inspector can say on the report card. Edit freely in the
    /// Unity Inspector; rebuilding the test room never overwrites this asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Inspector Lines", fileName = "InspectorLines")]
    public sealed class InspectorLines : ScriptableObject
    {
        [Serializable]
        public sealed class GradeLines
        {
            public Grade grade;
            [TextArea(2, 4)] public string[] lines = Array.Empty<string>();
        }

        [Serializable]
        public sealed class FamilyHints
        {
            public MessFamily family;
            [TextArea(2, 4)] public string[] lines = Array.Empty<string>();
        }

        [Tooltip("One remark is picked at random for the grade you got.")]
        public List<GradeLines> byGrade = new List<GradeLines>();

        [Tooltip("Used instead of the grade remark when you cleaned nothing at all.")]
        [TextArea(2, 4)] public string[] nothingCleaned = Array.Empty<string>();

        [Tooltip("Hints when you left a mess your tools couldn't handle, by mess type.")]
        public List<FamilyHints> tooToughHints = new List<FamilyHints>();

        [Tooltip("Hint used when there's no line for that mess type.")]
        [TextArea(2, 4)] public string[] genericTooToughHints = Array.Empty<string>();

        public bool IsEmpty => byGrade.Count == 0;
    }
}

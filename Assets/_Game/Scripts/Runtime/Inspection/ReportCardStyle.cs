using UnityEngine;

namespace DungeonJanitor.Inspection
{
    /// <summary>Look and fixed wording of the Inspector's report card. His remarks live in InspectorLines.</summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Report Card Style", fileName = "ReportCardStyle")]
    public sealed class ReportCardStyle : ScriptableObject
    {
        [Header("Canvas")]
        public Vector2 referenceResolution = new Vector2(1920f, 1080f);
        public int sortingOrder = 200;

        [Header("Layout")]
        public Vector2 cardSize = new Vector2(1040f, 850f);
        public int padding = 36;
        public float spacing = 8f;
        public float headerHeight = 40f;
        public float titleHeight = 64f;
        public float statsHeight = 110f;
        public float gradeHeight = 150f;
        public float payHeight = 44f;
        public float remarkHeight = 130f;
        public float signatureHeight = 34f;
        public float hintHeight = 90f;
        public float footerHeight = 34f;

        [Header("Text sizes")]
        public Font font;
        public int headerFontSize = 24;
        public int titleFontSize = 52;
        public int statsFontSize = 28;
        public int gradeFontSize = 140;
        public int payFontSize = 32;
        public int remarkFontSize = 32;
        public int signatureFontSize = 24;
        public int hintFontSize = 26;
        public int footerFontSize = 22;

        [Header("Wording")]
        public string header = "INSPECTION REPORT";
        public string allCleanTitle = "Shift complete";
        public string timeUpTitle = "Time's up";
        [Tooltip("{0} cleaned, {1} could clean, {2} time left, {3} too-tough line.")]
        public string statsFormat = "Cleaned {0} of {1}     Time left {2}{3}";
        [Tooltip("{0} = count.")]
        public string tooToughFormat = "\nLeft behind (too tough for your tools): {0}";
        [Tooltip("{0} points, {1} early bonus, {2} pay.")]
        public string payFormat = "{0:0} pts  +  {1:0} early bonus      Pay  ${2}";
        public string remarkFormat = "“{0}”";
        public string signature = "— the Inspector";
        public string footer = "F5 / Start to try again";

        [Header("Colours")]
        public Color backdropColor = new Color(0f, 0f, 0f, 0.55f);
        public Color cardColor = new Color(0.86f, 0.82f, 0.72f, 0.97f);
        public Color inkColor = new Color(0.16f, 0.13f, 0.11f, 1f);
        public Color mutedInkColor = new Color(0.36f, 0.31f, 0.27f, 1f);
        public Color hintColor = new Color(0.6f, 0.18f, 0.1f, 1f);
        public Color gradeS = new Color(0.78f, 0.58f, 0.08f, 1f);
        public Color gradeA = new Color(0.17f, 0.5f, 0.24f, 1f);
        public Color gradeB = new Color(0.16f, 0.42f, 0.5f, 1f);
        public Color gradeC = new Color(0.55f, 0.45f, 0.1f, 1f);
        public Color gradeD = new Color(0.72f, 0.36f, 0.08f, 1f);
        public Color gradeF = new Color(0.7f, 0.1f, 0.08f, 1f);
    }
}

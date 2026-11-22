using UnityEngine;

namespace DungeonJanitor.Run
{
    /// <summary>Look and wording of the shift clock and mess counter.</summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Run HUD Style", fileName = "RunHudStyle")]
    public sealed class RunHudStyle : ScriptableObject
    {
        [Header("Canvas")]
        public Vector2 referenceResolution = new Vector2(1920f, 1080f);
        public int sortingOrder = 100;

        [Header("Layout")]
        public float topMargin = 24f;
        public float clockHeight = 80f;
        public float counterHeight = 40f;

        [Header("Text")]
        public Font font;
        public int clockFontSize = 64;
        public int counterFontSize = 30;
        public string clockFormat = "{0}:{1:00}";
        public string counterFormat = "Messes left: {0} / {1}";
        [Tooltip("Added after the counter when some messes are beyond your tools. {0} = how many.")]
        public string tooToughFormat = "   (+{0} too tough for your tools)";

        [Header("Colours")]
        public Color textColor = new Color(0.95f, 0.93f, 0.88f, 1f);
        public Color warningColor = new Color(1f, 0.45f, 0.3f, 1f);
        public Color shadowColor = new Color(0f, 0f, 0f, 0.7f);
        public Vector2 shadowOffset = new Vector2(2f, -2f);
    }
}

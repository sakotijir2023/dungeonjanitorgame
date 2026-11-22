using UnityEngine;

namespace DungeonJanitor.Run
{
    /// <summary>Look and wording of the shift clock, mess counter and end banner.</summary>
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
        public float bannerWidth = 900f;
        public float bannerHeight = 260f;

        [Header("Text")]
        public Font font;
        public int clockFontSize = 64;
        public int counterFontSize = 30;
        public int bannerTitleFontSize = 64;
        public int bannerBodyFontSize = 30;
        public string clockFormat = "{0}:{1:00}";
        public string counterFormat = "Messes left: {0} / {1}";
        public string timeUpTitle = "Time's up!";
        public string allCleanTitle = "Spotless!";
        public string bannerBodyFormat = "Cleaned {0} of {1}\nTime left: {2}\n\n<size={3}>F5 / Start to restart</size>";
        public int bannerHintFontSize = 22;

        [Header("Colours")]
        public Color textColor = new Color(0.95f, 0.93f, 0.88f, 1f);
        public Color warningColor = new Color(1f, 0.45f, 0.3f, 1f);
        public Color shadowColor = new Color(0f, 0f, 0f, 0.7f);
        public Vector2 shadowOffset = new Vector2(2f, -2f);
        public Color bannerColor = new Color(0.06f, 0.06f, 0.08f, 0.88f);
        public Color allCleanColor = new Color(0.55f, 0.9f, 0.6f, 1f);
    }
}

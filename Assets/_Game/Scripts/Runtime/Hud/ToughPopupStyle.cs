using UnityEngine;

namespace DungeonJanitor.Hud
{
    /// <summary>Look and wording of the "too tough" popup under the crosshair.</summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Tough Popup Style", fileName = "ToughPopupStyle")]
    public sealed class ToughPopupStyle : ScriptableObject
    {
        public Vector2 referenceResolution = new Vector2(1920f, 1080f);
        public int sortingOrder = 110;
        [Tooltip("Offset from screen centre (UI pixels at 1080p). Negative y = below the crosshair.")]
        public Vector2 offset = new Vector2(0f, -120f);
        public Vector2 size = new Vector2(900f, 100f);

        public Font font;
        public int fontSize = 34;
        public int subFontSize = 24;
        [Tooltip("{0} = tool name.")]
        public string titleFormat = "Too tough for your {0}";
        public string subtitle = "upgrade needed";

        public Color textColor = new Color(1f, 0.55f, 0.35f, 1f);
        public Color subColor = new Color(0.9f, 0.85f, 0.8f, 1f);
        public Color shadowColor = new Color(0f, 0f, 0f, 0.8f);
        public Vector2 shadowOffset = new Vector2(2f, -2f);
    }
}

using UnityEngine;

namespace DungeonJanitor.Tools.Wheel
{
    /// <summary>
    /// Colours, sprites and wording for the tool wheel. Not a TuningConfig, so it
    /// stays out of the slider panel; sizes and timings live in ToolWheelConfig.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Tool Wheel Style", fileName = "ToolWheelStyle")]
    public sealed class ToolWheelStyle : ScriptableObject
    {
        [Header("Canvas")]
        public Vector2 referenceResolution = new Vector2(1920f, 1080f);
        public int sortingOrder = 900;

        [Header("Shapes")]
        [Tooltip("Round sprite used for slots and the centre.")]
        public Sprite disc;
        [Tooltip("Outline ring size relative to its slot.")]
        public float outlineScale = 1.12f;
        public float centerSize = 220f;

        [Header("Text")]
        public Font font;
        public int labelFontSize = 24;
        public int detailFontSize = 17;
        public int centerFontSize = 34;
        public int hintFontSize = 20;
        public string backLabel = "Back";
        public string folderHint = "keep pointing to open";
        public string unavailableHint = "not in your kit yet";

        [Header("Colours")]
        public Color backdropColor = new Color(0f, 0f, 0f, 0.35f);
        public Color centerColor = new Color(0.08f, 0.08f, 0.1f, 0.85f);
        public Color slotColor = new Color(0.16f, 0.17f, 0.2f, 0.92f);
        public Color hoverColor = new Color(0.36f, 0.62f, 0.42f, 0.95f);
        public Color unavailableColor = new Color(0.1f, 0.1f, 0.11f, 0.7f);
        public Color unavailableHoverColor = new Color(0.22f, 0.2f, 0.2f, 0.85f);
        public Color folderColor = new Color(0.25f, 0.2f, 0.12f, 0.92f);
        public Color backColor = new Color(0.22f, 0.12f, 0.12f, 0.92f);
        public Color currentOutlineColor = new Color(1f, 0.85f, 0.4f, 0.95f);
        public Color textColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        public Color pointerColor = new Color(1f, 0.85f, 0.4f, 0.9f);
        public Color unavailableTextColor = new Color(0.5f, 0.5f, 0.52f, 1f);
    }
}

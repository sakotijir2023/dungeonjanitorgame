using UnityEngine;

namespace DungeonJanitor.Tuning
{
    /// <summary>
    /// Look and layout of the debug tuning panel. Deliberately not a TuningConfig,
    /// so the panel does not list its own layout as sliders.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Tuning Panel Style", fileName = "TuningPanelStyle")]
    public sealed class TuningPanelStyle : ScriptableObject
    {
        [Header("Canvas")]
        public Vector2 referenceResolution = new Vector2(1920f, 1080f);
        public int sortingOrder = 1000;

        [Header("Layout")]
        public float panelWidth = 1040f;
        public float panelMargin = 24f;
        public int padding = 18;
        public float spacing = 6f;
        public float rowHeight = 44f;
        public float headerHeight = 54f;
        public float labelWidth = 420f;
        public float valueWidth = 120f;
        public float buttonWidth = 200f;
        public float smallButtonWidth = 60f;
        public float scrollSensitivity = 30f;

        [Header("Text")]
        public Font font;
        public int fontSize = 26;
        public int headerFontSize = 30;
        public int titleFontSize = 34;
        public string valueFormat = "0.###";
        public string title = "Tuning  (F1 / View button)";
        public string presetNamePlaceholder = "Preset name...";
        public string defaultPresetPrefix = "Preset_";
        public string defaultPresetTimeFormat = "yyyyMMdd_HHmmss";

        [Header("Colors")]
        public Color panelColor = new Color(0.07f, 0.07f, 0.09f, 0.94f);
        public Color textColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        public Color headerColor = new Color(0.55f, 0.85f, 0.6f, 1f);
        public Color subHeaderColor = new Color(0.7f, 0.7f, 0.75f, 1f);
        public Color sliderBackgroundColor = new Color(0.2f, 0.2f, 0.24f, 1f);
        public Color sliderFillColor = new Color(0.35f, 0.6f, 0.4f, 1f);
        public Color handleColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        public Color buttonColor = new Color(0.22f, 0.22f, 0.27f, 1f);
        public Color inputColor = new Color(0.15f, 0.15f, 0.18f, 1f);
        public Color selectedTint = new Color(1f, 0.85f, 0.4f, 1f);
    }
}

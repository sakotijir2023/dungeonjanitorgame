using System;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Tuning
{
    /// <summary>Builds the tuning panel's UGUI widgets in code, styled from TuningPanelStyle.</summary>
    public sealed class TuningUIFactory
    {
        const string BuiltinFontName = "LegacyRuntime.ttf";
        const float SliderHeightFraction = 0.6f;
        const float HoverTintBlend = 0.5f;

        readonly TuningPanelStyle style;
        readonly Font font;
        readonly DefaultControls.Resources resources = new DefaultControls.Resources();

        public TuningUIFactory(TuningPanelStyle style)
        {
            this.style = style;
            font = style.font != null ? style.font : Resources.GetBuiltinResource<Font>(BuiltinFontName);
        }

        public Canvas CreateCanvas(Transform parent)
        {
            var go = new GameObject("TuningCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.transform.SetParent(parent, false);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = style.sortingOrder;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = style.referenceResolution;
            return canvas;
        }

        /// <summary>Left-docked full-height panel with a vertical layout.</summary>
        public RectTransform CreatePanel(Transform parent)
        {
            var go = new GameObject("Panel", typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(style.panelWidth, -2f * style.panelMargin);
            rect.anchoredPosition = new Vector2(style.panelMargin, 0f);
            go.GetComponent<Image>().color = style.panelColor;
            ConfigureVertical(go.GetComponent<VerticalLayoutGroup>(), style.padding);
            return rect;
        }

        /// <summary>Scroll view that fills the remaining panel height. Returns the content transform.</summary>
        public RectTransform CreateScrollView(Transform parent, out ScrollRect scrollRect)
        {
            var root = new GameObject("Scroll", typeof(RectTransform), typeof(ScrollRect), typeof(LayoutElement));
            root.transform.SetParent(parent, false);
            root.GetComponent<LayoutElement>().flexibleHeight = 1f;

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            var viewportRect = (RectTransform)viewport.transform;
            viewportRect.SetParent(root.transform, false);
            Stretch(viewportRect);

            var content = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            var contentRect = (RectTransform)content.transform;
            contentRect.SetParent(viewportRect, false);
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.sizeDelta = Vector2.zero;
            ConfigureVertical(content.GetComponent<VerticalLayoutGroup>(), 0);
            content.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect = root.GetComponent<ScrollRect>();
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = style.scrollSensitivity;
            return contentRect;
        }

        public RectTransform CreateRow(Transform parent, string name = "Row")
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var layout = go.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = style.spacing;
            layout.childAlignment = TextAnchor.MiddleLeft;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = layout.childForceExpandHeight = false;
            var element = go.GetComponent<LayoutElement>();
            element.minHeight = element.preferredHeight = style.rowHeight;
            return (RectTransform)go.transform;
        }

        public Text CreateText(Transform parent, string content, int fontSize, Color color, float width = -1f, float height = -1f)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.text = content;
            StyleText(text, fontSize, color);
            var element = go.GetComponent<LayoutElement>();
            element.preferredWidth = width;
            element.preferredHeight = height;
            element.flexibleWidth = width < 0f ? 1f : 0f;
            return text;
        }

        public Slider CreateSlider(Transform parent, float min, float max, bool wholeNumbers)
        {
            GameObject go = DefaultControls.CreateSlider(resources);
            go.transform.SetParent(parent, false);
            AddFlexibleElement(go);
            go.transform.Find("Background").GetComponent<Image>().color = style.sliderBackgroundColor;
            go.transform.Find("Fill Area/Fill").GetComponent<Image>().color = style.sliderFillColor;
            go.transform.Find("Handle Slide Area/Handle").GetComponent<Image>().color = style.handleColor;
            var slider = go.GetComponent<Slider>();
            slider.minValue = min;
            slider.maxValue = max;
            slider.wholeNumbers = wholeNumbers;
            ApplySelectableColors(slider);
            return slider;
        }

        public Toggle CreateToggle(Transform parent)
        {
            GameObject go = DefaultControls.CreateToggle(resources);
            go.transform.SetParent(parent, false);
            go.AddComponent<LayoutElement>().preferredWidth = style.rowHeight;
            var label = go.transform.Find("Label");
            if (label != null)
                UnityEngine.Object.Destroy(label.gameObject);
            go.transform.Find("Background").GetComponent<Image>().color = style.sliderBackgroundColor;
            go.transform.Find("Background/Checkmark").GetComponent<Image>().color = style.sliderFillColor;
            var toggle = go.GetComponent<Toggle>();
            ApplySelectableColors(toggle);
            return toggle;
        }

        public Button CreateButton(Transform parent, string label, Action onClick, float width)
        {
            GameObject go = DefaultControls.CreateButton(resources);
            go.transform.SetParent(parent, false);
            var element = go.AddComponent<LayoutElement>();
            element.preferredWidth = width;
            element.preferredHeight = style.rowHeight;
            element.flexibleWidth = width < 0f ? 1f : 0f;
            go.GetComponent<Image>().color = style.buttonColor;
            var text = go.GetComponentInChildren<Text>();
            text.text = label;
            StyleText(text, style.fontSize, style.textColor);
            text.alignment = TextAnchor.MiddleCenter;
            var button = go.GetComponent<Button>();
            button.onClick.AddListener(() => onClick());
            ApplySelectableColors(button);
            return button;
        }

        public InputField CreateInputField(Transform parent, string placeholder)
        {
            GameObject go = DefaultControls.CreateInputField(resources);
            go.transform.SetParent(parent, false);
            AddFlexibleElement(go).preferredHeight = style.rowHeight;
            go.GetComponent<Image>().color = style.inputColor;
            var field = go.GetComponent<InputField>();
            StyleText(field.textComponent, style.fontSize, style.textColor);
            var placeholderText = (Text)field.placeholder;
            placeholderText.text = placeholder;
            StyleText(placeholderText, style.fontSize, style.subHeaderColor);
            ApplySelectableColors(field);
            return field;
        }

        void StyleText(Text text, int fontSize, Color color)
        {
            text.font = font;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
        }

        void ApplySelectableColors(Selectable selectable)
        {
            ColorBlock colors = selectable.colors;
            colors.selectedColor = style.selectedTint;
            colors.highlightedColor = Color.Lerp(Color.white, style.selectedTint, HoverTintBlend);
            selectable.colors = colors;
        }

        LayoutElement AddFlexibleElement(GameObject go)
        {
            var element = go.AddComponent<LayoutElement>();
            element.flexibleWidth = 1f;
            element.preferredHeight = style.rowHeight * SliderHeightFraction;
            return element;
        }

        void ConfigureVertical(VerticalLayoutGroup layout, int padding)
        {
            layout.padding = new RectOffset(padding, padding, padding, padding);
            layout.spacing = style.spacing;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Tools.Wheel
{
    /// <summary>
    /// Draws the tool wheel with UGUI built in code. Knows nothing about tools or
    /// input; it is handed entries and a hovered index. Animates on unscaled time
    /// so it stays snappy during slow-motion.
    /// </summary>
    public sealed class ToolWheelView : MonoBehaviour
    {
        const string BuiltinFontName = "LegacyRuntime.ttf";
        const int MaxSlots = 12;
        const float MatchHeight = 1f;

        sealed class SlotWidget
        {
            public RectTransform Rect;
            public Image Outline;
            public Image Disc;
            public Text Label;
            public WheelEntry Entry;
            public float Scale = 1f;
        }

        readonly List<SlotWidget> slots = new List<SlotWidget>();
        ToolWheelStyle style;
        ToolWheelConfig config;
        Font font;
        GameObject canvasRoot;
        RectTransform ring;
        Text centerText;
        int hovered = -1;
        int visibleCount;
        float openElapsed;

        public void Build(ToolWheelStyle wheelStyle, ToolWheelConfig wheelConfig)
        {
            style = wheelStyle;
            config = wheelConfig;
            font = style.font != null ? style.font : Resources.GetBuiltinResource<Font>(BuiltinFontName);

            var canvasGo = new GameObject("ToolWheelCanvas", typeof(Canvas), typeof(CanvasScaler));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = style.sortingOrder;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = style.referenceResolution;
            scaler.matchWidthOrHeight = MatchHeight;
            canvasRoot = canvasGo;

            Image backdrop = CreateImage("Backdrop", canvasGo.transform, null, style.backdropColor);
            backdrop.rectTransform.anchorMin = Vector2.zero;
            backdrop.rectTransform.anchorMax = Vector2.one;
            backdrop.rectTransform.sizeDelta = Vector2.zero;

            ring = new GameObject("Ring", typeof(RectTransform)).GetComponent<RectTransform>();
            ring.SetParent(canvasGo.transform, false);

            Image center = CreateImage("Center", ring, style.disc, style.centerColor);
            center.rectTransform.sizeDelta = Vector2.one * style.centerSize;
            centerText = CreateText("CenterText", center.transform, style.centerFontSize);

            for (int i = 0; i < MaxSlots; i++)
                slots.Add(CreateSlot(i));

            canvasRoot.SetActive(false);
        }

        SlotWidget CreateSlot(int index)
        {
            var rect = new GameObject($"Slot{index}", typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(ring, false);
            var widget = new SlotWidget
            {
                Rect = rect,
                Outline = CreateImage("Outline", rect, style.disc, style.currentOutlineColor),
                Disc = CreateImage("Disc", rect, style.disc, style.slotColor),
            };
            widget.Label = CreateText("Label", widget.Disc.transform, style.labelFontSize);
            return widget;
        }

        Image CreateImage(string objectName, Transform parent, Sprite sprite, Color color)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        Text CreateText(string objectName, Transform parent, int size)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(Text));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            var text = go.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = true;
            text.color = style.textColor;
            text.raycastTarget = false;
            return text;
        }

        public void Show()
        {
            openElapsed = 0f;
            canvasRoot.SetActive(true);
            Animate(0f);
        }

        public void Hide() => canvasRoot.SetActive(false);

        public void ShowRing(IReadOnlyList<WheelEntry> entries, float firstAngle)
        {
            visibleCount = Mathf.Min(entries.Count, MaxSlots);
            hovered = -1;
            for (int i = 0; i < slots.Count; i++)
            {
                SlotWidget widget = slots[i];
                bool used = i < visibleCount;
                widget.Rect.gameObject.SetActive(used);
                if (!used)
                    continue;

                widget.Entry = entries[i];
                Vector2 direction = WheelMath.DirectionFromIndex(i, visibleCount, firstAngle);
                widget.Rect.anchoredPosition = direction * config.ringRadius;
                widget.Disc.rectTransform.sizeDelta = Vector2.one * config.slotSize;
                widget.Outline.rectTransform.sizeDelta = Vector2.one * (config.slotSize * style.outlineScale);
                widget.Outline.enabled = widget.Entry.Current;
                widget.Label.text = string.IsNullOrEmpty(widget.Entry.Detail)
                    ? widget.Entry.Label
                    : $"{widget.Entry.Label}\n<size={style.detailFontSize}>{widget.Entry.Detail}</size>";
                widget.Label.color = widget.Entry.Available ? style.textColor : style.unavailableTextColor;
                widget.Scale = 1f;
                ApplyColor(widget, false);
            }
        }

        public void SetHovered(int index, string centerLabel)
        {
            if (hovered >= 0 && hovered < visibleCount)
                ApplyColor(slots[hovered], false);
            hovered = index;
            if (hovered >= 0 && hovered < visibleCount)
                ApplyColor(slots[hovered], true);
            centerText.text = centerLabel;
        }

        void ApplyColor(SlotWidget widget, bool isHovered)
        {
            WheelEntry entry = widget.Entry;
            Color color;
            if (!entry.Available)
                color = isHovered ? style.unavailableHoverColor : style.unavailableColor;
            else if (isHovered)
                color = style.hoverColor;
            else if (entry.IsBack)
                color = style.backColor;
            else if (entry.IsFolder)
                color = style.folderColor;
            else
                color = style.slotColor;
            widget.Disc.color = color;
        }

        void Update()
        {
            if (canvasRoot != null && canvasRoot.activeSelf)
                Animate(Time.unscaledDeltaTime);
        }

        void Animate(float dt)
        {
            openElapsed += dt;
            float open = config.openTime > 0f ? Mathf.Clamp01(openElapsed / config.openTime) : 1f;
            float eased = 1f - (1f - open) * (1f - open);
            ring.localScale = Vector3.one * Mathf.Lerp(config.openStartScale, 1f, eased);

            float blend = 1f - Mathf.Exp(-config.hoverSpeed * dt);
            for (int i = 0; i < visibleCount; i++)
            {
                SlotWidget widget = slots[i];
                float target = i == hovered ? config.hoverScale : 1f;
                widget.Scale = Mathf.Lerp(widget.Scale, target, blend);
                widget.Rect.localScale = Vector3.one * widget.Scale;
            }
        }
    }
}

using DungeonJanitor.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Run
{
    /// <summary>
    /// Shift clock and mess counter at the top of the screen. Freezes on the
    /// final values when the shift ends. Listens to run events only.
    /// </summary>
    public sealed class RunHud : MonoBehaviour
    {
        const string BuiltinFontName = "LegacyRuntime.ttf";
        const int SecondsPerMinute = 60;

        [SerializeField] RunConfig config;
        [SerializeField] RunHudStyle style;

        Font font;
        Text clock;
        Text counter;
        int shownSeconds = -1;
        int shownCleaned = -1;
        int shownTotal = -1;
        int shownTough = -1;

        void Awake()
        {
            font = style.font != null ? style.font : Resources.GetBuiltinResource<Font>(BuiltinFontName);
            Build();
        }

        void OnEnable()
        {
            EventBus<RunStatusChanged>.Subscribe(OnStatus);
            EventBus<RunEnded>.Subscribe(OnEnded);
        }

        void OnDisable()
        {
            EventBus<RunStatusChanged>.Unsubscribe(OnStatus);
            EventBus<RunEnded>.Unsubscribe(OnEnded);
        }

        void OnStatus(RunStatusChanged evt)
        {
            int seconds = Mathf.CeilToInt(evt.TimeLeft);
            if (seconds != shownSeconds)
            {
                shownSeconds = seconds;
                clock.text = FormatTime(seconds);
                clock.color = evt.TimeLeft <= config.warningTime ? style.warningColor : style.textColor;
            }
            if (evt.Cleaned != shownCleaned || evt.Total != shownTotal || evt.TooTough != shownTough)
            {
                shownCleaned = evt.Cleaned;
                shownTotal = evt.Total;
                shownTough = evt.TooTough;
                counter.text = CounterText(evt.Cleaned, evt.Total, evt.TooTough);
            }
        }

        void OnEnded(RunEnded evt)
        {
            clock.text = FormatTime(Mathf.CeilToInt(evt.TimeLeft));
            counter.text = CounterText(evt.Cleaned, evt.Total, evt.TooTough);
        }

        string CounterText(int cleaned, int total, int tooTough)
        {
            string text = string.Format(style.counterFormat, total - cleaned, total);
            return tooTough > 0 ? text + string.Format(style.tooToughFormat, tooTough) : text;
        }

        string FormatTime(int totalSeconds) =>
            string.Format(style.clockFormat, totalSeconds / SecondsPerMinute, totalSeconds % SecondsPerMinute);

        void Build()
        {
            var canvasGo = new GameObject("RunHudCanvas", typeof(Canvas), typeof(CanvasScaler));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = style.sortingOrder;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = style.referenceResolution;
            scaler.matchWidthOrHeight = 1f;

            clock = CreateText(canvasGo.transform, "Clock", style.clockFontSize, style.clockHeight,
                new Vector2(0f, -style.topMargin));
            counter = CreateText(canvasGo.transform, "Counter", style.counterFontSize, style.counterHeight,
                new Vector2(0f, -style.topMargin - style.clockHeight));
        }

        /// <summary>Full-width text anchored to the top of the screen.</summary>
        Text CreateText(Transform parent, string objectName, int size, float height, Vector2 position)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(Text), typeof(Shadow));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(0f, height);
            rect.anchoredPosition = position;
            var text = go.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = true;
            text.color = style.textColor;
            text.raycastTarget = false;
            var shadow = go.GetComponent<Shadow>();
            shadow.effectColor = style.shadowColor;
            shadow.effectDistance = style.shadowOffset;
            return text;
        }
    }
}

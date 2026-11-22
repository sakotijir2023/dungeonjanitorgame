using DungeonJanitor.Core;
using DungeonJanitor.Scoring;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Inspection
{
    /// <summary>
    /// The Inspector's report card at the end of a shift. Fades in, stamps the
    /// grade, then writes his remark out and adds a hint about anything your
    /// tools couldn't handle. Runs on real time. Listens to RunScored only.
    /// </summary>
    public sealed class ReportCard : MonoBehaviour
    {
        const string BuiltinFontName = "LegacyRuntime.ttf";
        const int SecondsPerMinute = 60;

        [SerializeField] InspectorConfig config;
        [SerializeField] InspectorLines lines;
        [SerializeField] ReportCardStyle style;

        Font font;
        GameObject canvasRoot;
        CanvasGroup group;
        Text title;
        Text stats;
        Text grade;
        Text pay;
        Text remark;
        Text signature;
        Text hint;
        string remarkFull = string.Empty;
        string hintFull = string.Empty;
        float shownAt;
        bool showing;

        void Awake()
        {
            font = style.font != null ? style.font : Resources.GetBuiltinResource<Font>(BuiltinFontName);
            Build();
        }

        void OnEnable() => EventBus<RunScored>.Subscribe(OnScored);
        void OnDisable() => EventBus<RunScored>.Unsubscribe(OnScored);

        void OnScored(RunScored evt)
        {
            RunEnded run = evt.Run;
            ScoreResult score = evt.Score;
            title.text = run.Reason == RunEndReason.AllClean ? style.allCleanTitle : style.timeUpTitle;
            string tough = run.TooTough > 0 ? string.Format(style.tooToughFormat, run.TooTough) : string.Empty;
            stats.text = string.Format(style.statsFormat, run.Cleaned, run.Total, FormatTime(run.TimeLeft), tough);
            grade.text = score.Grade.ToString();
            grade.color = GradeColor(score.Grade);
            pay.text = string.Format(style.payFormat, score.Points, score.EarlyBonus, score.Cash);

            remarkFull = InspectorRemarks.PickRemark(lines, score.Grade, run.Cleaned, PickIndex);
            hintFull = InspectorRemarks.PickHint(lines, run.TooToughFamilies, PickIndex);

            showing = true;
            shownAt = Time.unscaledTime;
            canvasRoot.SetActive(true);
            Animate();
        }

        static int PickIndex(int count) => Random.Range(0, count);

        void Update()
        {
            if (showing)
                Animate();
        }

        void Animate()
        {
            float t = Time.unscaledTime - shownAt;
            group.alpha = config.cardFadeTime > 0f ? Mathf.Clamp01(t / config.cardFadeTime) : 1f;

            // Grade: hidden, then slams down from big to normal size.
            float stampT = config.stampTime > 0f ? (t - config.gradeDelay) / config.stampTime : 1f;
            grade.enabled = stampT >= 0f;
            float landed = Mathf.Clamp01(stampT);
            grade.transform.localScale = Vector3.one * Mathf.Lerp(config.stampStartScale, 1f, landed * landed);

            // Remark: written out letter by letter.
            float remarkStart = config.gradeDelay + config.stampTime + config.remarkDelay;
            int letters = Letters(t - remarkStart, remarkFull.Length);
            remark.text = letters > 0 ? string.Format(style.remarkFormat, remarkFull.Substring(0, letters)) : string.Empty;
            signature.enabled = letters >= remarkFull.Length && remarkFull.Length > 0;

            // Hint: after the remark is done.
            float writeTime = config.typeSpeed > 0f ? remarkFull.Length / config.typeSpeed : 0f;
            hint.text = t >= remarkStart + writeTime + config.hintDelay ? hintFull : string.Empty;
        }

        int Letters(float elapsed, int length)
        {
            if (elapsed < 0f)
                return 0;
            if (config.typeSpeed <= 0f)
                return length;
            return Mathf.Min(length, Mathf.FloorToInt(elapsed * config.typeSpeed));
        }

        Color GradeColor(Grade value)
        {
            switch (value)
            {
                case Grade.S: return style.gradeS;
                case Grade.A: return style.gradeA;
                case Grade.B: return style.gradeB;
                case Grade.C: return style.gradeC;
                case Grade.D: return style.gradeD;
                default: return style.gradeF;
            }
        }

        static string FormatTime(float seconds)
        {
            int whole = Mathf.CeilToInt(Mathf.Max(0f, seconds));
            return $"{whole / SecondsPerMinute}:{whole % SecondsPerMinute:00}";
        }

        void Build()
        {
            canvasRoot = new GameObject("ReportCardCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
            canvasRoot.transform.SetParent(transform, false);
            var canvas = canvasRoot.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = style.sortingOrder;
            var scaler = canvasRoot.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = style.referenceResolution;
            scaler.matchWidthOrHeight = 1f;
            group = canvasRoot.GetComponent<CanvasGroup>();
            group.blocksRaycasts = false;

            Image backdrop = CreateImage("Backdrop", canvasRoot.transform, style.backdropColor);
            backdrop.rectTransform.anchorMin = Vector2.zero;
            backdrop.rectTransform.anchorMax = Vector2.one;
            backdrop.rectTransform.sizeDelta = Vector2.zero;

            Image card = CreateImage("Card", canvasRoot.transform, style.cardColor);
            card.rectTransform.sizeDelta = style.cardSize;
            var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(style.padding, style.padding, style.padding, style.padding);
            layout.spacing = style.spacing;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            Transform parent = card.transform;
            CreateText(parent, "Header", style.headerFontSize, style.headerHeight, style.mutedInkColor).text = style.header;
            title = CreateText(parent, "Title", style.titleFontSize, style.titleHeight, style.inkColor);
            stats = CreateText(parent, "Stats", style.statsFontSize, style.statsHeight, style.inkColor);
            grade = CreateText(parent, "Grade", style.gradeFontSize, style.gradeHeight, style.inkColor);
            grade.fontStyle = FontStyle.Bold;
            pay = CreateText(parent, "Pay", style.payFontSize, style.payHeight, style.inkColor);
            remark = CreateText(parent, "Remark", style.remarkFontSize, style.remarkHeight, style.inkColor);
            remark.fontStyle = FontStyle.Italic;
            signature = CreateText(parent, "Signature", style.signatureFontSize, style.signatureHeight, style.mutedInkColor);
            signature.text = style.signature;
            signature.alignment = TextAnchor.MiddleRight;
            hint = CreateText(parent, "Hint", style.hintFontSize, style.hintHeight, style.hintColor);
            CreateText(parent, "Footer", style.footerFontSize, style.footerHeight, style.mutedInkColor).text = style.footer;

            canvasRoot.SetActive(false);
        }

        Image CreateImage(string objectName, Transform parent, Color color)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        Text CreateText(Transform parent, string objectName, int size, float height, Color color)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(Text), typeof(LayoutElement));
            go.transform.SetParent(parent, false);
            go.GetComponent<LayoutElement>().preferredHeight = height;
            var text = go.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }
    }
}

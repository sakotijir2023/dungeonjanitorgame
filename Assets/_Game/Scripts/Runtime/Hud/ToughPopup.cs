using DungeonJanitor.Core;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Hud
{
    /// <summary>
    /// Shows "Too tough for your Vacuum — upgrade needed" under the crosshair
    /// while you keep working a mess your tool can't handle. Listens to
    /// TooToughTouched only; uses real time so slow-motion doesn't stretch it.
    /// </summary>
    public sealed class ToughPopup : MonoBehaviour
    {
        const string BuiltinFontName = "LegacyRuntime.ttf";

        [SerializeField] HintConfig config;
        [SerializeField] ToughPopupStyle style;

        readonly ToughPopupTimer timer = new ToughPopupTimer();
        CanvasGroup group;
        Text text;
        string shownTool;

        void Awake() => Build();
        void OnEnable() => EventBus<TooToughTouched>.Subscribe(OnTouched);
        void OnDisable() => EventBus<TooToughTouched>.Unsubscribe(OnTouched);

        void OnTouched(TooToughTouched evt)
        {
            timer.Touch(Time.unscaledTime, config.toughContactGrace);
            if (evt.ToolName == shownTool)
                return;
            shownTool = evt.ToolName;
            text.text = string.Format(style.titleFormat, evt.ToolName) +
                        $"\n<size={style.subFontSize}><color=#{ColorUtility.ToHtmlStringRGBA(style.subColor)}>{style.subtitle}</color></size>";
        }

        void Update() => group.alpha = timer.Update(Time.unscaledTime, Time.unscaledDeltaTime, config);

        void Build()
        {
            var canvasGo = new GameObject("ToughPopupCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(CanvasGroup));
            canvasGo.transform.SetParent(transform, false);
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = style.sortingOrder;
            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = style.referenceResolution;
            scaler.matchWidthOrHeight = 1f;
            group = canvasGo.GetComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;

            var go = new GameObject("Text", typeof(RectTransform), typeof(Text), typeof(Shadow));
            var rect = (RectTransform)go.transform;
            rect.SetParent(canvasGo.transform, false);
            rect.sizeDelta = style.size;
            rect.anchoredPosition = style.offset;
            text = go.GetComponent<Text>();
            text.font = style.font != null ? style.font : Resources.GetBuiltinResource<Font>(BuiltinFontName);
            text.fontSize = style.fontSize;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = true;
            text.color = style.textColor;
            text.raycastTarget = false;
            var shadow = go.GetComponent<Shadow>();
            shadow.effectColor = style.shadowColor;
            shadow.effectDistance = style.shadowOffset;
        }
    }
}

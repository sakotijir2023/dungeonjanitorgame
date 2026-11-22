using System;
using System.Collections.Generic;
using DungeonJanitor.Config;
using DungeonJanitor.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DungeonJanitor.Tuning
{
    /// <summary>
    /// Live debug panel with a control for every value in every registered
    /// TuningConfig, plus named presets. Opens on TuningPanelToggleRequested.
    /// </summary>
    public sealed class TuningPanel : MonoBehaviour
    {
        [SerializeField] ConfigRegistry registry;
        [SerializeField] TuningPanelStyle style;

        readonly List<Action> refreshers = new List<Action>();
        TuningUIFactory factory;
        PresetStore store;
        GameObject canvasRoot;
        Selectable firstSelectable;
        string startSnapshot;
        bool visible;

        void Awake()
        {
            if (style == null)
                style = ScriptableObject.CreateInstance<TuningPanelStyle>();
            factory = new TuningUIFactory(style);
            store = new PresetStore(PresetStore.DefaultDirectory);
            startSnapshot = CaptureCurrent();
            Build();
            canvasRoot.SetActive(false);
        }

        void OnEnable() => EventBus<TuningPanelToggleRequested>.Subscribe(OnToggleRequested);
        void OnDisable() => EventBus<TuningPanelToggleRequested>.Unsubscribe(OnToggleRequested);

        void OnToggleRequested(TuningPanelToggleRequested _) => SetVisible(!visible);

        void SetVisible(bool show)
        {
            visible = show;
            canvasRoot.SetActive(show);
            if (show)
                RefreshAll();

            if (EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(show && firstSelectable != null ? firstSelectable.gameObject : null);

            EventBus<TuningPanelVisibilityChanged>.Raise(new TuningPanelVisibilityChanged { Visible = show });
        }

        string CaptureCurrent() => PresetSerializer.Serialize(registry.Configs);

        void ApplyPreset(string json)
        {
            PresetSerializer.Apply(json, registry.Configs);
            RefreshAll();
        }

        void RevertToStart() => ApplyPreset(startSnapshot);

        void RefreshAll()
        {
            foreach (Action refresh in refreshers)
                refresh();
        }

        void Build()
        {
            Canvas canvas = factory.CreateCanvas(transform);
            canvasRoot = canvas.gameObject;
            RectTransform panel = factory.CreatePanel(canvas.transform);
            factory.CreateText(panel, style.title, style.titleFontSize, style.textColor, -1f, style.headerHeight);

            var presets = new TuningPresetBar(panel, factory, style, store, CaptureCurrent, ApplyPreset, RevertToStart);
            firstSelectable = presets.FirstSelectable;

            RectTransform content = factory.CreateScrollView(panel, out ScrollRect scrollRect);
            scrollRect.gameObject.AddComponent<ScrollToSelected>();

            foreach (TuningConfig config in registry.Configs)
            {
                if (config == null)
                    continue;
                factory.CreateText(content, config.DisplayName, style.headerFontSize, style.headerColor, -1f, style.headerHeight);
                foreach (TuningField field in TuningField.Collect(config))
                {
                    if (field.Header != null)
                        factory.CreateText(content, field.Header, style.fontSize, style.subHeaderColor, -1f, style.rowHeight);
                    AddControl(content, field);
                }
            }
        }

        void AddControl(Transform parent, TuningField field)
        {
            RectTransform row = factory.CreateRow(parent, field.Label);
            factory.CreateText(row, field.Label, style.fontSize, style.textColor, style.labelWidth);

            if (field.Kind == TuningFieldKind.Bool)
            {
                Toggle toggle = factory.CreateToggle(row);
                toggle.SetIsOnWithoutNotify(field.GetValue() > 0.5f);
                toggle.onValueChanged.AddListener(isOn => field.SetValue(isOn ? 1f : 0f));
                refreshers.Add(() => toggle.SetIsOnWithoutNotify(field.GetValue() > 0.5f));
                return;
            }

            Slider slider = factory.CreateSlider(row, field.Min, field.Max, field.Kind == TuningFieldKind.Int);
            Text valueText = factory.CreateText(row, string.Empty, style.fontSize, style.textColor, style.valueWidth);
            valueText.alignment = TextAnchor.MiddleRight;

            void ShowValue() => valueText.text = field.GetValue().ToString(style.valueFormat);

            slider.SetValueWithoutNotify(field.GetValue());
            ShowValue();
            slider.onValueChanged.AddListener(value =>
            {
                field.SetValue(value);
                ShowValue();
            });
            refreshers.Add(() =>
            {
                slider.SetValueWithoutNotify(field.GetValue());
                ShowValue();
            });
        }
    }
}

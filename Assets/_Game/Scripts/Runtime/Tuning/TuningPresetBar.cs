using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Tuning
{
    /// <summary>
    /// Preset controls at the top of the tuning panel: name field, Save,
    /// Revert, and a list of saved presets (click to load, X to delete).
    /// </summary>
    public sealed class TuningPresetBar
    {
        const string SaveLabel = "Save";
        const string RevertLabel = "Revert";
        const string DeleteLabel = "X";
        const string PresetsHeader = "Presets";

        readonly TuningUIFactory factory;
        readonly TuningPanelStyle style;
        readonly PresetStore store;
        readonly Func<string> captureCurrent;
        readonly Action<string> applyPreset;
        readonly Action revert;
        readonly RectTransform listRoot;
        readonly InputField nameField;
        readonly List<GameObject> listRows = new List<GameObject>();

        public Selectable FirstSelectable { get; }

        public TuningPresetBar(Transform parent, TuningUIFactory factory, TuningPanelStyle style, PresetStore store,
            Func<string> captureCurrent, Action<string> applyPreset, Action revert)
        {
            this.factory = factory;
            this.style = style;
            this.store = store;
            this.captureCurrent = captureCurrent;
            this.applyPreset = applyPreset;
            this.revert = revert;

            factory.CreateText(parent, PresetsHeader, style.headerFontSize, style.headerColor, -1f, style.headerHeight);

            RectTransform row = factory.CreateRow(parent, "PresetControls");
            nameField = factory.CreateInputField(row, style.presetNamePlaceholder);
            factory.CreateButton(row, SaveLabel, Save, style.buttonWidth);
            factory.CreateButton(row, RevertLabel, () => this.revert(), style.buttonWidth);
            FirstSelectable = nameField;

            var list = new GameObject("PresetList", typeof(RectTransform), typeof(VerticalLayoutGroup));
            listRoot = (RectTransform)list.transform;
            listRoot.SetParent(parent, false);
            var layout = list.GetComponent<VerticalLayoutGroup>();
            layout.spacing = style.spacing;
            layout.childControlWidth = layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            RebuildList();
        }

        void Save()
        {
            string presetName = string.IsNullOrWhiteSpace(nameField.text)
                ? style.defaultPresetPrefix + DateTime.Now.ToString(style.defaultPresetTimeFormat)
                : nameField.text;
            string saved = store.Save(presetName, captureCurrent());
            nameField.text = saved;
            RebuildList();
        }

        void Load(string presetName)
        {
            if (store.TryLoad(presetName, out string json))
            {
                applyPreset(json);
                nameField.text = presetName;
            }
        }

        void Delete(string presetName)
        {
            store.Delete(presetName);
            RebuildList();
        }

        void RebuildList()
        {
            foreach (GameObject row in listRows)
                UnityEngine.Object.Destroy(row);
            listRows.Clear();

            foreach (string presetName in store.ListNames())
            {
                RectTransform row = factory.CreateRow(listRoot, "Preset_" + presetName);
                factory.CreateButton(row, presetName, () => Load(presetName), -1f);
                factory.CreateButton(row, DeleteLabel, () => Delete(presetName), style.smallButtonWidth);
                listRows.Add(row.gameObject);
            }
        }
    }
}

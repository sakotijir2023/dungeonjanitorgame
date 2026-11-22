using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Tuning
{
    /// <summary>
    /// Filters the tuning panel as you type. A row shows if its label or its
    /// config's name contains the search text; section and sub-headers show only
    /// when something under them does. Enter jumps to the first match.
    /// </summary>
    public sealed class TuningSearch
    {
        enum Kind { ConfigHeader, SubHeader, Control }

        struct Item
        {
            public GameObject Go;
            public Kind Kind;
            public string Text;
            public Selectable Control;
        }

        readonly List<Item> items = new List<Item>();
        readonly ScrollRect scrollRect;
        readonly Action<Selectable> select;
        string currentGroup = string.Empty;

        public TuningSearch(ScrollRect scrollRect, Action<Selectable> select)
        {
            this.scrollRect = scrollRect;
            this.select = select;
        }

        public void AddConfigHeader(GameObject go, string configName)
        {
            currentGroup = configName;
            items.Add(new Item { Go = go, Kind = Kind.ConfigHeader, Text = configName });
        }

        public void AddSubHeader(GameObject go, string text) =>
            items.Add(new Item { Go = go, Kind = Kind.SubHeader, Text = text });

        public void AddControl(GameObject row, string label, Selectable control) =>
            items.Add(new Item { Go = row, Kind = Kind.Control, Text = currentGroup + " " + label, Control = control });

        /// <summary>Show only matching rows (empty text shows everything).</summary>
        public void Apply(string query)
        {
            string needle = (query ?? string.Empty).Trim();
            bool all = needle.Length == 0;

            // Walk backwards so each header knows whether anything below it is visible.
            bool anyInSection = false;
            bool anyInSubSection = false;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                Item item = items[i];
                bool show;
                switch (item.Kind)
                {
                    case Kind.Control:
                        show = all || item.Text.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0;
                        anyInSection |= show;
                        anyInSubSection |= show;
                        break;
                    case Kind.SubHeader:
                        show = anyInSubSection;
                        anyInSubSection = false;
                        break;
                    default:
                        show = anyInSection;
                        anyInSection = false;
                        anyInSubSection = false;
                        break;
                }
                if (item.Go.activeSelf != show)
                    item.Go.SetActive(show);
            }
            scrollRect.verticalNormalizedPosition = 1f;
        }

        /// <summary>Select the first visible slider or toggle (so arrow keys take over).</summary>
        public void SelectFirstMatch()
        {
            foreach (Item item in items)
            {
                if (item.Kind == Kind.Control && item.Go.activeSelf && item.Control != null)
                {
                    select(item.Control);
                    return;
                }
            }
        }
    }
}

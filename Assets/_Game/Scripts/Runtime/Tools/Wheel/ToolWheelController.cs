using System.Collections.Generic;
using DungeonJanitor.Core;
using DungeonJanitor.Input;
using UnityEngine;

namespace DungeonJanitor.Tools.Wheel
{
    /// <summary>
    /// Hold the wheel button to open, point with mouse or right stick, release to
    /// pick. Talks to the rest of the game only through events: it announces
    /// open/close (slow-motion listens) and asks for a tool by id (ToolUser listens).
    /// </summary>
    public sealed class ToolWheelController : MonoBehaviour
    {
        [SerializeField] InputReader input;
        [SerializeField] ToolWheelConfig config;
        [SerializeField] ToolWheelLayout layout;
        [SerializeField] ToolWheelView view;
        [SerializeField] ToolWheelStyle style;

        readonly HashSet<string> available = new HashSet<string>();
        readonly Dictionary<int, int> rememberedOption = new Dictionary<int, int>();
        readonly List<WheelEntry> entries = new List<WheelEntry>();
        WheelNavigator navigator;
        Vector2 pointer;
        string equippedId;
        int shownHover = int.MinValue;
        bool open;

        void Awake()
        {
            navigator = new WheelNavigator(layout.Slots.Count, FolderOptionCount);
            view.Build(style, config);
        }

        void OnEnable()
        {
            EventBus<ToolsAvailable>.Subscribe(OnToolsAvailable);
            EventBus<ToolEquipped>.Subscribe(OnToolEquipped);
            EventBus<TuningPanelVisibilityChanged>.Subscribe(OnTuningPanel);
        }

        void OnDisable()
        {
            EventBus<ToolsAvailable>.Unsubscribe(OnToolsAvailable);
            EventBus<ToolEquipped>.Unsubscribe(OnToolEquipped);
            EventBus<TuningPanelVisibilityChanged>.Unsubscribe(OnTuningPanel);
            if (open)
                Close(false);
        }

        void OnToolsAvailable(ToolsAvailable evt)
        {
            available.Clear();
            foreach (string id in evt.ToolIds)
                available.Add(id);
        }

        void OnToolEquipped(ToolEquipped evt) => equippedId = evt.ToolId;

        void OnTuningPanel(TuningPanelVisibilityChanged evt)
        {
            if (evt.Visible && open)
                Close(false);
        }

        int FolderOptionCount(int slot)
        {
            if (slot < 0 || slot >= layout.Slots.Count)
                return 0;
            ToolWheelLayout.Slot data = layout.Slots[slot];
            return data.IsFolder ? data.folderOptions.Length : 0;
        }

        void Update()
        {
            bool held = input.ToolWheelHeld;
            if (!open && held)
                Open();
            else if (open && !held)
                Close(true);

            if (!open)
                return;

            Vector2? direction = ReadDirection();
            bool ringChanged = navigator.Update(direction, Time.unscaledDeltaTime, config.folderOpenDelay, config.slotStickiness);
            view.SetPointer(PointerFraction(direction));
            if (ringChanged)
                ShowRing();
            if (ringChanged || navigator.Hovered != shownHover)
            {
                shownHover = navigator.Hovered;
                view.SetHovered(shownHover, HoverText());
            }
        }

        Vector2? ReadDirection()
        {
            Vector2 look = input.Look;
            if (input.LookIsMouse)
            {
                pointer = Vector2.ClampMagnitude(pointer + look * config.mousePointerSpeed, config.pointerMaxDistance);
                return pointer.magnitude > config.mouseDeadzone ? pointer : (Vector2?)null;
            }
            return look.magnitude > config.stickDeadzone ? look : (Vector2?)null;
        }

        /// <summary>Where to draw the aiming dot: 0 = centre, 1 = the slot ring.</summary>
        Vector2 PointerFraction(Vector2? direction)
        {
            if (input.LookIsMouse || !direction.HasValue)
                return pointer / config.pointerMaxDistance;
            return Vector2.ClampMagnitude(direction.Value, 1f);
        }

        void Open()
        {
            open = true;
            pointer = Vector2.zero;
            navigator.Reset();
            ShowRing();
            shownHover = navigator.Hovered;
            view.SetHovered(shownHover, HoverText());
            view.Show();
            EventBus<ToolWheelVisibilityChanged>.Raise(new ToolWheelVisibilityChanged { Visible = true });
        }

        void Close(bool commit)
        {
            open = false;
            view.Hide();
            if (commit)
                Commit(navigator.Pick());
            EventBus<ToolWheelVisibilityChanged>.Raise(new ToolWheelVisibilityChanged { Visible = false });
        }

        void Commit(WheelPick pick)
        {
            if (pick.IsNone)
                return;
            ToolWheelLayout.Slot slot = layout.Slots[pick.Slot];
            ToolDefinition choice;
            if (!slot.IsFolder)
                choice = slot.tool;
            else
            {
                int option = pick.Option >= 0 ? pick.Option : Remembered(pick.Slot);
                choice = slot.folderOptions[option];
                if (IsAvailable(choice))
                    rememberedOption[pick.Slot] = option;
            }

            if (IsAvailable(choice))
                EventBus<ToolSelectRequested>.Raise(new ToolSelectRequested { ToolId = choice.id });
        }

        int Remembered(int slot) => rememberedOption.TryGetValue(slot, out int option) ? option : 0;

        bool IsAvailable(ToolDefinition tool) => tool != null && available.Contains(tool.id);

        void ShowRing()
        {
            entries.Clear();
            if (navigator.InFolder)
            {
                ToolWheelLayout.Slot folder = layout.Slots[navigator.FolderSlot];
                entries.Add(new WheelEntry { Label = style.backLabel, Available = true, IsBack = true });
                foreach (ToolDefinition option in folder.folderOptions)
                    entries.Add(ToolEntry(option));
            }
            else
            {
                for (int i = 0; i < layout.Slots.Count; i++)
                    entries.Add(SlotEntry(i));
            }
            view.ShowRing(entries, navigator.RingFirstAngle);
        }

        WheelEntry SlotEntry(int index)
        {
            ToolWheelLayout.Slot slot = layout.Slots[index];
            if (!slot.IsFolder)
                return ToolEntry(slot.tool);

            ToolDefinition last = slot.folderOptions[Remembered(index)];
            bool anyAvailable = false;
            bool holdsCurrent = false;
            foreach (ToolDefinition option in slot.folderOptions)
            {
                anyAvailable |= IsAvailable(option);
                holdsCurrent |= option != null && option.id == equippedId;
            }
            return new WheelEntry
            {
                Label = slot.folderName,
                Detail = last != null ? last.displayName : string.Empty,
                Available = anyAvailable,
                Current = holdsCurrent,
                IsFolder = true,
            };
        }

        WheelEntry ToolEntry(ToolDefinition tool) => new WheelEntry
        {
            Label = tool != null ? tool.displayName : string.Empty,
            Available = IsAvailable(tool),
            Current = tool != null && tool.id == equippedId,
        };

        string HoverText()
        {
            int hovered = navigator.Hovered;
            if (hovered < 0 || hovered >= entries.Count)
                return string.Empty;
            WheelEntry entry = entries[hovered];
            if (entry.IsFolder)
                return $"{entry.Label}\n<size={style.hintFontSize}>{style.folderHint}</size>";
            if (!entry.Available && !entry.IsBack)
                return $"{entry.Label}\n<size={style.hintFontSize}>{style.unavailableHint}</size>";
            return entry.Label;
        }
    }
}

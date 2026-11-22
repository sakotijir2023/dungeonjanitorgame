using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonJanitor.Tools.Wheel
{
    /// <summary>
    /// What sits in each wheel slot, clockwise from the top. A slot is either one
    /// tool or a folder of options. Adding a tool or folder option is a data change.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Tool Wheel Layout", fileName = "ToolWheelLayout")]
    public sealed class ToolWheelLayout : ScriptableObject
    {
        [Serializable]
        public sealed class Slot
        {
            public ToolDefinition tool;
            [Tooltip("Only for folders.")]
            public string folderName;
            [Tooltip("Fill this to make the slot a folder.")]
            public ToolDefinition[] folderOptions = Array.Empty<ToolDefinition>();

            public bool IsFolder => folderOptions != null && folderOptions.Length > 0;
        }

        [SerializeField] List<Slot> slots = new List<Slot>();

        public IReadOnlyList<Slot> Slots => slots;

        public void SetSlots(IEnumerable<Slot> newSlots) => slots = new List<Slot>(newSlots);
    }
}

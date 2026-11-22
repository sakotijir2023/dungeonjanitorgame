using System.Collections.Generic;
using DungeonJanitor.Core;
using DungeonJanitor.Input;
using UnityEngine;

namespace DungeonJanitor.Tools
{
    /// <summary>
    /// Holds the player's tools, keeps one equipped, and feeds it the Use and
    /// Secondary buttons. Equips on number keys, D-pad, or a ToolSelectRequested
    /// event from the tool wheel.
    /// </summary>
    public sealed class ToolUser : MonoBehaviour
    {
        [SerializeField] InputReader input;
        [SerializeField] List<ToolBase> tools = new List<ToolBase>();
        [SerializeField] int startingToolIndex;

        ToolBase equipped;

        public IReadOnlyList<ToolBase> Tools => tools;

        void OnEnable()
        {
            input.ToolSlotPressed += Equip;
            input.ToolCyclePressed += Cycle;
            EventBus<ToolSelectRequested>.Subscribe(OnSelectRequested);
        }

        void OnDisable()
        {
            input.ToolSlotPressed -= Equip;
            input.ToolCyclePressed -= Cycle;
            EventBus<ToolSelectRequested>.Unsubscribe(OnSelectRequested);
        }

        void OnSelectRequested(ToolSelectRequested evt)
        {
            int index = tools.FindIndex(t => IdOf(t) == evt.ToolId);
            if (index >= 0)
                Equip(index);
        }

        static string IdOf(ToolBase tool) => tool.Definition != null ? tool.Definition.id : tool.name;

        void Cycle(int direction)
        {
            if (tools.Count == 0)
                return;
            int current = Mathf.Max(0, tools.IndexOf(equipped));
            Equip((current + direction + tools.Count) % tools.Count);
        }

        void Start()
        {
            foreach (ToolBase tool in tools)
                tool.OnUnequipped();
            var ids = new string[tools.Count];
            for (int i = 0; i < tools.Count; i++)
                ids[i] = IdOf(tools[i]);
            EventBus<ToolsAvailable>.Raise(new ToolsAvailable { ToolIds = ids });
            Equip(startingToolIndex);
        }

        public void Equip(int index)
        {
            if (index < 0 || index >= tools.Count || tools[index] == equipped)
                return;
            if (equipped != null)
                equipped.OnUnequipped();
            equipped = tools[index];
            equipped.OnEquipped();
            EventBus<ToolEquipped>.Raise(new ToolEquipped { ToolId = IdOf(equipped) });
        }

        void Update()
        {
            if (equipped == null)
                return;
            equipped.SetPrimaryHeld(input.UseHeld);
            equipped.SetSecondaryHeld(input.SecondaryHeld);
        }
    }
}

using System.Collections.Generic;
using DungeonJanitor.Core;
using DungeonJanitor.Input;
using UnityEngine;

namespace DungeonJanitor.Tools
{
    /// <summary>
    /// Holds the player's tools, keeps one equipped, and feeds it the Use and
    /// Secondary buttons. The tool wheel will drive Equip later.
    /// </summary>
    public sealed class ToolUser : MonoBehaviour
    {
        [SerializeField] InputReader input;
        [SerializeField] List<ToolBase> tools = new List<ToolBase>();
        [SerializeField] int startingToolIndex;

        ToolBase equipped;

        public IReadOnlyList<ToolBase> Tools => tools;

        void Start()
        {
            foreach (ToolBase tool in tools)
                tool.OnUnequipped();
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
            string id = equipped.Definition != null ? equipped.Definition.id : equipped.name;
            EventBus<ToolEquipped>.Raise(new ToolEquipped { ToolId = id });
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

using DungeonJanitor.Core;
using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Tools
{
    /// <summary>Shared interface every cleaning or setup tool implements.</summary>
    public interface ITool
    {
        ToolDefinition Definition { get; }
        /// <summary>Hardest mess this tool can clean right now (upgrades raise it).</summary>
        float MaxHardness { get; }
        void OnEquipped();
        void OnUnequipped();
        void SetPrimaryHeld(bool held);
        void SetSecondaryHeld(bool held);
    }

    /// <summary>Default MonoBehaviour implementation: equipping shows the tool, unequipping hides it.</summary>
    public abstract class ToolBase : MonoBehaviour, ITool
    {
        [SerializeField] ToolDefinition definition;

        public ToolDefinition Definition => definition;
        public virtual float MaxHardness => 0f;
        protected bool PrimaryHeld { get; private set; }
        protected bool SecondaryHeld { get; private set; }

        public virtual void OnEquipped() => gameObject.SetActive(true);

        public virtual void OnUnequipped()
        {
            PrimaryHeld = false;
            SecondaryHeld = false;
            gameObject.SetActive(false);
        }

        public virtual void SetPrimaryHeld(bool held) => PrimaryHeld = held;

        /// <summary>
        /// Call while working on a mess. If it is beyond this tool, tells the
        /// player (via a popup) so they don't waste time. Returns true if too tough.
        /// </summary>
        protected bool ReportIfTooTough(Mess mess)
        {
            if (mess == null || mess.IsCleaned || mess.Hardness <= MaxHardness)
                return false;
            string toolName = definition != null ? definition.displayName : name;
            EventBus<TooToughTouched>.Raise(new TooToughTouched { ToolName = toolName, Family = mess.Family });
            return true;
        }
        public virtual void SetSecondaryHeld(bool held) => SecondaryHeld = held;
    }
}

using System;
using UnityEngine;

namespace DungeonJanitor.Config
{
    /// <summary>
    /// Base class for every feel config. Public float, int and bool fields show up
    /// as live controls in the tuning panel; [Range] sets the slider limits and
    /// [Header] groups them.
    /// </summary>
    public abstract class TuningConfig : ScriptableObject
    {
        /// <summary>Raised after the tuning panel or a preset changes a value.</summary>
        public event Action Changed;

        public virtual string DisplayName => name;

        public void NotifyChanged()
        {
            Changed?.Invoke();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}

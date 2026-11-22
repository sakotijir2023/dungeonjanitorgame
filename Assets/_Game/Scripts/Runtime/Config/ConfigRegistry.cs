using System.Collections.Generic;
using UnityEngine;

namespace DungeonJanitor.Config
{
    /// <summary>
    /// Every TuningConfig in the project. Kept up to date automatically by an
    /// editor script, so a new config appears in the tuning panel with no wiring.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config Registry", fileName = "ConfigRegistry")]
    public sealed class ConfigRegistry : ScriptableObject
    {
        [SerializeField] List<TuningConfig> configs = new List<TuningConfig>();

        public IReadOnlyList<TuningConfig> Configs => configs;

        public void SetConfigs(IEnumerable<TuningConfig> newConfigs)
        {
            configs = new List<TuningConfig>(newConfigs);
        }
    }
}

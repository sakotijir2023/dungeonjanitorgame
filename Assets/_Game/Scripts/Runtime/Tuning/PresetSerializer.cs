using System;
using System.Collections.Generic;
using System.Linq;
using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Tuning
{
    [Serializable]
    public sealed class PresetData
    {
        public List<PresetEntry> configs = new List<PresetEntry>();
    }

    [Serializable]
    public sealed class PresetEntry
    {
        public string configName;
        public string json;
    }

    /// <summary>
    /// Turns the current values of a set of configs into one JSON preset and back.
    /// Configs are matched by asset name; unknown entries are ignored.
    /// </summary>
    public static class PresetSerializer
    {
        public static string Serialize(IEnumerable<TuningConfig> configs)
        {
            var data = new PresetData();
            foreach (TuningConfig config in configs)
                data.configs.Add(new PresetEntry { configName = config.name, json = JsonUtility.ToJson(config) });
            return JsonUtility.ToJson(data, true);
        }

        /// <summary>Overwrites matching configs. Returns how many were applied.</summary>
        public static int Apply(string presetJson, IEnumerable<TuningConfig> configs)
        {
            PresetData data = JsonUtility.FromJson<PresetData>(presetJson);
            if (data?.configs == null)
                return 0;

            Dictionary<string, TuningConfig> byName = configs.ToDictionary(config => config.name);
            int applied = 0;
            foreach (PresetEntry entry in data.configs)
            {
                if (!byName.TryGetValue(entry.configName, out TuningConfig config))
                    continue;
                JsonUtility.FromJsonOverwrite(entry.json, config);
                config.NotifyChanged();
                applied++;
            }
            return applied;
        }
    }
}

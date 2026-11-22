using System.Collections.Generic;
using System.Linq;
using DungeonJanitor.Config;
using UnityEditor;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// Keeps every ConfigRegistry filled with all TuningConfig assets in the
    /// project, so new configs show up in the tuning panel automatically.
    /// </summary>
    public sealed class ConfigRegistryAutoFill : AssetPostprocessor
    {
        const string AssetExtension = ".asset";

        static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFrom)
        {
            bool touchedAssets = imported.Concat(deleted).Concat(moved)
                .Any(path => path.EndsWith(AssetExtension));
            if (touchedAssets)
                Refresh();
        }

        [MenuItem("Dungeon Janitor/Refresh Config Registry")]
        public static void Refresh()
        {
            List<TuningConfig> configs = FindAll<TuningConfig>()
                .OrderBy(config => config.name)
                .ToList();

            foreach (ConfigRegistry registry in FindAll<ConfigRegistry>())
            {
                if (registry.Configs.SequenceEqual(configs))
                    continue;
                registry.SetConfigs(configs);
                EditorUtility.SetDirty(registry);
                AssetDatabase.SaveAssetIfDirty(registry);
            }
        }

        static IEnumerable<T> FindAll<T>() where T : UnityEngine.Object
        {
            return AssetDatabase.FindAssets("t:" + typeof(T).Name)
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .Where(asset => asset != null);
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DungeonJanitor.Tuning
{
    /// <summary>
    /// Named preset files on disk. In the editor they live in
    /// Assets/_Game/Config/Presets so they can be committed; in a build they go
    /// to the player's persistent data folder.
    /// </summary>
    public sealed class PresetStore
    {
        const string FileExtension = ".json";
        const string MetaExtension = ".meta";
        const string EditorFolder = "_Game/Config/Presets";
        const string PlayerFolder = "Presets";

        readonly string directory;

        public PresetStore(string directory)
        {
            this.directory = directory;
        }

        public static string DefaultDirectory
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.dataPath, EditorFolder);
#else
                return Path.Combine(Application.persistentDataPath, PlayerFolder);
#endif
            }
        }

        public IReadOnlyList<string> ListNames()
        {
            if (!Directory.Exists(directory))
                return new List<string>();
            return Directory.GetFiles(directory, "*" + FileExtension)
                .Select(Path.GetFileNameWithoutExtension)
                .OrderBy(presetName => presetName)
                .ToList();
        }

        /// <summary>Saves under a cleaned-up name and returns that name.</summary>
        public string Save(string presetName, string json)
        {
            string clean = Sanitize(presetName);
            Directory.CreateDirectory(directory);
            File.WriteAllText(PathFor(clean), json);
            return clean;
        }

        public bool TryLoad(string presetName, out string json)
        {
            string path = PathFor(Sanitize(presetName));
            json = File.Exists(path) ? File.ReadAllText(path) : null;
            return json != null;
        }

        public void Delete(string presetName)
        {
            string path = PathFor(Sanitize(presetName));
            if (File.Exists(path))
                File.Delete(path);
            if (File.Exists(path + MetaExtension))
                File.Delete(path + MetaExtension);
        }

        public static string Sanitize(string presetName)
        {
            string trimmed = (presetName ?? string.Empty).Trim();
            foreach (char invalid in Path.GetInvalidFileNameChars())
                trimmed = trimmed.Replace(invalid, '_');
            return trimmed;
        }

        string PathFor(string cleanName) => Path.Combine(directory, cleanName + FileExtension);
    }
}

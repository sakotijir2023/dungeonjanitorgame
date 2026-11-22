using System;
using System.Collections.Generic;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>Which prefab to spawn for each mess family, and its hardness range for seeded runs.</summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Mess Catalog", fileName = "MessCatalog")]
    public sealed class MessCatalog : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public MessFamily family;
            public Mess prefab;
            [Min(0f)] public float minHardness;
            [Min(0f)] public float maxHardness = 1f;
        }

        [SerializeField] List<Entry> entries = new List<Entry>();

        public void SetEntries(IEnumerable<Entry> newEntries) => entries = new List<Entry>(newEntries);

        public bool TryGet(MessFamily family, out Entry entry)
        {
            entry = entries.Find(e => e.family == family && e.prefab != null);
            return entry != null;
        }
    }
}

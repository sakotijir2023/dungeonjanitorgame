using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    public enum MessLayoutMode
    {
        /// <summary>Every point spawns its fixed family and hardness. For testing.</summary>
        Fixed,
        /// <summary>Random families, amounts and hardness from a seed. For real runs.</summary>
        Seeded,
    }

    /// <summary>Spawns messes at the room's spawn points when the scene starts.</summary>
    public sealed class MessSpawner : MonoBehaviour
    {
        [SerializeField] MessCatalog catalog;
        [SerializeField] MessLayoutMode mode = MessLayoutMode.Fixed;
        [SerializeField] int seed = 1;
        [SerializeField, Range(0f, 1f)] float fillChance = 0.7f;
        [SerializeField] Transform spawnPointRoot;

        readonly List<Mess> spawned = new List<Mess>();

        void Start() => SpawnAll();

        public void SpawnAll()
        {
            MessSpawnPoint[] points = spawnPointRoot.GetComponentsInChildren<MessSpawnPoint>();
            if (mode == MessLayoutMode.Fixed)
            {
                foreach (MessSpawnPoint point in points)
                    Spawn(point, point.FixedFamily, point.FixedHardness, point.FixedVariant);
                return;
            }

            List<int> choices = points.Select(p => p.AllowedFamilies?.Length ?? 0).ToList();
            foreach (PlannedSpawn planned in MessLayoutPlanner.PlanSeeded(choices, seed, fillChance))
            {
                MessSpawnPoint point = points[planned.PointIndex];
                MessFamily family = point.AllowedFamilies[planned.ChoiceIndex];
                if (!catalog.TryGet(family, out MessCatalog.Entry entry))
                    continue;
                float hardness = Mathf.Lerp(entry.minHardness, entry.maxHardness, planned.HardnessRoll);
                Spawn(point, family, hardness, planned.VariantIndex(entry.variantCount));
            }
        }

        void Spawn(MessSpawnPoint point, MessFamily family, float hardness, int variant)
        {
            if (!catalog.TryGet(family, out MessCatalog.Entry entry))
            {
                Debug.LogWarning($"No prefab in the mess catalog for {family}.", this);
                return;
            }
            Mess mess = Instantiate(entry.prefab, point.transform.position, point.transform.rotation, transform);
            // Spawn points can be scaled to make bigger or smaller messes.
            mess.transform.localScale = Vector3.Scale(entry.prefab.transform.localScale, point.transform.lossyScale);
            mess.Configure(hardness, variant);
            spawned.Add(mess);
        }
    }
}

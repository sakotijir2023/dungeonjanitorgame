using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// The fixed goo test layout for the training room: a cluster of soft goo
    /// near the start, then harder goo further out, one at the vacuum's limit and
    /// one too tough to clean.
    /// </summary>
    public static class GooLayout
    {
        struct Point
        {
            public Vector3 Floor; public float Hardness;
            public Point(float x, float floorY, float z, float hardness) { Floor = new Vector3(x, floorY, z); Hardness = hardness; }
        }

        const float JumpBoxTop = 0.9f;
        const float LandingTop = 1f;

        static readonly Point[] Points =
        {
            // Soft (loose) cluster right in front of the start.
            new Point(-1f, 0f, 1f, 0f), new Point(-0.5f, 0f, 1.6f, 0f), new Point(0f, 0f, 1f, 0f),
            new Point(0.6f, 0f, 1.5f, 0f), new Point(1.1f, 0f, 0.8f, 0f),
            // Lightly stuck.
            new Point(-3f, 0f, -1f, 0.5f), new Point(3f, 0f, -1f, 0.5f), new Point(-3f, JumpBoxTop, 4f, 0.5f),
            // Medium.
            new Point(-2f, 0f, -4f, 1f), new Point(2f, 0f, -4f, 1f),
            // Hard.
            new Point(8f, 0f, 0f, 2f), new Point(-9f, 0f, 0f, 2f),
            // At the vacuum's limit (max hardness 3), on the stair landing.
            new Point(5f, LandingTop, 8f, 3f),
            // Too tough for the starting vacuum (needs an upgrade).
            new Point(9f, 0f, -9f, 5f),
        };

        public static void Build(MessCatalog catalog)
        {
            var root = new GameObject("MessSpawnPoints").transform;
            for (int i = 0; i < Points.Length; i++)
            {
                Point point = Points[i];
                var go = new GameObject($"Goo_{i:00}_H{point.Hardness}");
                go.transform.SetParent(root, false);
                go.transform.position = point.Floor + Vector3.up * GooAssetsBuilder.BlobRadius;
                go.AddComponent<MessSpawnPoint>().Setup(MessFamily.Goo, point.Hardness, new[] { MessFamily.Goo });
            }

            var spawner = new GameObject("MessSpawner").AddComponent<MessSpawner>();
            TestRoomBuilder.SetRef(spawner, "catalog", catalog);
            TestRoomBuilder.SetRef(spawner, "spawnPointRoot", root);
        }
    }
}

using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// The fixed test layout for the training room. Each family has easy
    /// messes near the start, harder ones further out, one at the starting
    /// tool's limit (hardness 3) and one too tough without upgrades.
    /// </summary>
    public static class MessLayout
    {
        struct Point
        {
            public MessFamily Family; public Vector3 Floor; public float Hardness; public float Size; public float Yaw;
            public Point(MessFamily family, float x, float floorY, float z, float hardness, float size = 1f, float yaw = 0f)
            { Family = family; Floor = new Vector3(x, floorY, z); Hardness = hardness; Size = size; Yaw = yaw; }
        }

        const MessFamily Goo = MessFamily.Goo;
        const MessFamily Stain = MessFamily.Stain;
        const float JumpBoxTop = 0.9f;
        const float LandingTop = 1f;

        static readonly Point[] Points =
        {
            // Goo: soft (loose) cluster right in front of the start.
            new Point(Goo, -1f, 0f, 1f, 0f), new Point(Goo, -0.5f, 0f, 1.6f, 0f), new Point(Goo, 0f, 0f, 1f, 0f),
            new Point(Goo, 0.6f, 0f, 1.5f, 0f), new Point(Goo, 1.1f, 0f, 0.8f, 0f),
            // Goo: lightly stuck, medium, hard.
            new Point(Goo, -3f, 0f, -1f, 0.5f), new Point(Goo, 3f, 0f, -1f, 0.5f), new Point(Goo, -3f, JumpBoxTop, 4f, 0.5f),
            new Point(Goo, -2f, 0f, -4f, 1f), new Point(Goo, 2f, 0f, -4f, 1f),
            new Point(Goo, 8f, 0f, 0f, 2f), new Point(Goo, -9f, 0f, 0f, 2f),
            // Goo: at the limit (stair landing), and too tough.
            new Point(Goo, 5f, LandingTop, 8f, 3f), new Point(Goo, 9f, 0f, -9f, 5f),

            // Stains: easy grime either side of the start, and a big light one at the back.
            new Point(Stain, -4.5f, 0f, 1.5f, 0f, 1.2f, 20f), new Point(Stain, 4.5f, 0f, 1.5f, 0f, 1f, 70f),
            new Point(Stain, 0f, 0f, 9.5f, 0.3f, 2.5f, 10f),
            // Stains: light, medium, hard.
            new Point(Stain, -9f, 0f, -5f, 0.5f, 1.5f, 45f), new Point(Stain, 3.5f, 0f, -3.5f, 0.5f, 1.1f, 120f),
            new Point(Stain, 6f, 0f, -6f, 1f, 1.6f, 200f), new Point(Stain, -3f, 0f, -9f, 1f, 1.3f, 300f),
            new Point(Stain, -9f, 0f, 8f, 2f, 2f, 80f),
            // Stains: at the limit, and too tough.
            new Point(Stain, 9.5f, 0f, 5f, 3f, 1.5f, 160f), new Point(Stain, -10f, 0f, -10f, 4f, 1.2f, 30f),
        };

        public static void Build(MessCatalog catalog)
        {
            var root = new GameObject("MessSpawnPoints").transform;
            for (int i = 0; i < Points.Length; i++)
            {
                Point point = Points[i];
                var go = new GameObject($"{point.Family}_{i:00}_H{point.Hardness}");
                go.transform.SetParent(root, false);
                float lift = point.Family == Goo ? GooAssetsBuilder.BlobRadius : 0f;
                go.transform.position = point.Floor + Vector3.up * lift;
                go.transform.rotation = Quaternion.Euler(0f, point.Yaw, 0f);
                go.transform.localScale = new Vector3(point.Size, 1f, point.Size);
                go.AddComponent<MessSpawnPoint>().Setup(point.Family, point.Hardness, new[] { point.Family });
            }

            var spawner = new GameObject("MessSpawner").AddComponent<MessSpawner>();
            TestRoomBuilder.SetRef(spawner, "catalog", catalog);
            TestRoomBuilder.SetRef(spawner, "spawnPointRoot", root);
        }
    }
}

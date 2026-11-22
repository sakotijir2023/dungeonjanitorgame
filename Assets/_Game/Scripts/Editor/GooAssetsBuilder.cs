using DungeonJanitor.Messes;
using UnityEditor;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>Creates the goo config, materials, prefab and the mess catalog.</summary>
    public static class GooAssetsBuilder
    {
        public const string GooConfigPath = "Assets/_Game/Config/GooConfig.asset";
        const string PrefabPath = "Assets/_Game/Prefabs/Messes/Goo.prefab";
        const string MaterialPath = "Assets/_Game/Art/Materials/Goo.mat";
        const string PhysicsMaterialPath = "Assets/_Game/Art/Materials/Goo.physicMaterial";

        const float BlobDiameter = 0.45f;
        const float ColliderRadius = 0.5f;
        const float GooSmoothness = 0.85f;
        const float GooBounciness = 0.1f;
        const float GooFriction = 0.8f;
        const float SeededMinHardness = 0f;
        const float SeededMaxHardness = 2.5f;

        public static float BlobRadius => BlobDiameter * 0.5f;

        /// <summary>Builds the goo prefab and returns its catalog entry.</summary>
        public static MessCatalog.Entry Build()
        {
            var config = EditorAssets.LoadOrCreate<GooConfig>(GooConfigPath);
            return new MessCatalog.Entry
            {
                family = MessFamily.Goo, prefab = BuildPrefab(config),
                minHardness = SeededMinHardness, maxHardness = SeededMaxHardness,
            };
        }

        static Mess BuildPrefab(GooConfig config)
        {
            Material material = EditorAssets.LoadOrCreateMaterial(MaterialPath, EditorAssets.LitShader, config.softColor, GooSmoothness);
            PhysicsMaterial physics = LoadOrCreatePhysicsMaterial();

            var root = new GameObject("Goo");
            root.transform.localScale = Vector3.one * BlobDiameter;

            var body = root.AddComponent<Rigidbody>();
            body.constraints = RigidbodyConstraints.FreezeRotation;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var collider = root.AddComponent<SphereCollider>();
            collider.radius = ColliderRadius;
            collider.sharedMaterial = physics;

            var mess = root.AddComponent<Mess>();
            EditorAssets.SetValue(mess, "family", p => p.enumValueIndex = (int)MessFamily.Goo);

            GameObject visual = EditorAssets.Primitive(PrimitiveType.Sphere, "Visual", root.transform, Vector3.zero, Vector3.one, material);
            var jiggle = visual.AddComponent<GooJiggle>();
            TestRoomBuilder.SetRef(jiggle, "config", config);
            TestRoomBuilder.SetRef(jiggle, "visualRenderer", visual.GetComponent<Renderer>());

            var blob = root.AddComponent<GooBlob>();
            TestRoomBuilder.SetRef(blob, "config", config);
            TestRoomBuilder.SetRef(blob, "jiggle", jiggle);
            TestRoomBuilder.SetRef(blob, "blobCollider", collider);

            EditorAssets.EnsureFolder(System.IO.Path.GetDirectoryName(PrefabPath));
            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            return saved.GetComponent<Mess>();
        }

        static PhysicsMaterial LoadOrCreatePhysicsMaterial()
        {
            var physics = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>(PhysicsMaterialPath);
            if (physics != null)
                return physics;
            physics = new PhysicsMaterial("Goo")
            {
                bounciness = GooBounciness,
                dynamicFriction = GooFriction,
                staticFriction = GooFriction,
                bounceCombine = PhysicsMaterialCombine.Minimum,
            };
            AssetDatabase.CreateAsset(physics, PhysicsMaterialPath);
            return physics;
        }
    }
}

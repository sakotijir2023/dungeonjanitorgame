using DungeonJanitor.Messes;
using UnityEditor;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>Creates the stain config, transparent decal material and stain prefab.</summary>
    public static class StainAssetsBuilder
    {
        const string ConfigPath = "Assets/_Game/Config/StainConfig.asset";
        const string PrefabPath = "Assets/_Game/Prefabs/Messes/Stain.prefab";
        const string MaterialPath = "Assets/_Game/Art/Materials/Stain.mat";

        const float DefaultSize = 1.4f;
        // Lift the decal off the floor to avoid flicker.
        const float DecalLift = 0.006f;
        const float TriggerHeight = 0.2f;
        const float StainSmoothness = 0.3f;
        const float SeededMinHardness = 0f;
        const float SeededMaxHardness = 2.5f;
        const int LookCount = 3;

        // URP transparent alpha-blend setup values.
        const float SurfaceTransparent = 1f;
        const float BlendAlpha = 0f;
        const int TransparentQueue = 3000;

        public static MessCatalog.Entry Build()
        {
            var config = EditorAssets.LoadOrCreate<StainConfig>(ConfigPath);
            return new MessCatalog.Entry
            {
                family = MessFamily.Stain, prefab = BuildPrefab(config),
                minHardness = SeededMinHardness, maxHardness = SeededMaxHardness, variantCount = LookCount,
            };
        }

        static Mess BuildPrefab(StainConfig config)
        {
            Material material = LoadOrCreateDecalMaterial();

            var root = new GameObject("Stain");
            root.transform.localScale = new Vector3(DefaultSize, 1f, DefaultSize);

            var trigger = root.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1f, TriggerHeight, 1f);
            trigger.center = new Vector3(0f, TriggerHeight * 0.5f, 0f);

            var mess = root.AddComponent<Mess>();
            EditorAssets.SetValue(mess, "family", p => p.enumValueIndex = (int)MessFamily.Stain);

            GameObject decal = EditorAssets.Primitive(PrimitiveType.Quad, "Decal", root.transform,
                Vector3.up * DecalLift, Vector3.one, material, new Vector3(90f, 0f, 0f));
            decal.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            var stain = root.AddComponent<Stain>();
            TestRoomBuilder.SetRef(stain, "config", config);
            TestRoomBuilder.SetRef(stain, "decal", decal.GetComponent<Renderer>());

            EditorAssets.EnsureFolder(System.IO.Path.GetDirectoryName(PrefabPath));
            GameObject saved = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
            return saved.GetComponent<Mess>();
        }

        static Material LoadOrCreateDecalMaterial()
        {
            Material material = EditorAssets.LoadOrCreateMaterial(MaterialPath, EditorAssets.LitShader, Color.white, StainSmoothness);
            // Without this, URP keeps drawing shine on fully transparent areas, showing the square.
            material.SetFloat("_BlendModePreserveSpecular", 0f);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.SetFloat("_Smoothness", StainSmoothness);
            material.SetFloat("_Surface", SurfaceTransparent);
            material.SetFloat("_Blend", BlendAlpha);
            material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_SrcBlendAlpha", (float)UnityEngine.Rendering.BlendMode.One);
            material.SetFloat("_DstBlendAlpha", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.SetOverrideTag("RenderType", "Transparent");
            material.renderQueue = TransparentQueue;
            EditorUtility.SetDirty(material);
            return material;
        }
    }
}

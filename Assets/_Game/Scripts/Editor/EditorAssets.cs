using System.IO;
using UnityEditor;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>Small helpers for builders that create assets.</summary>
    public static class EditorAssets
    {
        public const string LitShader = "Universal Render Pipeline/Lit";
        public const string ParticleUnlitShader = "Universal Render Pipeline/Particles/Unlit";
        const string SmoothnessProperty = "_Smoothness";

        public static void EnsureFolder(string assetFolder)
        {
            string full = Path.Combine(Directory.GetParent(Application.dataPath).FullName, assetFolder);
            if (Directory.Exists(full))
                return;
            Directory.CreateDirectory(full);
            AssetDatabase.Refresh();
        }

        public static Material LoadOrCreateMaterial(string path, string shaderName, Color color, float smoothness)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
                return material;
            EnsureFolder(Path.GetDirectoryName(path));
            material = new Material(Shader.Find(shaderName)) { color = color };
            if (material.HasProperty(SmoothnessProperty))
                material.SetFloat(SmoothnessProperty, smoothness);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        public static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            EnsureFolder(Path.GetDirectoryName(path));
            return TestRoomBuilder.LoadOrCreate<T>(path);
        }

        public static GameObject Primitive(PrimitiveType type, string name, Transform parent, Vector3 localPosition,
            Vector3 localScale, Material material, Vector3 localEuler = default)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name;
            Object.DestroyImmediate(go.GetComponent<Collider>());
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localEulerAngles = localEuler;
            go.transform.localScale = localScale;
            go.GetComponent<Renderer>().sharedMaterial = material;
            return go;
        }

        /// <summary>First-person models shouldn't cast shadows onto the world.</summary>
        public static void DisableShadows(GameObject root)
        {
            foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>())
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        public static void SetValue(Object target, string field, System.Action<SerializedProperty> set)
        {
            var so = new SerializedObject(target);
            set(so.FindProperty(field));
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}

using DungeonJanitor.Messes;
using DungeonJanitor.Tools;
using DungeonJanitor.Tools.Mop;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>Builds the grey-box mop: hands under the camera, a stretchy handle and a floor head.</summary>
    public static class MopBuilder
    {
        const string ConfigPath = "Assets/_Game/Config/MopConfig.asset";
        const string DefinitionPath = "Assets/_Game/Config/Tools/Mop.asset";
        const string MaterialFolder = "Assets/_Game/Art/Materials";
        const string ToolId = "mop";
        const string ToolName = "Mop";

        // Grey-box layout (camera space for hands and rest, head space for the head parts; meters).
        static readonly Vector3 HandsOffset = new Vector3(0.3f, -0.34f, 0.5f);
        static readonly Vector3 HeadRestOffset = new Vector3(0.3f, -1.3f, 1.2f);
        static readonly Vector3 HandleScale = new Vector3(0.045f, 1f, 0.045f);
        static readonly Vector3 GripScale = new Vector3(0.055f, 0.04f, 0.055f);
        static readonly Vector3 HeadBlockScale = new Vector3(0.5f, 0.05f, 0.22f);
        static readonly Vector3 HeadBlockOffset = new Vector3(0f, 0.06f, 0f);
        static readonly Vector3 StrandsScale = new Vector3(0.62f, 0.06f, 0.36f);
        static readonly Vector3 StrandsOffset = new Vector3(0f, 0.02f, 0f);

        // Water droplets.
        const float DropLifetime = 0.45f;
        const float DropSpeedMin = 0.8f;
        const float DropSpeedMax = 2f;
        const float DropSize = 0.035f;
        const float DropGravity = 1.2f;
        const float DropSpreadAngle = 50f;
        const float DropEmitRadius = 0.25f;
        const int MaxDrops = 300;

        public static MopTool Build(Transform cameraTransform)
        {
            var config = EditorAssets.LoadOrCreate<MopConfig>(ConfigPath);
            ToolDefinition definition = ToolDefinitions.LoadOrCreate(DefinitionPath, ToolId, ToolName, MessFamily.Stain);

            Material handleMaterial = EditorAssets.LoadOrCreateMaterial($"{MaterialFolder}/Mop_Handle.mat",
                EditorAssets.LitShader, new Color(0.45f, 0.3f, 0.18f), 0.3f);
            Material headMaterial = EditorAssets.LoadOrCreateMaterial($"{MaterialFolder}/Mop_Head.mat",
                EditorAssets.LitShader, new Color(0.2f, 0.45f, 0.55f), 0.3f);
            Material strandsMaterial = EditorAssets.LoadOrCreateMaterial($"{MaterialFolder}/Mop_Strands.mat",
                EditorAssets.LitShader, new Color(0.82f, 0.8f, 0.72f), 0.1f);
            Material waterMaterial = EditorAssets.LoadOrCreateMaterial($"{MaterialFolder}/Mop_Water.mat",
                EditorAssets.ParticleUnlitShader, new Color(0.7f, 0.85f, 1f), 0f);

            var root = new GameObject(ToolName);
            root.transform.SetParent(cameraTransform, false);

            var hands = new GameObject("Hands").transform;
            hands.SetParent(root.transform, false);
            hands.localPosition = HandsOffset;
            EditorAssets.Primitive(PrimitiveType.Cylinder, "Grip", hands, Vector3.zero, GripScale, handleMaterial);

            var headRest = new GameObject("HeadRest").transform;
            headRest.SetParent(root.transform, false);
            headRest.localPosition = HeadRestOffset;

            GameObject handle = EditorAssets.Primitive(PrimitiveType.Cylinder, "Handle", root.transform, Vector3.zero, HandleScale, handleMaterial);

            var head = new GameObject("Head").transform;
            head.SetParent(root.transform, false);
            var headVisual = new GameObject("HeadVisual").transform;
            headVisual.SetParent(head, false);
            EditorAssets.Primitive(PrimitiveType.Cube, "Block", headVisual, HeadBlockOffset, HeadBlockScale, headMaterial);
            EditorAssets.Primitive(PrimitiveType.Cylinder, "Strands", headVisual, StrandsOffset, StrandsScale, strandsMaterial);

            ParticleSystem splash = BuildSplash(head, waterMaterial);
            EditorAssets.DisableShadows(hands.gameObject);
            EditorAssets.DisableShadows(handle);

            var tool = root.AddComponent<MopTool>();
            TestRoomBuilder.SetRef(tool, "definition", definition);
            TestRoomBuilder.SetRef(tool, "config", config);
            TestRoomBuilder.SetRef(tool, "aim", cameraTransform);
            TestRoomBuilder.SetRef(tool, "headRest", headRest);
            TestRoomBuilder.SetRef(tool, "head", head);

            var viewmodel = root.AddComponent<MopViewmodel>();
            TestRoomBuilder.SetRef(viewmodel, "config", config);
            TestRoomBuilder.SetRef(viewmodel, "tool", tool);
            TestRoomBuilder.SetRef(viewmodel, "hands", hands);
            TestRoomBuilder.SetRef(viewmodel, "handle", handle.transform);
            TestRoomBuilder.SetRef(viewmodel, "headVisual", headVisual);
            TestRoomBuilder.SetRef(viewmodel, "splash", splash);
            return tool;
        }

        static ParticleSystem BuildSplash(Transform parent, Material material)
        {
            var go = new GameObject("Splash");
            go.transform.SetParent(parent, false);
            go.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            var particles = go.AddComponent<ParticleSystem>();

            ParticleSystem.MainModule main = particles.main;
            main.startLifetime = DropLifetime;
            main.startSpeed = new ParticleSystem.MinMaxCurve(DropSpeedMin, DropSpeedMax);
            main.startSize = DropSize;
            main.gravityModifier = DropGravity;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.maxParticles = MaxDrops;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = DropSpreadAngle;
            shape.radius = DropEmitRadius;

            go.GetComponent<ParticleSystemRenderer>().sharedMaterial = material;
            return particles;
        }
    }
}

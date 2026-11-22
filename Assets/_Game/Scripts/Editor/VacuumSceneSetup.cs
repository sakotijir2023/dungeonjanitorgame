using DungeonJanitor.Config;
using DungeonJanitor.Input;
using DungeonJanitor.Messes;
using DungeonJanitor.Player;
using DungeonJanitor.Tools;
using DungeonJanitor.Tools.Vacuum;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// Adds the vacuum, tool holder, crosshair and goo spawn layout to the test room
    /// whenever it is built.
    /// </summary>
    [InitializeOnLoad]
    public static class VacuumSceneSetup
    {
        const string VacuumConfigPath = "Assets/_Game/Config/VacuumConfig.asset";
        const string VacuumDefinitionPath = "Assets/_Game/Config/Tools/Vacuum.asset";
        const string MaterialFolder = "Assets/_Game/Art/Materials";
        const string VacuumId = "vacuum";
        const string VacuumName = "Vacuum";

        // Grey-box viewmodel layout (camera space, meters).
        static readonly Vector3 ModelOffset = new Vector3(0.28f, -0.3f, 0.45f);
        static readonly Vector3 BodyScale = new Vector3(0.16f, 0.22f, 0.16f);
        static readonly Vector3 NozzleOffset = new Vector3(0f, 0f, 0.3f);
        static readonly Vector3 NozzleScale = new Vector3(0.1f, 0.09f, 0.1f);
        static readonly Vector3 SuctionPointOffset = new Vector3(0f, 0f, 0.4f);
        static readonly Vector3 CylinderAlongZ = new Vector3(90f, 0f, 0f);

        // Air particle look.
        const float AirLifetimeMin = 0.1f;
        const float AirLifetimeMax = 0.35f;
        const float AirSpeed = -10f;
        const float AirSize = 0.025f;
        const float AirConeRadius = 0.05f;
        const float AirConeLength = 3.5f;
        const float AirStretch = 3f;

        // Crosshair.
        const float CrosshairSize = 6f;
        static readonly Color CrosshairColor = new Color(1f, 1f, 1f, 0.8f);

        static VacuumSceneSetup()
        {
            TestRoomBuilderHooks.SceneBuilt += AddToScene;
        }

        static void AddToScene()
        {
            MessCatalog catalog = GooAssetsBuilder.Build();
            var config = EditorAssets.LoadOrCreate<VacuumConfig>(VacuumConfigPath);
            ToolDefinition definition = LoadOrCreateDefinition();
            ConfigRegistryAutoFill.Refresh();

            var look = Object.FindFirstObjectByType<FirstPersonLook>();
            Camera camera = look.GetComponentInChildren<Camera>();
            VacuumTool vacuum = BuildVacuum(camera.transform, config, definition);

            var toolUser = look.gameObject.AddComponent<ToolUser>();
            TestRoomBuilder.SetRef(toolUser, "input", look.GetComponent<InputReader>());
            EditorAssets.SetValue(toolUser, "tools", p =>
            {
                p.arraySize = 1;
                p.GetArrayElementAtIndex(0).objectReferenceValue = vacuum;
            });

            BuildCrosshair();
            GooLayout.Build(catalog);
        }

        static ToolDefinition LoadOrCreateDefinition()
        {
            var definition = EditorAssets.LoadOrCreate<ToolDefinition>(VacuumDefinitionPath);
            definition.id = VacuumId;
            definition.displayName = VacuumName;
            definition.handles = new[] { MessFamily.Goo };
            EditorUtility.SetDirty(definition);
            return definition;
        }

        static VacuumTool BuildVacuum(Transform cameraTransform, VacuumConfig config, ToolDefinition definition)
        {
            Material bodyMaterial = EditorAssets.LoadOrCreateMaterial($"{MaterialFolder}/Vacuum_Body.mat",
                EditorAssets.LitShader, new Color(0.25f, 0.55f, 0.6f), 0.4f);
            Material nozzleMaterial = EditorAssets.LoadOrCreateMaterial($"{MaterialFolder}/Vacuum_Nozzle.mat",
                EditorAssets.LitShader, new Color(0.15f, 0.15f, 0.17f), 0.3f);
            Material airMaterial = EditorAssets.LoadOrCreateMaterial($"{MaterialFolder}/Vacuum_Air.mat",
                EditorAssets.ParticleUnlitShader, new Color(0.85f, 0.95f, 1f), 0f);

            var root = new GameObject(VacuumName);
            root.transform.SetParent(cameraTransform, false);

            var model = new GameObject("Model").transform;
            model.SetParent(root.transform, false);
            model.localPosition = ModelOffset;

            EditorAssets.Primitive(PrimitiveType.Cylinder, "Body", model, Vector3.zero, BodyScale, bodyMaterial, CylinderAlongZ);
            GameObject nozzleVisual = EditorAssets.Primitive(PrimitiveType.Cylinder, "NozzleVisual", model,
                NozzleOffset, NozzleScale, nozzleMaterial, CylinderAlongZ);

            var suctionPoint = new GameObject("SuctionPoint").transform;
            suctionPoint.SetParent(model, false);
            suctionPoint.localPosition = SuctionPointOffset;

            ParticleSystem air = BuildAirParticles(suctionPoint, airMaterial, config.coneAngle);
            EditorAssets.DisableShadows(model.gameObject);

            var tool = root.AddComponent<VacuumTool>();
            TestRoomBuilder.SetRef(tool, "definition", definition);
            TestRoomBuilder.SetRef(tool, "config", config);
            TestRoomBuilder.SetRef(tool, "aim", cameraTransform);
            TestRoomBuilder.SetRef(tool, "nozzle", suctionPoint);
            TestRoomBuilder.SetRef(tool, "airParticles", air);

            var viewmodel = root.AddComponent<VacuumViewmodel>();
            TestRoomBuilder.SetRef(viewmodel, "config", config);
            TestRoomBuilder.SetRef(viewmodel, "tool", tool);
            TestRoomBuilder.SetRef(viewmodel, "model", model);
            TestRoomBuilder.SetRef(viewmodel, "nozzleVisual", nozzleVisual.transform);
            return tool;
        }

        static ParticleSystem BuildAirParticles(Transform parent, Material material, float coneAngle)
        {
            var go = new GameObject("AirParticles");
            go.transform.SetParent(parent, false);
            var particles = go.AddComponent<ParticleSystem>();

            ParticleSystem.MainModule main = particles.main;
            main.startLifetime = new ParticleSystem.MinMaxCurve(AirLifetimeMin, AirLifetimeMax);
            main.startSpeed = AirSpeed;
            main.startSize = AirSize;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;

            ParticleSystem.EmissionModule emission = particles.emission;
            emission.rateOverTime = 0f;

            ParticleSystem.ShapeModule shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.ConeVolume;
            shape.angle = coneAngle;
            shape.radius = AirConeRadius;
            shape.length = AirConeLength;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.sharedMaterial = material;
            renderer.renderMode = ParticleSystemRenderMode.Stretch;
            renderer.lengthScale = AirStretch;
            return particles;
        }

        static void BuildCrosshair()
        {
            var canvasGo = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler));
            canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

            var dot = new GameObject("Crosshair", typeof(RectTransform), typeof(Image));
            var rect = (RectTransform)dot.transform;
            rect.SetParent(canvasGo.transform, false);
            rect.sizeDelta = Vector2.one * CrosshairSize;
            var image = dot.GetComponent<Image>();
            image.color = CrosshairColor;
            image.raycastTarget = false;
        }
    }
}

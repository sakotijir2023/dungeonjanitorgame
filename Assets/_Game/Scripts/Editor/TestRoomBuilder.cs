using DungeonJanitor.Config;
using DungeonJanitor.Input;
using DungeonJanitor.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// Builds the grey-box janitor training room scene from scratch.
    /// Menu: Dungeon Janitor > Build Test Room. Safe to re-run; it overwrites the scene.
    /// </summary>
    public static class TestRoomBuilder
    {
        const string ScenePath = "Assets/_Game/Scenes/TestRoom.unity";
        const string MaterialFolder = "Assets/_Game/Art/Materials";
        const string MovementConfigPath = "Assets/_Game/Config/MovementConfig.asset";
        const string RegistryPath = "Assets/_Game/Config/ConfigRegistry.asset";
        const string InputActionsPath = "Assets/_Game/Input/DungeonJanitorInput.inputactions";
        const string LitShader = "Universal Render Pipeline/Lit";
        const string MainCameraTag = "MainCamera";

        // Grey-box layout data: name, position, scale, material key.
        struct Block
        {
            public string Name; public Vector3 Position; public Vector3 Scale; public Vector3 Euler; public string Material;
            public Block(string name, Vector3 position, Vector3 scale, string material, Vector3 euler = default)
            { Name = name; Position = position; Scale = scale; Material = material; Euler = euler; }
        }

        const float RoomSize = 24f;
        const float WallHeight = 4f;
        const float WallThickness = 0.5f;
        const float FloorThickness = 0.2f;

        static readonly Block[] Props =
        {
            new Block("Step_Low_0.3m", new Vector3(-6f, 0.15f, 4f), new Vector3(1.5f, 0.3f, 1.5f), "Prop"),
            new Block("Box_Jump_0.9m", new Vector3(-3f, 0.45f, 4f), new Vector3(1.5f, 0.9f, 1.5f), "Prop"),
            new Block("Box_Tall_1.6m", new Vector3(0f, 0.8f, 4f), new Vector3(1.5f, 1.6f, 1.5f), "Accent"),
            new Block("Stair_1", new Vector3(5f, 0.125f, 3f), new Vector3(2f, 0.25f, 1f), "Prop"),
            new Block("Stair_2", new Vector3(5f, 0.375f, 4f), new Vector3(2f, 0.25f, 1f), "Prop"),
            new Block("Stair_3", new Vector3(5f, 0.625f, 5f), new Vector3(2f, 0.25f, 1f), "Prop"),
            new Block("Stair_4", new Vector3(5f, 0.875f, 6f), new Vector3(2f, 0.25f, 1f), "Prop"),
            new Block("Landing", new Vector3(5f, 0.5f, 8f), new Vector3(4f, 1f, 3f), "Prop"),
            new Block("Ramp_20deg", new Vector3(-6f, 0.6f, -5f), new Vector3(2.5f, 0.2f, 6f), "Prop", new Vector3(-20f, 0f, 0f)),
            new Block("Beam_LowCeiling_2.2m", new Vector3(5f, 2.45f, -4f), new Vector3(4f, 0.5f, 0.5f), "Accent"),
            new Block("Pillar", new Vector3(0f, WallHeight * 0.5f, -6f), new Vector3(1f, WallHeight, 1f), "Wall"),
        };

        [MenuItem("Dungeon Janitor/Build Test Room")]
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            // Create the scene first: NewScene unloads unreferenced assets, which
            // would drop anything loaded before it.
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            MovementConfig movement = LoadOrCreate<MovementConfig>(MovementConfigPath);
            LoadOrCreate<ConfigRegistry>(RegistryPath);
            ConfigRegistryAutoFill.Refresh();
            var inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);

            BuildRoom();
            BuildLight();
            BuildPlayer(movement, inputActions);
            TestRoomBuilderHooks.RaiseSceneBuilt();

            EditorSceneManager.SaveScene(scene, ScenePath);
            AddToBuildSettings();
            Debug.Log($"Dungeon Janitor: test room built at {ScenePath}");
        }

        static void BuildRoom()
        {
            var root = new GameObject("Room").transform;
            float half = RoomSize * 0.5f;
            float wallY = WallHeight * 0.5f;

            CreateBlock(root, new Block("Floor", new Vector3(0f, -FloorThickness * 0.5f, 0f),
                new Vector3(RoomSize, FloorThickness, RoomSize), "Floor"));
            CreateBlock(root, new Block("Wall_North", new Vector3(0f, wallY, half), new Vector3(RoomSize, WallHeight, WallThickness), "Wall"));
            CreateBlock(root, new Block("Wall_South", new Vector3(0f, wallY, -half), new Vector3(RoomSize, WallHeight, WallThickness), "Wall"));
            CreateBlock(root, new Block("Wall_East", new Vector3(half, wallY, 0f), new Vector3(WallThickness, WallHeight, RoomSize), "Wall"));
            CreateBlock(root, new Block("Wall_West", new Vector3(-half, wallY, 0f), new Vector3(WallThickness, WallHeight, RoomSize), "Wall"));

            var props = new GameObject("Props").transform;
            props.SetParent(root);
            foreach (Block block in Props)
                CreateBlock(props, block);
        }

        static void CreateBlock(Transform parent, Block block)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = block.Name;
            go.transform.SetParent(parent);
            go.transform.SetPositionAndRotation(block.Position, Quaternion.Euler(block.Euler));
            go.transform.localScale = block.Scale;
            go.isStatic = true;
            go.GetComponent<MeshRenderer>().sharedMaterial = GetMaterial(block.Material);
        }

        static Material GetMaterial(string key)
        {
            string path = $"{MaterialFolder}/Greybox_{key}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
                return material;

            material = new Material(Shader.Find(LitShader)) { color = MaterialColor(key) };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        static Color MaterialColor(string key)
        {
            switch (key)
            {
                case "Floor": return new Color(0.22f, 0.22f, 0.25f);
                case "Wall": return new Color(0.38f, 0.37f, 0.40f);
                case "Accent": return new Color(0.35f, 0.45f, 0.38f);
                default: return new Color(0.55f, 0.52f, 0.50f);
            }
        }

        static void BuildLight()
        {
            var go = new GameObject("Directional Light");
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.shadows = LightShadows.Soft;
            go.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        static void BuildPlayer(MovementConfig movement, InputActionAsset inputActions)
        {
            var player = new GameObject("Player");
            player.transform.position = new Vector3(0f, 0f, -2f);
            player.AddComponent<CharacterController>();
            var reader = player.AddComponent<InputReader>();
            var motor = player.AddComponent<FirstPersonMotor>();
            var look = player.AddComponent<FirstPersonLook>();
            player.AddComponent<CursorLock>();

            var pivot = new GameObject("CameraPivot").transform;
            pivot.SetParent(player.transform, false);
            pivot.localPosition = Vector3.up * movement.eyeHeight;

            var cameraGo = new GameObject("Main Camera") { tag = MainCameraTag };
            cameraGo.transform.SetParent(pivot, false);
            var camera = cameraGo.AddComponent<Camera>();
            cameraGo.AddComponent<AudioListener>();

            SetRef(reader, "actions", inputActions);
            SetRef(motor, "config", movement);
            SetRef(motor, "input", reader);
            SetRef(look, "config", movement);
            SetRef(look, "input", reader);
            SetRef(look, "cameraPivot", pivot);
            SetRef(look, "viewCamera", camera);
        }

        public static void SetRef(Object target, string field, Object value)
        {
            var so = new SerializedObject(target);
            so.FindProperty(field).objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
                return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            return asset;
        }

        static void AddToBuildSettings()
        {
            var scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            EditorBuildSettings.scenes = scenes;
        }
    }

    /// <summary>Lets other editor builders add to the test room (e.g. the tuning panel) without coupling.</summary>
    public static class TestRoomBuilderHooks
    {
        public static event System.Action SceneBuilt;
        internal static void RaiseSceneBuilt() => SceneBuilt?.Invoke();
    }
}

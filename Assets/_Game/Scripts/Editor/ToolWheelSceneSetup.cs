using DungeonJanitor.Core;
using DungeonJanitor.Input;
using DungeonJanitor.Messes;
using DungeonJanitor.Tools;
using DungeonJanitor.Tools.Wheel;
using UnityEditor;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// Adds the tool wheel and slow-motion to the test room whenever it is built.
    /// Also creates the wheel layout and placeholder definitions for tools that
    /// don't exist yet, so they show greyed out in the right slots.
    /// </summary>
    [InitializeOnLoad]
    public static class ToolWheelSceneSetup
    {
        const string ToolsFolder = "Assets/_Game/Config/Tools";
        const string SetupFolder = ToolsFolder + "/Setup";
        const string LayoutPath = "Assets/_Game/Config/ToolWheelLayout.asset";
        const string WheelConfigPath = "Assets/_Game/Config/ToolWheelConfig.asset";
        const string SlowMotionConfigPath = "Assets/_Game/Config/SlowMotionConfig.asset";
        const string StylePath = "Assets/_Game/UI/ToolWheelStyle.asset";
        const string DiscSpritePath = "UI/Skin/Knob.psd";
        const string SetupFolderName = "Setup";

        static ToolWheelSceneSetup()
        {
            TestRoomBuilderHooks.SceneBuilt += AddToScene;
        }

        static void AddToScene()
        {
            ToolWheelLayout layout = BuildLayout();
            var config = EditorAssets.LoadOrCreate<ToolWheelConfig>(WheelConfigPath);
            var slowConfig = EditorAssets.LoadOrCreate<SlowMotionConfig>(SlowMotionConfigPath);
            ToolWheelStyle style = BuildStyle();
            ConfigRegistryAutoFill.Refresh();

            var wheelGo = new GameObject("ToolWheel");
            var view = wheelGo.AddComponent<ToolWheelView>();
            var controller = wheelGo.AddComponent<ToolWheelController>();
            TestRoomBuilder.SetRef(controller, "input", Object.FindFirstObjectByType<InputReader>());
            TestRoomBuilder.SetRef(controller, "config", config);
            TestRoomBuilder.SetRef(controller, "layout", layout);
            TestRoomBuilder.SetRef(controller, "view", view);
            TestRoomBuilder.SetRef(controller, "style", style);

            var slowMotion = new GameObject("SlowMotion").AddComponent<SlowMotion>();
            TestRoomBuilder.SetRef(slowMotion, "config", slowConfig);
        }

        static ToolWheelLayout BuildLayout()
        {
            ToolDefinition Tool(string fileName, string id, string displayName, MessFamily handles) =>
                ToolDefinitions.LoadOrCreate($"{ToolsFolder}/{fileName}.asset", id, displayName, handles);
            ToolDefinition Setup(string fileName, string id, string displayName) =>
                ToolDefinitions.LoadOrCreate($"{SetupFolder}/{fileName}.asset", id, displayName, MessFamily.Setup);

            // Slot order is clockwise from the top and matches number keys 1-8.
            ToolDefinition[] tools =
            {
                AssetDatabase.LoadAssetAtPath<ToolDefinition>(VacuumBuilder.DefinitionPath),
                AssetDatabase.LoadAssetAtPath<ToolDefinition>(MopBuilder.DefinitionPath),
                Tool("TrashBag", "trash_bag", "Trash Bag", MessFamily.Bones),
                Tool("Neutralizer", "neutralizer", "Neutralizer", MessFamily.Magic),
                Tool("Duster", "duster", "Duster", MessFamily.Cobweb),
                Tool("PolishCloth", "polish_cloth", "Polish Cloth", MessFamily.Tarnish),
                Tool("CreatureCatcher", "creature_catcher", "Catcher", MessFamily.Creature),
            };
            ToolDefinition[] setupOptions =
            {
                Setup("TrapKit", "trap_kit", "Trap Kit"),
                Setup("ChestStocker", "chest_stocker", "Chest Stocker"),
                Setup("TorchRelighter", "torch_relighter", "Torch Relighter"),
                Setup("LightRepairKit", "light_repair_kit", "Light Repair"),
            };

            var layout = EditorAssets.LoadOrCreate<ToolWheelLayout>(LayoutPath);
            var slots = new ToolWheelLayout.Slot[tools.Length + 1];
            for (int i = 0; i < tools.Length; i++)
                slots[i] = new ToolWheelLayout.Slot { tool = tools[i] };
            slots[tools.Length] = new ToolWheelLayout.Slot { folderName = SetupFolderName, folderOptions = setupOptions };
            layout.SetSlots(slots);
            EditorUtility.SetDirty(layout);
            AssetDatabase.SaveAssets();
            return layout;
        }

        static ToolWheelStyle BuildStyle()
        {
            var style = EditorAssets.LoadOrCreate<ToolWheelStyle>(StylePath);
            if (style.disc == null)
            {
                style.disc = AssetDatabase.GetBuiltinExtraResource<Sprite>(DiscSpritePath);
                EditorUtility.SetDirty(style);
                AssetDatabase.SaveAssetIfDirty(style);
            }
            return style;
        }
    }
}

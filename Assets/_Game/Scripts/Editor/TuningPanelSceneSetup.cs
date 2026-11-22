using DungeonJanitor.Config;
using DungeonJanitor.Tuning;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace DungeonJanitor.Editor
{
    /// <summary>Adds the tuning panel and a UI EventSystem whenever the test room is built.</summary>
    [InitializeOnLoad]
    public static class TuningPanelSceneSetup
    {
        const string RegistryPath = "Assets/_Game/Config/ConfigRegistry.asset";
        const string StylePath = "Assets/_Game/UI/TuningPanelStyle.asset";

        static TuningPanelSceneSetup()
        {
            TestRoomBuilderHooks.SceneBuilt += AddToScene;
        }

        static void AddToScene()
        {
            var registry = AssetDatabase.LoadAssetAtPath<ConfigRegistry>(RegistryPath);
            var style = TestRoomBuilder.LoadOrCreate<TuningPanelStyle>(StylePath);

            var panelGo = new GameObject("TuningPanel");
            var panel = panelGo.AddComponent<TuningPanel>();
            TestRoomBuilder.SetRef(panel, "registry", registry);
            TestRoomBuilder.SetRef(panel, "style", style);

            var eventSystemGo = new GameObject("EventSystem", typeof(EventSystem));
            var module = eventSystemGo.AddComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }
    }
}

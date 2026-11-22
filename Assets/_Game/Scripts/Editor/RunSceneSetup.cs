using DungeonJanitor.Run;
using UnityEditor;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>Adds the shift clock, mess counter and run logic to the test room whenever it is built.</summary>
    [InitializeOnLoad]
    public static class RunSceneSetup
    {
        const string ConfigPath = "Assets/_Game/Config/RunConfig.asset";
        const string StylePath = "Assets/_Game/UI/RunHudStyle.asset";

        static RunSceneSetup()
        {
            TestRoomBuilderHooks.SceneBuilt += AddToScene;
        }

        static void AddToScene()
        {
            var config = EditorAssets.LoadOrCreate<RunConfig>(ConfigPath);
            var style = EditorAssets.LoadOrCreate<RunHudStyle>(StylePath);
            ConfigRegistryAutoFill.Refresh();

            var runGo = new GameObject("Run");
            var controller = runGo.AddComponent<RunController>();
            TestRoomBuilder.SetRef(controller, "config", config);

            var hud = runGo.AddComponent<RunHud>();
            TestRoomBuilder.SetRef(hud, "config", config);
            TestRoomBuilder.SetRef(hud, "style", style);
        }
    }
}

using DungeonJanitor.Hud;
using DungeonJanitor.Run;
using DungeonJanitor.Scoring;
using UnityEditor;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// Adds the shift clock, mess counter, run logic and the "too tough" popup
    /// to the test room whenever it is built.
    /// </summary>
    [InitializeOnLoad]
    public static class RunSceneSetup
    {
        const string ConfigPath = "Assets/_Game/Config/RunConfig.asset";
        const string StylePath = "Assets/_Game/UI/RunHudStyle.asset";
        const string HintConfigPath = "Assets/_Game/Config/HintConfig.asset";
        const string ToughStylePath = "Assets/_Game/UI/ToughPopupStyle.asset";
        const string ScoringConfigPath = "Assets/_Game/Config/ScoringConfig.asset";

        static RunSceneSetup()
        {
            TestRoomBuilderHooks.SceneBuilt += AddToScene;
        }

        static void AddToScene()
        {
            var config = EditorAssets.LoadOrCreate<RunConfig>(ConfigPath);
            var style = EditorAssets.LoadOrCreate<RunHudStyle>(StylePath);
            var hintConfig = EditorAssets.LoadOrCreate<HintConfig>(HintConfigPath);
            var toughStyle = EditorAssets.LoadOrCreate<ToughPopupStyle>(ToughStylePath);
            var scoringConfig = EditorAssets.LoadOrCreate<ScoringConfig>(ScoringConfigPath);
            ConfigRegistryAutoFill.Refresh();

            var runGo = new GameObject("Run");
            var controller = runGo.AddComponent<RunController>();
            TestRoomBuilder.SetRef(controller, "config", config);

            var scoreKeeper = runGo.AddComponent<ScoreKeeper>();
            TestRoomBuilder.SetRef(scoreKeeper, "config", scoringConfig);

            var hud = runGo.AddComponent<RunHud>();
            TestRoomBuilder.SetRef(hud, "config", config);
            TestRoomBuilder.SetRef(hud, "style", style);

            var popup = new GameObject("ToughPopup").AddComponent<ToughPopup>();
            TestRoomBuilder.SetRef(popup, "config", hintConfig);
            TestRoomBuilder.SetRef(popup, "style", toughStyle);
        }
    }
}

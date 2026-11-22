using DungeonJanitor.Inspection;
using UnityEditor;
using UnityEngine;

namespace DungeonJanitor.Editor
{
    /// <summary>Adds the Inspector's report card to the test room whenever it is built.</summary>
    [InitializeOnLoad]
    public static class InspectionSceneSetup
    {
        const string ConfigPath = "Assets/_Game/Config/InspectorConfig.asset";
        const string LinesPath = "Assets/_Game/Config/InspectorLines.asset";
        const string StylePath = "Assets/_Game/UI/ReportCardStyle.asset";

        static InspectionSceneSetup()
        {
            TestRoomBuilderHooks.SceneBuilt += AddToScene;
        }

        static void AddToScene()
        {
            var config = EditorAssets.LoadOrCreate<InspectorConfig>(ConfigPath);
            var style = EditorAssets.LoadOrCreate<ReportCardStyle>(StylePath);
            var lines = EditorAssets.LoadOrCreate<InspectorLines>(LinesPath);
            if (lines.IsEmpty)
            {
                InspectorDefaultLines.Fill(lines);
                EditorUtility.SetDirty(lines);
                AssetDatabase.SaveAssetIfDirty(lines);
            }
            ConfigRegistryAutoFill.Refresh();

            var card = new GameObject("ReportCard").AddComponent<ReportCard>();
            TestRoomBuilder.SetRef(card, "config", config);
            TestRoomBuilder.SetRef(card, "lines", lines);
            TestRoomBuilder.SetRef(card, "style", style);
        }
    }
}

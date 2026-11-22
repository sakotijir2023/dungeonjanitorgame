using DungeonJanitor.Input;
using DungeonJanitor.Messes;
using DungeonJanitor.Player;
using DungeonJanitor.Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DungeonJanitor.Editor
{
    /// <summary>
    /// Adds the cleaning game to the test room whenever it is built: mess
    /// prefabs and catalog, the player's tools, a crosshair and the mess layout.
    /// </summary>
    [InitializeOnLoad]
    public static class CleaningSceneSetup
    {
        const string CatalogPath = "Assets/_Game/Config/MessCatalog.asset";
        const float CrosshairSize = 6f;
        static readonly Color CrosshairColor = new Color(1f, 1f, 1f, 0.8f);

        static CleaningSceneSetup()
        {
            TestRoomBuilderHooks.SceneBuilt += AddToScene;
        }

        static void AddToScene()
        {
            MessCatalog catalog = BuildCatalog();

            var look = Object.FindFirstObjectByType<FirstPersonLook>();
            Transform cameraTransform = look.GetComponentInChildren<Camera>().transform;
            ToolBase[] tools =
            {
                VacuumBuilder.Build(cameraTransform),
                MopBuilder.Build(cameraTransform),
            };
            ConfigRegistryAutoFill.Refresh();

            var toolUser = look.gameObject.AddComponent<ToolUser>();
            TestRoomBuilder.SetRef(toolUser, "input", look.GetComponent<InputReader>());
            EditorAssets.SetValue(toolUser, "tools", p =>
            {
                p.arraySize = tools.Length;
                for (int i = 0; i < tools.Length; i++)
                    p.GetArrayElementAtIndex(i).objectReferenceValue = tools[i];
            });

            BuildCrosshair();
            MessLayout.Build(catalog);
        }

        static MessCatalog BuildCatalog()
        {
            var catalog = EditorAssets.LoadOrCreate<MessCatalog>(CatalogPath);
            catalog.SetEntries(new[] { GooAssetsBuilder.Build(), StainAssetsBuilder.Build() });
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            return catalog;
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

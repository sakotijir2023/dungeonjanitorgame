using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Tools
{
    /// <summary>Data describing a tool, used by the tool wheel. Adding a tool is a data change.</summary>
    [CreateAssetMenu(menuName = "Dungeon Janitor/Tool Definition", fileName = "Tool")]
    public sealed class ToolDefinition : ScriptableObject
    {
        public string id;
        public string displayName;
        public Sprite icon;
        public MessFamily[] handles;
    }
}

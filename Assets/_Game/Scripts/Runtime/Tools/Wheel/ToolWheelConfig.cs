using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Tools.Wheel
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Tool Wheel", fileName = "ToolWheelConfig")]
    public sealed class ToolWheelConfig : TuningConfig
    {
        [Header("Pointing")]
        [Tooltip("How far the wheel pointer moves per unit of mouse movement.")]
        [Range(0.1f, 5f)] public float mousePointerSpeed = 1f;
        [Tooltip("The pointer can't travel further than this from the centre, so changing your mind is quick.")]
        [Range(20f, 400f)] public float pointerMaxDistance = 120f;
        [Tooltip("Mouse pointer must leave this centre circle before a slot is picked.")]
        [Range(0f, 100f)] public float mouseDeadzone = 30f;
        [Tooltip("Stick tilt (0..1) needed to pick a slot. Letting go of the stick keeps the last pick.")]
        [Range(0.1f, 0.9f)] public float stickDeadzone = 0.45f;

        [Header("Setup folder")]
        [Tooltip("Real seconds to rest on the Setup slot (or Back) before it opens (or closes).")]
        [Range(0.05f, 1.5f)] public float folderOpenDelay = 0.3f;

        [Header("Look")]
        [Tooltip("Distance from screen centre to each slot (UI pixels at 1080p).")]
        [Range(100f, 500f)] public float ringRadius = 280f;
        [Tooltip("Slot circle size (UI pixels at 1080p).")]
        [Range(60f, 260f)] public float slotSize = 150f;
        [Tooltip("How much the hovered slot grows.")]
        [Range(1f, 1.6f)] public float hoverScale = 1.18f;
        [Tooltip("How quickly slots grow and shrink (higher = snappier).")]
        [Range(1f, 60f)] public float hoverSpeed = 18f;
        [Tooltip("Real seconds for the wheel to pop open.")]
        [Range(0f, 0.4f)] public float openTime = 0.08f;
        [Tooltip("Wheel size at the start of the pop-open.")]
        [Range(0.5f, 1f)] public float openStartScale = 0.85f;
    }
}

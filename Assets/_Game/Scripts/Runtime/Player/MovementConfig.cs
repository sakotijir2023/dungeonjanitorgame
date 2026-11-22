using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Player
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Movement", fileName = "MovementConfig")]
    public sealed class MovementConfig : TuningConfig
    {
        [Header("Ground movement")]
        [Tooltip("Walking speed (m/s).")]
        [Range(0.5f, 15f)] public float walkSpeed = 4.5f;
        [Tooltip("Sprinting speed (m/s).")]
        [Range(0.5f, 20f)] public float sprintSpeed = 7.5f;
        [Tooltip("How fast you reach full speed on the ground (m/s per second).")]
        [Range(1f, 200f)] public float groundAcceleration = 60f;
        [Tooltip("How fast you stop on the ground when you let go (m/s per second).")]
        [Range(1f, 200f)] public float groundDeceleration = 50f;

        [Header("Air and jumping")]
        [Tooltip("Steering strength while airborne (m/s per second).")]
        [Range(0f, 100f)] public float airAcceleration = 15f;
        [Tooltip("Downward pull (m/s per second). Real life is 9.81; games usually feel better higher.")]
        [Range(1f, 80f)] public float gravity = 25f;
        [Tooltip("Extra gravity while falling, for a snappier arc. 1 = same as going up.")]
        [Range(1f, 4f)] public float fallGravityMultiplier = 1.5f;
        [Tooltip("Peak jump height (m).")]
        [Range(0.1f, 4f)] public float jumpHeight = 1.2f;
        [Tooltip("Max falling speed (m/s).")]
        [Range(5f, 100f)] public float terminalVelocity = 40f;
        [Tooltip("Grace time to still jump after walking off a ledge (s).")]
        [Range(0f, 0.5f)] public float coyoteTime = 0.1f;
        [Tooltip("A jump pressed this long before landing still counts (s).")]
        [Range(0f, 0.5f)] public float jumpBufferTime = 0.1f;
        [Tooltip("Small downward push that keeps you glued to slopes and steps (m/s).")]
        [Range(0f, 10f)] public float groundStickSpeed = 2f;
        [Tooltip("Landings faster than this raise a landed event (m/s). Used later for thuds and shake.")]
        [Range(0f, 30f)] public float landedEventMinFallSpeed = 3f;

        [Header("Look")]
        [Tooltip("Mouse look speed (degrees per pixel of mouse movement).")]
        [Range(0.01f, 1f)] public float mouseSensitivity = 0.12f;
        [Tooltip("Stick look speed at full tilt (degrees per second).")]
        [Range(30f, 720f)] public float stickSensitivity = 200f;
        [Tooltip("Stick response curve. 1 = linear, higher = finer aim near center.")]
        [Range(1f, 4f)] public float stickResponseExponent = 2f;
        [Tooltip("Invert vertical look.")]
        public bool invertY;
        [Tooltip("Camera smoothing time (s). 0 = raw and instant.")]
        [Range(0f, 0.2f)] public float lookSmoothingTime = 0.02f;
        [Tooltip("How far you can look down (degrees).")]
        [Range(45f, 90f)] public float maxLookDown = 85f;
        [Tooltip("How far you can look up (degrees).")]
        [Range(45f, 90f)] public float maxLookUp = 85f;

        [Header("Body and camera")]
        [Tooltip("Eye height above the floor (m).")]
        [Range(0.5f, 2.5f)] public float eyeHeight = 1.6f;
        [Tooltip("Body height (m).")]
        [Range(0.8f, 2.5f)] public float bodyHeight = 1.8f;
        [Tooltip("Body radius (m).")]
        [Range(0.15f, 0.8f)] public float bodyRadius = 0.35f;
        [Tooltip("Tallest step you walk up without jumping (m).")]
        [Range(0f, 0.8f)] public float stepOffset = 0.35f;
        [Tooltip("Steepest walkable slope (degrees).")]
        [Range(10f, 80f)] public float slopeLimit = 45f;
        [Tooltip("Camera field of view (degrees, vertical).")]
        [Range(50f, 110f)] public float fieldOfView = 75f;
    }
}

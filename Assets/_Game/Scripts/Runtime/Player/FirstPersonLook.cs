using DungeonJanitor.Input;
using UnityEngine;

namespace DungeonJanitor.Player
{
    /// <summary>
    /// Mouse and stick look. Yaw turns the body, pitch tilts the camera pivot.
    /// Uses unscaled time so look stays responsive during slow-motion.
    /// </summary>
    public sealed class FirstPersonLook : MonoBehaviour
    {
        [SerializeField] MovementConfig config;
        [SerializeField] InputReader input;
        [SerializeField] Transform cameraPivot;
        [SerializeField] Camera viewCamera;

        float targetYaw;
        float targetPitch;
        float yaw;
        float pitch;

        void Start()
        {
            yaw = targetYaw = transform.eulerAngles.y;
            pitch = targetPitch = 0f;
        }

        void Update()
        {
            float dt = Time.unscaledDeltaTime;
            Vector2 degrees = ReadLookDegrees(dt);
            float verticalSign = config.invertY ? -1f : 1f;

            targetYaw += degrees.x;
            targetPitch -= degrees.y * verticalSign;
            targetPitch = Mathf.Clamp(targetPitch, -config.maxLookUp, config.maxLookDown);

            float blend = MovementMath.SmoothingBlend(config.lookSmoothingTime, dt);
            yaw = Mathf.Lerp(yaw, targetYaw, blend);
            pitch = Mathf.Lerp(pitch, targetPitch, blend);

            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            cameraPivot.localPosition = Vector3.up * config.eyeHeight;
            viewCamera.fieldOfView = config.fieldOfView;
        }

        Vector2 ReadLookDegrees(float dt)
        {
            Vector2 raw = input.Look;
            if (input.LookIsMouse)
                return raw * config.mouseSensitivity;

            Vector2 curved = new Vector2(
                MovementMath.ApplyResponseCurve(raw.x, config.stickResponseExponent),
                MovementMath.ApplyResponseCurve(raw.y, config.stickResponseExponent));
            return curved * (config.stickSensitivity * dt);
        }
    }
}

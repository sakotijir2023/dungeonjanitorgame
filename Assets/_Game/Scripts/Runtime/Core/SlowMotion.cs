using UnityEngine;

namespace DungeonJanitor.Core
{
    /// <summary>
    /// The only script that changes Time.timeScale. Eases into slow-motion
    /// while the tool wheel is open, using real (unscaled) time.
    /// </summary>
    public sealed class SlowMotion : MonoBehaviour
    {
        [SerializeField] SlowMotionConfig config;

        float baseFixedDeltaTime;
        float current = 1f;
        bool wheelOpen;

        void Awake() => baseFixedDeltaTime = Time.fixedDeltaTime;

        void OnEnable() => EventBus<ToolWheelVisibilityChanged>.Subscribe(OnWheel);

        void OnDisable()
        {
            EventBus<ToolWheelVisibilityChanged>.Unsubscribe(OnWheel);
            current = 1f;
            Apply();
        }

        void OnWheel(ToolWheelVisibilityChanged evt) => wheelOpen = evt.Visible;

        void Update()
        {
            float target = wheelOpen ? config.wheelTimeScale : 1f;
            float easeTime = wheelOpen ? config.easeInTime : config.easeOutTime;
            float span = Mathf.Abs(1f - config.wheelTimeScale);
            float step = easeTime > 0f ? span * Time.unscaledDeltaTime / easeTime : float.MaxValue;
            current = Mathf.MoveTowards(current, target, step);
            Apply();
        }

        void Apply()
        {
            Time.timeScale = current;
            bool scaleSteps = config == null || config.smoothPhysics;
            Time.fixedDeltaTime = scaleSteps ? baseFixedDeltaTime * current : baseFixedDeltaTime;
        }
    }
}

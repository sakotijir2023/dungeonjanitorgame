using DungeonJanitor.Player;
using UnityEngine;

namespace DungeonJanitor.Tools.Mop
{
    /// <summary>
    /// Mop visuals: stretches the handle from your hands to the head, leans the
    /// head into its movement, splashes water while scrubbing and bursts when a
    /// stain is finished.
    /// </summary>
    public sealed class MopViewmodel : MonoBehaviour
    {
        // Unity's cylinder primitive is 2 units tall.
        const float CylinderHeight = 2f;

        [SerializeField] MopConfig config;
        [SerializeField] MopTool tool;
        [SerializeField] Transform hands;
        [SerializeField] Transform handle;
        [SerializeField] Transform headVisual;
        [SerializeField] ParticleSystem splash;

        Vector3 handsRest;
        float kick;

        void Awake() => handsRest = hands.localPosition;
        void OnEnable() => tool.StainCleaned += OnStainCleaned;
        void OnDisable() => tool.StainCleaned -= OnStainCleaned;

        void OnStainCleaned(Vector3 position)
        {
            kick = config.cleanKick;
            var emit = new ParticleSystem.EmitParams { position = position, applyShapeToPosition = true };
            splash.Emit(emit, config.cleanBurstCount);
        }

        void LateUpdate()
        {
            kick = Mathf.Lerp(kick, 0f, MovementMath.SmoothingBlend(config.kickRecoverTime, Time.deltaTime));
            hands.localPosition = handsRest + Vector3.back * kick;

            StretchHandle(hands.position, tool.HeadPosition);
            LeanHead();

            ParticleSystem.EmissionModule emission = splash.emission;
            emission.rateOverTime = tool.TouchingMess ? config.splashRate * tool.ScrubFactor : 0f;
        }

        void StretchHandle(Vector3 from, Vector3 to)
        {
            Vector3 span = to - from;
            float length = span.magnitude;
            if (length <= 0f)
                return;
            handle.position = from + span * 0.5f;
            handle.rotation = Quaternion.FromToRotation(Vector3.up, span / length);
            Vector3 scale = handle.localScale;
            scale.y = length / CylinderHeight;
            handle.localScale = scale;
        }

        void LeanHead()
        {
            Vector3 up = tool.OnFloor ? tool.FloorNormal : Vector3.up;
            Vector3 flatVelocity = Vector3.ProjectOnPlane(tool.HeadVelocity, up);
            float speed = flatVelocity.magnitude;
            Quaternion lean = Quaternion.identity;
            if (speed > 0f)
            {
                float angle = Mathf.Min(speed * config.leanPerSpeed, config.maxLean);
                lean = Quaternion.AngleAxis(angle, Vector3.Cross(up, flatVelocity / speed));
            }
            headVisual.rotation = lean * Quaternion.FromToRotation(Vector3.up, up);
        }
    }
}

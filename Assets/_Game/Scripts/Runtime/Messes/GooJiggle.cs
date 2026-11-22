using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>
    /// Jelly squash-and-stretch for a goo blob's visual. A damped spring chases
    /// a target stretch along an axis, keeping volume roughly constant.
    /// </summary>
    public sealed class GooJiggle : MonoBehaviour
    {
        const string BaseColorProperty = "_BaseColor";
        // Never squash an axis thinner than this fraction, so the blob can't invert.
        const float MinAxisScale = 0.3f;

        [SerializeField] GooConfig config;
        [SerializeField] Renderer visualRenderer;

        static readonly int BaseColorId = Shader.PropertyToID(BaseColorProperty);

        Vector3 restScale;
        Vector3 axis = Vector3.up;
        float targetStretch;
        float stretch;
        float stretchVelocity;
        float sizeMultiplier = 1f;
        MaterialPropertyBlock block;

        void Awake()
        {
            restScale = transform.localScale;
            block = new MaterialPropertyBlock();
        }

        /// <summary>Stretch along a world direction. 0 = round, positive = longer along the axis.</summary>
        public void SetTarget(Vector3 worldAxis, float stretchAmount)
        {
            if (worldAxis.sqrMagnitude > 0f)
                axis = worldAxis.normalized;
            targetStretch = stretchAmount;
        }

        /// <summary>Instant jiggle. Positive stretches, negative squashes.</summary>
        public void Kick(float velocity) => stretchVelocity += velocity;

        /// <summary>Overall size, used to shrink into the nozzle.</summary>
        public void SetSizeMultiplier(float multiplier) => sizeMultiplier = multiplier;

        public void SetHardnessTint(float hardness)
        {
            float t = Mathf.Clamp01(hardness / config.hardnessForDarkest);
            block.SetColor(BaseColorId, Color.Lerp(config.softColor, config.hardColor, t));
            visualRenderer.SetPropertyBlock(block);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            float acceleration = config.springStiffness * (targetStretch - stretch) - config.springDamping * stretchVelocity;
            stretchVelocity += acceleration * dt;
            stretch += stretchVelocity * dt;

            float along = Mathf.Max(MinAxisScale, 1f + stretch);
            float across = 1f / Mathf.Sqrt(along);

            transform.rotation = Quaternion.FromToRotation(Vector3.up, axis);
            transform.localScale = Vector3.Scale(restScale, new Vector3(across, along, across)) * sizeMultiplier;
        }
    }
}

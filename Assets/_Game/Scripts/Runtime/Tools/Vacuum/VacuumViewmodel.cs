using DungeonJanitor.Messes;
using DungeonJanitor.Player;
using UnityEngine;

namespace DungeonJanitor.Tools.Vacuum
{
    /// <summary>
    /// Makes the vacuum in your hands feel alive: shakes with motor power,
    /// kicks back and bulges the nozzle on every gulp.
    /// </summary>
    public sealed class VacuumViewmodel : MonoBehaviour
    {
        // Offsets between the three noise channels so each axis shakes independently.
        const float NoiseSeedY = 17.3f;
        const float NoiseSeedZ = 41.7f;
        const float NoiseCenter = 0.5f;

        [SerializeField] VacuumConfig config;
        [SerializeField] VacuumTool tool;
        [SerializeField] Transform model;
        [SerializeField] Transform nozzleVisual;

        Vector3 restPosition;
        Vector3 nozzleRestScale;
        float kick;
        float bulge;

        void Awake()
        {
            restPosition = model.localPosition;
            nozzleRestScale = nozzleVisual.localScale;
        }

        void OnEnable() => tool.Swallowed += OnSwallowed;
        void OnDisable() => tool.Swallowed -= OnSwallowed;

        void OnSwallowed(Mess _)
        {
            kick = config.gulpKick;
            bulge = config.gulpBulge - 1f;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            float blend = MovementMath.SmoothingBlend(config.gulpRecoverTime, dt);
            kick = Mathf.Lerp(kick, 0f, blend);
            bulge = Mathf.Lerp(bulge, 0f, blend);

            float time = Time.time * config.shakeFrequency;
            Vector3 shake = new Vector3(
                Mathf.PerlinNoise(time, 0f) - NoiseCenter,
                Mathf.PerlinNoise(time, NoiseSeedY) - NoiseCenter,
                Mathf.PerlinNoise(time, NoiseSeedZ) - NoiseCenter) * (config.shakeAmount * tool.Power);

            model.localPosition = restPosition + shake + Vector3.back * kick;
            nozzleVisual.localScale = nozzleRestScale * (1f + bulge);
        }
    }
}

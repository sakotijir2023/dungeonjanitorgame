using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>
    /// Generates a splotchy stain shape as a square grid of 0..1 coverage values.
    /// Deterministic: the same seed always gives the same splotch.
    /// </summary>
    public static class StainShape
    {
        const float NoiseScale = 3.5f;
        const float AngleNoiseScale = 1.6f;
        const float SpeckleScale = 9f;
        const float SeedSpread = 97.13f;
        const float SeedWrap = 1000f;

        /// <param name="resolution">Grid width and height.</param>
        /// <param name="baseRadius">Average splotch radius as a fraction of the grid (0..0.5).</param>
        /// <param name="edgeWobble">How much the outline wobbles (fraction of the grid).</param>
        /// <param name="edgeSoftness">Width of the soft outer edge (fraction of the grid).</param>
        /// <param name="speckle">How patchy the inside is (0 = solid).</param>
        public static float[] Generate(int resolution, int seed, float baseRadius, float edgeWobble, float edgeSoftness, float speckle)
        {
            var coverage = new float[resolution * resolution];
            float offsetX = Mathf.Repeat(seed * SeedSpread, SeedWrap);
            float offsetY = Mathf.Repeat(seed * SeedSpread * 0.5f + SeedSpread, SeedWrap);
            float softness = Mathf.Max(edgeSoftness, 1f / resolution);

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float u = (x + 0.5f) / resolution - 0.5f;
                    float v = (y + 0.5f) / resolution - 0.5f;
                    float distance = Mathf.Sqrt(u * u + v * v);
                    float angle = Mathf.Atan2(v, u);

                    float outline = Mathf.PerlinNoise(offsetX + Mathf.Cos(angle) * AngleNoiseScale, offsetY + Mathf.Sin(angle) * AngleNoiseScale);
                    float radius = baseRadius + (outline - 0.5f) * 2f * edgeWobble;
                    float edge = Mathf.Clamp01((radius - distance) / softness);

                    float inner = Mathf.PerlinNoise(offsetX + u * SpeckleScale, offsetY + v * SpeckleScale);
                    float patchy = Mathf.Lerp(1f, inner, speckle);

                    float blob = Mathf.PerlinNoise(offsetX + u * NoiseScale, offsetY + v * NoiseScale);
                    coverage[y * resolution + x] = Mathf.Clamp01(edge * patchy * Mathf.Lerp(1f, blob + 0.5f, speckle));
                }
            }
            return coverage;
        }
    }
}

using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>
    /// The dirt left on a stain, as a square grid of 0..1 values. Scrubbing wipes
    /// dirt away under a round brush. Plain C# so it can be unit tested.
    /// </summary>
    public sealed class StainMask
    {
        readonly float[] dirt;
        readonly float initialTotal;
        float total;

        public int Resolution { get; }
        public float RemainingFraction => initialTotal > 0f ? total / initialTotal : 0f;

        public StainMask(int resolution, float[] coverage)
        {
            Resolution = resolution;
            dirt = (float[])coverage.Clone();
            foreach (float value in dirt)
                initialTotal += value;
            total = initialTotal;
        }

        public float this[int x, int y] => dirt[y * Resolution + x];

        /// <summary>
        /// Wipes dirt under a brush centered at uv (0..1). Each cell loses up to
        /// <paramref name="amount"/>, scaled down toward the brush edge.
        /// Returns how much dirt was removed (in cells' worth).
        /// </summary>
        public float Scrub(Vector2 uv, float radiusUv, float amount, float falloff)
        {
            if (radiusUv <= 0f || amount <= 0f)
                return 0f;

            float radiusCells = radiusUv * Resolution;
            float centerX = uv.x * Resolution - 0.5f;
            float centerY = uv.y * Resolution - 0.5f;
            int minX = Mathf.Max(0, Mathf.FloorToInt(centerX - radiusCells));
            int maxX = Mathf.Min(Resolution - 1, Mathf.CeilToInt(centerX + radiusCells));
            int minY = Mathf.Max(0, Mathf.FloorToInt(centerY - radiusCells));
            int maxY = Mathf.Min(Resolution - 1, Mathf.CeilToInt(centerY + radiusCells));

            float removed = 0f;
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    float dx = x - centerX;
                    float dy = y - centerY;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    if (distance >= radiusCells)
                        continue;

                    int index = y * Resolution + x;
                    float before = dirt[index];
                    if (before <= 0f)
                        continue;

                    float strength = Mathf.Pow(1f - distance / radiusCells, falloff);
                    float after = Mathf.Max(0f, before - amount * strength);
                    dirt[index] = after;
                    removed += before - after;
                }
            }
            total = Mathf.Max(0f, total - removed);
            return removed;
        }
    }
}

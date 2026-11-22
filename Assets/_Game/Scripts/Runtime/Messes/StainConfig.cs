using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    [CreateAssetMenu(menuName = "Dungeon Janitor/Config/Stain", fileName = "StainConfig")]
    public sealed class StainConfig : TuningConfig
    {
        [Header("Shape (new stains only)")]
        [Tooltip("Detail of the dirt map. Higher = finer wipe streaks.")]
        [Range(16, 128)] public int resolution = 64;
        [Tooltip("Average splotch size, as a fraction of the stain's square.")]
        [Range(0.1f, 0.5f)] public float baseRadius = 0.36f;
        [Tooltip("How lumpy the outline is.")]
        [Range(0f, 0.2f)] public float edgeWobble = 0.1f;
        [Tooltip("Soft edge width.")]
        [Range(0f, 0.2f)] public float edgeSoftness = 0.05f;
        [Tooltip("How patchy the inside looks. 0 = solid.")]
        [Range(0f, 1f)] public float speckle = 0.35f;

        [Header("Look")]
        [Tooltip("Opacity of fully dirty areas.")]
        [Range(0.1f, 1f)] public float opacity = 0.92f;
        [Tooltip("Hardness at which stains show their darkest color.")]
        [Range(0.1f, 10f)] public float hardnessForDarkest = 3f;
        public Color softColor = new Color(0.38f, 0.29f, 0.18f);
        public Color hardColor = new Color(0.06f, 0.05f, 0.05f);

        [Header("Finishing")]
        [Tooltip("When this little dirt is left (fraction), the stain counts as clean and fades away.")]
        [Range(0f, 0.3f)] public float cleanThreshold = 0.05f;
        [Tooltip("Time for the last bits to fade once clean (s).")]
        [Range(0.01f, 1f)] public float finishFadeTime = 0.3f;
    }
}

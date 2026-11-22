using System;
using DungeonJanitor.Config;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>One kind of stain: its colours from easy to tough, its shine and its shape tweaks.</summary>
    [Serializable]
    public struct StainLook
    {
        public string name;
        public Color softColor;
        public Color hardColor;
        [Range(0f, 1f)] public float smoothness;
        [Tooltip("Multiplies the shared edge softness (scorch marks fade out more).")]
        [Range(0f, 4f)] public float edgeSoftnessScale;
        [Tooltip("Multiplies the shared speckle (blood is splattery).")]
        [Range(0f, 3f)] public float speckleScale;
    }

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

        [Tooltip("Stain variants, picked by the mess variant number: 0 grime, 1 blood, 2 scorch.")]
        public StainLook[] looks =
        {
            new StainLook { name = "Grime", softColor = new Color(0.38f, 0.29f, 0.18f), hardColor = new Color(0.06f, 0.05f, 0.05f),
                smoothness = 0.3f, edgeSoftnessScale = 1f, speckleScale = 1f },
            new StainLook { name = "Blood", softColor = new Color(0.5f, 0.05f, 0.05f), hardColor = new Color(0.16f, 0.01f, 0.02f),
                smoothness = 0.75f, edgeSoftnessScale = 0.6f, speckleScale = 1.8f },
            new StainLook { name = "Scorch", softColor = new Color(0.14f, 0.12f, 0.1f), hardColor = new Color(0.02f, 0.02f, 0.02f),
                smoothness = 0.05f, edgeSoftnessScale = 2.5f, speckleScale = 0.5f },
        };

        /// <summary>The look for a variant number; wraps around if there are fewer looks.</summary>
        public StainLook LookFor(int variant) => looks[Mathf.Abs(variant) % looks.Length];

        [Header("Finishing")]
        [Tooltip("When this little dirt is left (fraction), the stain counts as clean and fades away.")]
        [Range(0f, 0.3f)] public float cleanThreshold = 0.05f;
        [Tooltip("Time for the last bits to fade once clean (s).")]
        [Range(0.01f, 1f)] public float finishFadeTime = 0.3f;
    }
}

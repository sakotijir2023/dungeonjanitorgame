using DungeonJanitor.Tools.Mop;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>
    /// A flat stain on the floor. Its dirt map is wiped exactly where the mop
    /// passes; when little enough is left it fades away and counts as clean.
    /// The stain's square is the root's X/Z scale.
    /// </summary>
    [RequireComponent(typeof(Mess))]
    public sealed class Stain : MonoBehaviour, IScrubTarget
    {
        const string BaseMapProperty = "_BaseMap";
        const string BaseColorProperty = "_BaseColor";
        // Shade variation: thin dirt is a bit lighter than thick dirt.
        const float ThinShade = 0.8f;
        const float ThickShade = 1.1f;
        // Turns a floor position into a repeatable per-stain seed.
        const float SeedFromX = 73.1f;
        const float SeedFromZ = 151.7f;

        static readonly int BaseMapId = Shader.PropertyToID(BaseMapProperty);
        static readonly int BaseColorId = Shader.PropertyToID(BaseColorProperty);

        [SerializeField] StainConfig config;
        [SerializeField] Renderer decal;

        Mess mess;
        StainMask mask;
        Texture2D texture;
        Color32[] pixels;
        Color tint;
        bool textureDirty;
        bool finishing;
        float finishElapsed;
        IScrubSource finisher;

        /// <summary>0..1 dirt left, for UI and tests.</summary>
        public float RemainingFraction => mask?.RemainingFraction ?? 1f;

        void Awake() => mess = GetComponent<Mess>();

        void Start()
        {
            int seed = Mathf.RoundToInt(transform.position.x * SeedFromX + transform.position.z * SeedFromZ);
            int resolution = config.resolution;
            float[] coverage = StainShape.Generate(resolution, seed, config.baseRadius, config.edgeWobble,
                config.edgeSoftness, config.speckle);
            mask = new StainMask(resolution, coverage);

            tint = Color.Lerp(config.softColor, config.hardColor, Mathf.Clamp01(mess.Hardness / config.hardnessForDarkest));
            texture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
            };
            pixels = new Color32[resolution * resolution];

            var block = new MaterialPropertyBlock();
            block.SetTexture(BaseMapId, texture);
            block.SetColor(BaseColorId, Color.white);
            decal.SetPropertyBlock(block);

            textureDirty = true;
        }

        public void Scrub(in ScrubContext context)
        {
            if (finishing || mask == null)
                return;

            Vector3 local = transform.InverseTransformPoint(context.Position);
            var uv = new Vector2(local.x + 0.5f, local.z + 0.5f);
            float radiusUv = context.Radius / Mathf.Max(transform.lossyScale.x, Mathf.Epsilon);

            float rate = CleaningMath.ProgressRate(context.CleaningSpeed, mess.Hardness, context.MaxHardness);
            float removed = mask.Scrub(uv, radiusUv, rate * context.DeltaTime, context.BrushFalloff);
            if (removed <= 0f)
                return;

            textureDirty = true;
            if (mask.RemainingFraction <= config.cleanThreshold)
            {
                finishing = true;
                finisher = context.Source;
            }
        }

        void LateUpdate()
        {
            if (mask == null)
                return;

            float fade = 1f;
            if (finishing)
            {
                finishElapsed += Time.deltaTime;
                fade = 1f - Mathf.Clamp01(finishElapsed / config.finishFadeTime);
                textureDirty = true;
            }

            if (textureDirty)
                UploadTexture(fade);

            if (finishing && fade <= 0f)
                Finish();
        }

        void UploadTexture(float fade)
        {
            textureDirty = false;
            int resolution = mask.Resolution;
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float dirt = mask[x, y];
                    Color color = tint * Mathf.Lerp(ThinShade, ThickShade, dirt);
                    color.a = dirt * config.opacity * fade;
                    pixels[y * resolution + x] = color;
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(false);
        }

        void Finish()
        {
            mess.Clean();
            finisher?.NotifyCleaned(mess, transform.position);
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            if (texture != null)
                Destroy(texture);
        }
    }
}

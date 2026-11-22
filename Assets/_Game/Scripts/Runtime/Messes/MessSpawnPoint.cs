using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>
    /// A place a mess can appear. The fixed test layout uses FixedFamily and
    /// FixedHardness; seeded runs pick from AllowedFamilies.
    /// </summary>
    public sealed class MessSpawnPoint : MonoBehaviour
    {
        const float GizmoRadius = 0.2f;

        [SerializeField] MessFamily fixedFamily = MessFamily.Goo;
        [SerializeField, Min(0f)] float fixedHardness;
        [SerializeField] MessFamily[] allowedFamilies = { MessFamily.Goo };

        public MessFamily FixedFamily => fixedFamily;
        public float FixedHardness => fixedHardness;
        public MessFamily[] AllowedFamilies => allowedFamilies;

        public void Setup(MessFamily family, float hardness, MessFamily[] allowed)
        {
            fixedFamily = family;
            fixedHardness = hardness;
            allowedFamilies = allowed;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, GizmoRadius);
        }
    }
}

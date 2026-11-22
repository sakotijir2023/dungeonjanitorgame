using System;
using DungeonJanitor.Messes;
using DungeonJanitor.Player;
using UnityEngine;

namespace DungeonJanitor.Tools.Mop
{
    /// <summary>
    /// Hold Use to put the mop head down where you aim and scrub. Moving the
    /// head cleans faster than holding it still.
    /// </summary>
    public sealed class MopTool : ToolBase, IScrubSource
    {
        const int MaxTargets = 32;
        // Keeps the head just above the floor so it never z-fights or clips.
        const float HeadHover = 0.02f;

        [SerializeField] MopConfig config;
        [SerializeField] Transform aim;
        [SerializeField] Transform headRest;
        [SerializeField] Transform head;

        readonly Collider[] overlapBuffer = new Collider[MaxTargets];
        Vector3 headPosition;
        Vector3 floorNormal = Vector3.up;
        bool initialized;

        public Vector3 HeadPosition => headPosition;
        public Vector3 HeadVelocity { get; private set; }
        public Vector3 FloorNormal => floorNormal;
        public bool OnFloor { get; private set; }
        /// <summary>True while the head is down and touching at least one stain.</summary>
        public bool TouchingMess { get; private set; }
        /// <summary>0..1 how vigorously you are scrubbing right now.</summary>
        public float ScrubFactor { get; private set; }

        /// <summary>Raised when a stain is finished, with its position.</summary>
        public event Action<Vector3> StainCleaned;

        public override void OnEquipped()
        {
            base.OnEquipped();
            initialized = false;
        }

        public override void OnUnequipped()
        {
            OnFloor = false;
            TouchingMess = false;
            base.OnUnequipped();
        }

        void Update()
        {
            float dt = Time.deltaTime;
            Vector3 target = FindHeadTarget();
            if (!initialized)
            {
                headPosition = target;
                initialized = true;
            }

            Vector3 previous = headPosition;
            headPosition = Vector3.Lerp(headPosition, target, MovementMath.SmoothingBlend(config.headFollowTime, dt));
            HeadVelocity = dt > 0f ? (headPosition - previous) / dt : Vector3.zero;
            head.position = headPosition;

            ScrubFactor = OnFloor ? ScrubMath.ScrubFactor(HeadVelocity.magnitude, config.speedForFullScrub, config.idleScrubFraction) : 0f;
            TouchingMess = OnFloor && ScrubTargets(dt);
        }

        Vector3 FindHeadTarget()
        {
            OnFloor = false;
            floorNormal = Vector3.up;
            if (PrimaryHeld && Physics.Raycast(aim.position, aim.forward, out RaycastHit hit, config.reach,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)
                && hit.normal.y >= config.minFloorFlatness)
            {
                OnFloor = true;
                floorNormal = hit.normal;
                return hit.point + hit.normal * HeadHover;
            }
            return headRest.position;
        }

        bool ScrubTargets(float dt)
        {
            int count = Physics.OverlapSphereNonAlloc(headPosition, config.headRadius, overlapBuffer,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
            var context = new ScrubContext(this, headPosition, config.headRadius, config.cleaningSpeed * ScrubFactor,
                config.maxHardness, config.brushFalloff, dt);

            bool touched = false;
            for (int i = 0; i < count; i++)
            {
                if (!overlapBuffer[i].TryGetComponent(out IScrubTarget target))
                    continue;
                target.Scrub(context);
                touched = true;
            }
            return touched;
        }

        public void NotifyCleaned(Mess mess, Vector3 position) => StainCleaned?.Invoke(position);
    }
}

using System;
using DungeonJanitor.Messes;
using DungeonJanitor.Player;
using UnityEngine;

namespace DungeonJanitor.Tools.Mop
{
    /// <summary>
    /// Tap Use for a hard scrub stroke where you aim; hold it to sweep side to
    /// side across where you aim. The motion itself lives in MopMotion; this
    /// script places the head on the floor and cleans what is under it.
    /// </summary>
    public sealed class MopTool : ToolBase, IScrubSource
    {
        const int MaxTargets = 32;
        // Keeps the head just above the floor so it never z-fights or clips.
        const float HeadHover = 0.02f;
        // How far above/below a swung-out head position to look for floor (m).
        const float FloorProbeHeight = 0.5f;

        [SerializeField] MopConfig config;
        [SerializeField] Transform aim;
        [SerializeField] Transform headRest;
        [SerializeField] Transform head;

        readonly Collider[] overlapBuffer = new Collider[MaxTargets];
        MopMotion motion;
        Vector3 headPosition;
        Vector3 floorNormal = Vector3.up;
        bool initialized;

        public Vector3 HeadPosition => headPosition;
        public Vector3 HeadVelocity { get; private set; }
        public Vector3 FloorNormal => floorNormal;
        public bool OnFloor { get; private set; }
        /// <summary>True while the head is down and touching at least one stain.</summary>
        public bool TouchingMess { get; private set; }
        /// <summary>0..1 how much water should fly right now.</summary>
        public float Splash => OnFloor ? motion.Splash : 0f;

        /// <summary>Raised when a stain is finished, with its position.</summary>
        public event Action<Vector3> StainCleaned;
        /// <summary>Raised at the peak of each stroke that lands on the floor, with the head position.</summary>
        public event Action<Vector3> StrokeLanded;

        void Awake() => motion = new MopMotion(config);

        public override void OnEquipped()
        {
            base.OnEquipped();
            initialized = false;
            motion.Reset();
        }

        public override void OnUnequipped()
        {
            OnFloor = false;
            TouchingMess = false;
            motion?.Reset();
            base.OnUnequipped();
        }

        public override void SetPrimaryHeld(bool held)
        {
            if (held && !PrimaryHeld)
                motion.Press();
            base.SetPrimaryHeld(held);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            motion.Tick(PrimaryHeld, dt);

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

            TouchingMess = OnFloor && motion.Power > 0f && ScrubTargets(dt);
            if (OnFloor && motion.StrokePeakedThisTick)
                StrokeLanded?.Invoke(headPosition);
        }

        Vector3 FindHeadTarget()
        {
            OnFloor = false;
            floorNormal = Vector3.up;
            bool active = motion.Current != MopMotion.Mode.Idle;
            if (!active || !RaycastFloor(aim.position, aim.forward, config.reach, out RaycastHit hit))
                return headRest.position;

            OnFloor = true;
            floorNormal = hit.normal;
            Vector3 forward = Vector3.ProjectOnPlane(aim.forward, hit.normal).normalized;
            Vector3 right = Vector3.Cross(hit.normal, forward);
            Vector3 swung = hit.point + right * motion.Offset.x + forward * motion.Offset.y;

            // Follow the floor under the swung-out head; if there is none (a wall
            // or a ledge), stay on the aim point instead.
            Vector3 point = RaycastFloor(swung + hit.normal * FloorProbeHeight, -hit.normal, FloorProbeHeight * 2f,
                out RaycastHit under) ? under.point : hit.point;
            return point + hit.normal * HeadHover;
        }

        bool RaycastFloor(Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
        {
            return Physics.Raycast(origin, direction, out hit, distance, Physics.DefaultRaycastLayers,
                       QueryTriggerInteraction.Ignore)
                   && hit.normal.y >= config.minFloorFlatness;
        }

        bool ScrubTargets(float dt)
        {
            int count = Physics.OverlapSphereNonAlloc(headPosition, motion.Radius, overlapBuffer,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
            var context = new ScrubContext(this, headPosition, motion.Radius, config.cleaningSpeed * motion.Power,
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

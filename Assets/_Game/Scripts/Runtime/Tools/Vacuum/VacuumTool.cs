using System;
using DungeonJanitor.Core;
using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Tools.Vacuum
{
    /// <summary>
    /// Hold Use to suck. Finds suckable things in a cone from the camera, checks
    /// line of sight, and pulls them toward the nozzle.
    /// </summary>
    public sealed class VacuumTool : ToolBase, ISuctionSource
    {
        const int MaxTargets = 128;

        [SerializeField] VacuumConfig config;
        [SerializeField] Transform aim;
        [SerializeField] Transform nozzle;
        [SerializeField] ParticleSystem airParticles;

        readonly Collider[] overlapBuffer = new Collider[MaxTargets];
        bool wasSucking;

        /// <summary>0..1 motor power, ramps with spin-up and spin-down.</summary>
        public float Power { get; private set; }

        /// <summary>Raised each time a mess goes down the nozzle.</summary>
        public event Action<Mess> Swallowed;

        public override void OnUnequipped()
        {
            Power = 0f;
            SetSucking(false);
            UpdateParticles();
            base.OnUnequipped();
        }

        void Update()
        {
            float target = PrimaryHeld ? 1f : 0f;
            float rampTime = PrimaryHeld ? config.spinUpTime : config.spinDownTime;
            Power = rampTime <= 0f ? target : Mathf.MoveTowards(Power, target, Time.deltaTime / rampTime);
            SetSucking(PrimaryHeld);
            UpdateParticles();
        }

        void FixedUpdate()
        {
            if (Power <= 0f)
                return;

            Vector3 origin = aim.position;
            Vector3 forward = aim.forward;
            int count = Physics.OverlapSphereNonAlloc(origin, config.range, overlapBuffer, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                ISuctionTarget target = FindTarget(overlapBuffer[i]);
                if (target == null || !target.CanBeSucked)
                    continue;

                Vector3 toTarget = target.SuctionPoint - origin;
                float distance = toTarget.magnitude;
                if (Vector3.Angle(forward, toTarget) > config.coneAngle)
                    continue;
                if (!HasLineOfSight(origin, toTarget, distance, target))
                    continue;

                float strength = SuctionMath.DistanceFalloff(distance, config.range, config.distanceFalloff) * Power;
                if (strength <= 0f)
                    continue;

                target.ReceiveSuction(new SuctionContext(this, nozzle, strength, config.pullAcceleration,
                    config.lateralDamping, config.cleaningSpeed, config.maxHardness, config.swallowRadius,
                    Time.fixedDeltaTime));
            }
        }

        public void NotifySwallowed(Mess mess) => Swallowed?.Invoke(mess);

        static ISuctionTarget FindTarget(Collider collider)
        {
            if (collider.attachedRigidbody != null && collider.attachedRigidbody.TryGetComponent(out ISuctionTarget fromBody))
                return fromBody;
            return collider.TryGetComponent(out ISuctionTarget fromCollider) ? fromCollider : null;
        }

        static bool HasLineOfSight(Vector3 origin, Vector3 toTarget, float distance, ISuctionTarget target)
        {
            if (!Physics.Raycast(origin, toTarget / distance, out RaycastHit hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return true;
            return hit.rigidbody != null && hit.rigidbody == target.Body;
        }

        void SetSucking(bool sucking)
        {
            if (sucking == wasSucking)
                return;
            wasSucking = sucking;
            EventBus<VacuumSuctionChanged>.Raise(new VacuumSuctionChanged { Active = sucking });
        }

        void UpdateParticles()
        {
            if (airParticles == null)
                return;
            ParticleSystem.EmissionModule emission = airParticles.emission;
            emission.rateOverTime = config.particleRate * Power;
        }
    }
}

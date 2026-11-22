using DungeonJanitor.Core;
using DungeonJanitor.Tools.Vacuum;
using UnityEngine;

namespace DungeonJanitor.Messes
{
    /// <summary>
    /// A goo blob. Stuck goo has to be loosened (it stretches and wobbles toward
    /// the nozzle while it resists), then breaks free, flies at the nozzle and
    /// gets swallowed. Soft goo (hardness 0) starts loose.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(Mess))]
    public sealed class GooBlob : MonoBehaviour, ISuctionTarget
    {
        enum State { Stuck, Loose, Swallowing }

        const float SuctionGraceSteps = 2f;

        [SerializeField] GooConfig config;
        [SerializeField] GooJiggle jiggle;
        [SerializeField] Collider blobCollider;

        Rigidbody body;
        Mess mess;
        State state;
        float loosen;
        float lastSuctionTime = float.NegativeInfinity;
        float suctionStrength;
        Vector3 suctionDirection = Vector3.up;
        ISuctionSource swallower;
        Transform swallowTarget;
        Vector3 swallowStart;
        float swallowElapsed;

        public Vector3 SuctionPoint => body.worldCenterOfMass;
        public Rigidbody Body => body;
        public bool CanBeSucked => state != State.Swallowing;
        /// <summary>0..1 loosening progress while stuck.</summary>
        public float LoosenProgress => loosen;

        void Awake()
        {
            body = GetComponent<Rigidbody>();
            mess = GetComponent<Mess>();
        }

        void Start()
        {
            body.mass = config.mass;
            body.linearDamping = config.drag;
            jiggle.SetHardnessTint(mess.Hardness);
            SetState(mess.Hardness > 0f ? State.Stuck : State.Loose);
        }

        public void ReceiveSuction(in SuctionContext context)
        {
            lastSuctionTime = Time.fixedTime;
            Vector3 toNozzle = context.Nozzle.position - SuctionPoint;
            float distance = toNozzle.magnitude;
            suctionDirection = distance > 0f ? toNozzle / distance : Vector3.up;
            suctionStrength = context.Strength;

            if (state == State.Stuck)
            {
                float rate = CleaningMath.ProgressRate(context.CleaningSpeed, mess.Hardness, context.MaxHardness);
                loosen += rate * context.Strength * context.DeltaTime;
                if (loosen >= 1f)
                    BreakFree();
                return;
            }

            if (state != State.Loose)
                return;

            if (distance <= context.SwallowRadius)
            {
                BeginSwallow(context.Source, context.Nozzle);
                return;
            }

            Vector3 velocity = body.linearVelocity;
            Vector3 lateral = velocity - Vector3.Dot(velocity, suctionDirection) * suctionDirection;
            body.AddForce(suctionDirection * (context.PullAcceleration * context.Strength), ForceMode.Acceleration);
            body.AddForce(-lateral * (context.LateralDamping * context.Strength), ForceMode.Acceleration);
        }

        void FixedUpdate()
        {
            // Grace of a couple of physics steps: script order between the vacuum and
            // this blob, plus float rounding, would otherwise make suction flicker off.
            bool beingSucked = Time.fixedTime - lastSuctionTime <= Time.fixedDeltaTime * SuctionGraceSteps;
            if (!beingSucked)
            {
                suctionStrength = 0f;
                if (state == State.Stuck)
                    loosen = Mathf.Max(0f, loosen - config.loosenDecay * Time.fixedDeltaTime);
            }
        }

        void Update()
        {
            switch (state)
            {
                case State.Stuck: UpdateStuckVisual(); break;
                case State.Loose: UpdateLooseVisual(); break;
                case State.Swallowing: UpdateSwallow(); break;
            }
        }

        void UpdateStuckVisual()
        {
            float tug = Mathf.Lerp(config.tugStretchAtStart, config.tugStretchAtBreak, loosen);
            float wobble = Mathf.Sin(Time.time * config.tugWobbleFrequency * 2f * Mathf.PI) * config.tugWobbleAmount;
            jiggle.SetTarget(suctionDirection, (tug + wobble) * suctionStrength);
        }

        void UpdateLooseVisual()
        {
            Vector3 velocity = body.linearVelocity;
            float speed = velocity.magnitude;
            float stretch = Mathf.Min(speed * config.stretchPerSpeed, config.maxStretch);
            jiggle.SetTarget(speed > 0f ? velocity : Vector3.up, stretch);
        }

        void BreakFree()
        {
            loosen = 1f;
            SetState(State.Loose);
            body.linearVelocity = suctionDirection * config.breakFreeImpulse;
            jiggle.Kick(config.breakFreeJiggle);
            EventBus<MessBrokeFree>.Raise(new MessBrokeFree { Family = mess.Family, Position = transform.position });
        }

        void BeginSwallow(ISuctionSource source, Transform nozzle)
        {
            swallower = source;
            swallowTarget = nozzle;
            swallowStart = transform.position;
            swallowElapsed = 0f;
            SetState(State.Swallowing);
        }

        void UpdateSwallow()
        {
            swallowElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(swallowElapsed / config.swallowTime);
            transform.position = Vector3.Lerp(swallowStart, swallowTarget.position, t * t);
            jiggle.SetSizeMultiplier(1f - t);
            if (t < 1f)
                return;

            mess.Clean();
            swallower?.NotifySwallowed(mess);
            Destroy(gameObject);
        }

        void SetState(State newState)
        {
            state = newState;
            bool simulated = state == State.Loose;
            body.isKinematic = !simulated;
            body.interpolation = simulated ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
            blobCollider.enabled = state != State.Swallowing;
        }

        void OnCollisionEnter(Collision collision)
        {
            float impactSpeed = collision.relativeVelocity.magnitude;
            jiggle.Kick(-impactSpeed * config.impactJiggle);
        }
    }
}

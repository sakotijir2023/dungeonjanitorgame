using DungeonJanitor.Messes;
using UnityEngine;

namespace DungeonJanitor.Tools.Vacuum
{
    /// <summary>Everything a suckable object needs to know about one physics step of suction.</summary>
    public readonly struct SuctionContext
    {
        public readonly ISuctionSource Source;
        public readonly Transform Nozzle;
        /// <summary>0..1 after distance falloff and spin-up.</summary>
        public readonly float Strength;
        public readonly float PullAcceleration;
        public readonly float LateralDamping;
        public readonly float CleaningSpeed;
        public readonly float MaxHardness;
        public readonly float SwallowRadius;
        public readonly float DeltaTime;

        public SuctionContext(ISuctionSource source, Transform nozzle, float strength, float pullAcceleration,
            float lateralDamping, float cleaningSpeed, float maxHardness, float swallowRadius, float deltaTime)
        {
            Source = source;
            Nozzle = nozzle;
            Strength = strength;
            PullAcceleration = pullAcceleration;
            LateralDamping = lateralDamping;
            CleaningSpeed = cleaningSpeed;
            MaxHardness = maxHardness;
            SwallowRadius = swallowRadius;
            DeltaTime = deltaTime;
        }
    }

    /// <summary>Something the vacuum can pull on (goo now, small debris later).</summary>
    public interface ISuctionTarget
    {
        Vector3 SuctionPoint { get; }
        Rigidbody Body { get; }
        bool CanBeSucked { get; }
        void ReceiveSuction(in SuctionContext context);
    }

    /// <summary>The thing doing the sucking; told when it swallows a mess.</summary>
    public interface ISuctionSource
    {
        void NotifySwallowed(Mess mess);
    }

    public static class SuctionMath
    {
        /// <summary>1 at the nozzle, 0 at max range, shaped by the falloff exponent (0 = no falloff).</summary>
        public static float DistanceFalloff(float distance, float range, float exponent)
        {
            if (range <= 0f || distance >= range)
                return 0f;
            float t = 1f - Mathf.Max(0f, distance) / range;
            return Mathf.Pow(t, Mathf.Max(0f, exponent));
        }
    }
}

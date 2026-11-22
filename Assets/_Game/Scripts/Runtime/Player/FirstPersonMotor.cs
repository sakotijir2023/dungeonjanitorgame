using DungeonJanitor.Core;
using DungeonJanitor.Input;
using UnityEngine;

namespace DungeonJanitor.Player
{
    /// <summary>
    /// Walk, sprint, jump and gravity on a CharacterController. Every number
    /// comes from MovementConfig.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonMotor : MonoBehaviour
    {
        [SerializeField] MovementConfig config;
        [SerializeField] InputReader input;

        CharacterController controller;
        Vector3 horizontalVelocity;
        float verticalVelocity;
        float lastMoveVerticalVelocity;
        float timeSinceGrounded = float.PositiveInfinity;
        float timeSinceJumpPressed = float.PositiveInfinity;
        bool wasGrounded;

        public Vector3 Velocity => horizontalVelocity + Vector3.up * verticalVelocity;
        public bool IsGrounded => controller.isGrounded;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
            ApplyBodyShape();
        }

        void OnEnable()
        {
            input.JumpPressed += OnJumpPressed;
            config.Changed += ApplyBodyShape;
        }

        void OnDisable()
        {
            input.JumpPressed -= OnJumpPressed;
            config.Changed -= ApplyBodyShape;
        }

        void OnJumpPressed() => timeSinceJumpPressed = 0f;

        void ApplyBodyShape()
        {
            controller.height = config.bodyHeight;
            controller.radius = config.bodyRadius;
            controller.center = Vector3.up * (config.bodyHeight * 0.5f);
            controller.stepOffset = Mathf.Min(config.stepOffset, config.bodyHeight);
            controller.slopeLimit = config.slopeLimit;
        }

        void Update()
        {
            float dt = Time.deltaTime;
            bool grounded = controller.isGrounded;

            UpdateGroundState(grounded, dt);
            timeSinceJumpPressed += dt;
            TryJump();
            UpdateHorizontal(grounded, dt);
            ApplyGravity(dt);

            lastMoveVerticalVelocity = verticalVelocity;
            CollisionFlags flags = controller.Move(Velocity * dt);
            if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0f)
                verticalVelocity = 0f;

            wasGrounded = grounded;
        }

        void UpdateGroundState(bool grounded, float dt)
        {
            if (grounded && !wasGrounded)
            {
                float fallSpeed = -lastMoveVerticalVelocity;
                if (fallSpeed >= config.landedEventMinFallSpeed)
                    EventBus<PlayerLanded>.Raise(new PlayerLanded { FallSpeed = fallSpeed });
            }

            if (grounded)
            {
                timeSinceGrounded = 0f;
                if (verticalVelocity < 0f)
                    verticalVelocity = -config.groundStickSpeed;
            }
            else
            {
                timeSinceGrounded += dt;
            }
        }

        void TryJump()
        {
            bool jumpBuffered = timeSinceJumpPressed <= config.jumpBufferTime;
            bool canJump = timeSinceGrounded <= config.coyoteTime;
            if (!jumpBuffered || !canJump)
                return;

            verticalVelocity = MovementMath.JumpVelocity(config.gravity, config.jumpHeight);
            timeSinceJumpPressed = float.PositiveInfinity;
            timeSinceGrounded = float.PositiveInfinity;
            EventBus<PlayerJumped>.Raise(new PlayerJumped());
        }

        void UpdateHorizontal(bool grounded, float dt)
        {
            Vector2 moveInput = Vector2.ClampMagnitude(input.Move, 1f);
            Vector3 wishDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            bool hasInput = wishDirection.sqrMagnitude > 0f;

            float targetSpeed = input.SprintHeld ? config.sprintSpeed : config.walkSpeed;
            Vector3 targetVelocity = wishDirection * targetSpeed;

            float acceleration;
            if (!grounded)
                acceleration = config.airAcceleration;
            else
                acceleration = hasInput ? config.groundAcceleration : config.groundDeceleration;

            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, targetVelocity, acceleration * dt);
        }

        void ApplyGravity(float dt)
        {
            float multiplier = verticalVelocity < 0f ? config.fallGravityMultiplier : 1f;
            verticalVelocity -= config.gravity * multiplier * dt;
            verticalVelocity = Mathf.Max(verticalVelocity, -config.terminalVelocity);
        }
    }
}

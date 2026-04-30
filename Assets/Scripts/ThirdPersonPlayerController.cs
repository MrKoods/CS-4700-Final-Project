using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Camera-relative third-person player controller.
    /// Reads WASD/arrow input, translates it relative to the camera's yaw,
    /// applies gravity, and rotates the character to face the move direction.
    /// Base movement is a run; hold Shift to sprint. Space to jump.
    /// Requires a CharacterController on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonPlayerController : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DefaultRunSpeed      = 4f;
        private const float DefaultSprintSpeed   = 7f;
        private const float DefaultRotationSpeed = 720f;
        private const float DefaultGravity       = -18f;
        private const float DefaultJumpHeight    = 1.4f;
        private const float GroundedDownForce    = -2f;

        private const string SpeedParam       = "Speed";
        private const string IsJumpingParam   = "IsJumping";
        private const string IsSprintingParam = "IsSprinting";

        // ---------------------------------------------------------------------------
        // Public fields
        // ---------------------------------------------------------------------------

        /// <summary>World units per second the player runs (base movement).</summary>
        public float RunSpeed = DefaultRunSpeed;

        /// <summary>World units per second while sprinting (Shift held).</summary>
        public float SprintSpeed = DefaultSprintSpeed;

        /// <summary>Degrees per second the character body rotates toward the move direction.</summary>
        public float RotationSpeed = DefaultRotationSpeed;

        /// <summary>Gravity acceleration (negative, m/s²).</summary>
        public float Gravity = DefaultGravity;

        /// <summary>Height the player reaches at the apex of a jump, in world units.</summary>
        public float JumpHeight = DefaultJumpHeight;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private CharacterController _controller;
        private Animator            _animator;
        private Camera              _mainCamera;
        private Vector3             _verticalVelocity;
        private bool                _movementEnabled = true;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator   = GetComponent<Animator>();
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            ApplyGravity();

            if (_movementEnabled)
            {
                HandleJumpInput();
                ApplyMovement();
            }
            else
            {
                _animator?.SetFloat(SpeedParam, 0f);
                _animator?.SetBool(IsJumpingParam, false);
                _animator?.SetBool(IsSprintingParam, false);
            }
        }

        // ---------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------

        /// <summary>Enables or disables player movement (e.g., during dialogue).</summary>
        public void SetMovementEnabled(bool enabled)
        {
            _movementEnabled = enabled;
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private void HandleJumpInput()
        {
            if (_controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                // v = sqrt(2 * |gravity| * jumpHeight)
                _verticalVelocity.y = Mathf.Sqrt(2f * Mathf.Abs(Gravity) * JumpHeight);
                _animator?.SetBool(IsJumpingParam, true);
            }

            // Clear the jump flag once we land.
            if (_controller.isGrounded && _verticalVelocity.y <= 0f)
            {
                _animator?.SetBool(IsJumpingParam, false);
            }
        }

        private void ApplyMovement()
        {
            float horizontal  = Input.GetAxisRaw("Horizontal");
            float vertical    = Input.GetAxisRaw("Vertical");
            bool  isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDir.sqrMagnitude < 0.01f)
            {
                _animator?.SetFloat(SpeedParam, 0f);
                _animator?.SetBool(IsSprintingParam, false);
                return;
            }

            // Project input onto the camera's horizontal plane so WASD is camera-relative.
            Vector3 cameraForward = _mainCamera != null
                ? Vector3.ProjectOnPlane(_mainCamera.transform.forward, Vector3.up).normalized
                : Vector3.forward;
            Vector3 cameraRight = _mainCamera != null
                ? Vector3.ProjectOnPlane(_mainCamera.transform.right, Vector3.up).normalized
                : Vector3.right;

            Vector3 moveDir = (cameraForward * inputDir.z + cameraRight * inputDir.x).normalized;

            // Smoothly rotate character body toward move direction.
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRot, RotationSpeed * Time.deltaTime);

            float speed = isSprinting ? SprintSpeed : RunSpeed;
            _controller.Move(moveDir * speed * Time.deltaTime);

            _animator?.SetFloat(SpeedParam, speed);
            _animator?.SetBool(IsSprintingParam, isSprinting);
        }

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity.y < 0f)
            {
                _verticalVelocity.y = GroundedDownForce;
            }

            _verticalVelocity.y += Gravity * Time.deltaTime;
            _controller.Move(_verticalVelocity * Time.deltaTime);
        }
    }
}

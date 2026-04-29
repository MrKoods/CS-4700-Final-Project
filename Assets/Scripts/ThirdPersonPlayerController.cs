using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Camera-relative third-person player controller.
    /// Reads WASD/arrow input, translates it relative to the camera's yaw,
    /// applies gravity, and rotates the character to face the move direction.
    /// Requires a CharacterController on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonPlayerController : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DefaultMoveSpeed     = 4f;
        private const float DefaultRotationSpeed = 720f;
        private const float DefaultGravity       = -18f;
        private const float GroundedDownForce    = -2f;
        private const string SpeedParam          = "Speed";

        // ---------------------------------------------------------------------------
        // Public fields
        // ---------------------------------------------------------------------------

        /// <summary>World units per second the player walks.</summary>
        public float MoveSpeed = DefaultMoveSpeed;

        /// <summary>Degrees per second the character body rotates toward the move direction.</summary>
        public float RotationSpeed = DefaultRotationSpeed;

        /// <summary>Gravity acceleration (negative, m/s²).</summary>
        public float Gravity = DefaultGravity;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private CharacterController _controller;
        private Animator            _animator;
        private Vector3             _verticalVelocity;
        private bool                _movementEnabled = true;
        private Camera              _mainCamera;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Awake()
        {
            _controller  = GetComponent<CharacterController>();
            _animator    = GetComponent<Animator>();
            _mainCamera  = Camera.main;
        }

        private void Update()
        {
            if (_movementEnabled)
            {
                ApplyMovement();
            }
            else
            {
                _animator?.SetFloat(SpeedParam, 0f);
            }

            ApplyGravity();
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

        private void ApplyMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical   = Input.GetAxisRaw("Vertical");

            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;
            if (inputDir.sqrMagnitude < 0.01f)
            {
                _animator?.SetFloat(SpeedParam, 0f);
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

            _controller.Move(moveDir * MoveSpeed * Time.deltaTime);
            _animator?.SetFloat(SpeedParam, MoveSpeed * inputDir.magnitude);
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

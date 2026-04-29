using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Simple third-person player controller driven by keyboard/gamepad input.
    /// Requires a CharacterController component on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DefaultMoveSpeed = 4f;
        private const float DefaultRotationSpeed = 720f;
        private const float DefaultGravity = -9.81f;

        // ---------------------------------------------------------------------------
        // Public fields
        // ---------------------------------------------------------------------------

        /// <summary>Movement speed in world units per second.</summary>
        public float MoveSpeed = DefaultMoveSpeed;

        /// <summary>Degrees per second the character rotates toward the move direction.</summary>
        public float RotationSpeed = DefaultRotationSpeed;

        /// <summary>Gravity acceleration applied each frame when the character is airborne.</summary>
        public float Gravity = DefaultGravity;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private CharacterController _characterController;
        private Vector3 _verticalVelocity;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            ApplyMovement();
            ApplyGravity();
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private void ApplyMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                // Rotate toward movement direction smoothly
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

                _characterController.Move(moveDirection * MoveSpeed * Time.deltaTime);
            }
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity.y < 0f)
            {
                _verticalVelocity.y = -2f; // small downward force to keep grounded
            }

            _verticalVelocity.y += Gravity * Time.deltaTime;
            _characterController.Move(_verticalVelocity * Time.deltaTime);
        }
    }
}

using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Smooth third-person camera that orbits the player using mouse input and
    /// follows behind and above the target. Attach to the Main Camera.
    /// </summary>
    public class ThirdPersonCameraFollow : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DefaultFollowDistance  = 5f;
        private const float DefaultHeightOffset    = 2f;
        private const float DefaultSmoothSpeed     = 10f;
        private const float DefaultMouseSensitivity = 3f;
        private const float MinPitchAngle          = -10f;
        private const float MaxPitchAngle          = 60f;

        // ---------------------------------------------------------------------------
        // Public fields
        // ---------------------------------------------------------------------------

        /// <summary>The transform to follow (assign the UserPlayer).</summary>
        public Transform Target;

        /// <summary>Distance the camera maintains behind the target.</summary>
        public float FollowDistance = DefaultFollowDistance;

        /// <summary>Vertical offset above the target's pivot.</summary>
        public float HeightOffset = DefaultHeightOffset;

        /// <summary>Smoothing factor for camera position interpolation.</summary>
        public float SmoothSpeed = DefaultSmoothSpeed;

        /// <summary>Mouse sensitivity for orbit input.</summary>
        public float MouseSensitivity = DefaultMouseSensitivity;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private float _yaw;
        private float _pitch = 15f;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Start()
        {
            if (Target == null)
            {
                Debug.LogWarning("[ThirdPersonCameraFollow] No target assigned. Camera will not follow.", this);
                return;
            }

            // Initialise orbit angles from current camera orientation.
            Vector3 angles = transform.eulerAngles;
            _yaw   = angles.y;
            _pitch = angles.x;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        private void LateUpdate()
        {
            if (Target == null)
                return;

            ReadMouseInput();
            UpdateCameraTransform();
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private void ReadMouseInput()
        {
            _yaw   += Input.GetAxis("Mouse X") * MouseSensitivity;
            _pitch -= Input.GetAxis("Mouse Y") * MouseSensitivity;
            _pitch  = Mathf.Clamp(_pitch, MinPitchAngle, MaxPitchAngle);
        }

        private void UpdateCameraTransform()
        {
            Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);

            Vector3 lookAtPoint  = Target.position + Vector3.up * HeightOffset;
            Vector3 desiredPosition = lookAtPoint - rotation * Vector3.forward * FollowDistance;

            transform.position = Vector3.Lerp(
                transform.position, desiredPosition, SmoothSpeed * Time.deltaTime);

            transform.LookAt(lookAtPoint);
        }
    }
}

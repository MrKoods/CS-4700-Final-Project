using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Moves an NPC along a looping series of waypoints using frame-rate-independent
    /// position stepping. Attach to a villager GameObject and assign the Waypoints
    /// array in the Inspector.
    /// Uses CharacterController for ground-pinned movement with gravity.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class NPCWaypointPatrol : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DefaultMoveSpeed                = 1.5f;
        private const float DefaultWaypointReachedThreshold = 0.2f;
        private const float Gravity                         = -18f;
        private const float GroundedDownForce               = -2f;

        // ---------------------------------------------------------------------------
        // Public fields (Inspector-assignable)
        // ---------------------------------------------------------------------------

        /// <summary>Ordered patrol points the NPC will walk between in a loop.</summary>
        public Transform[] Waypoints;

        /// <summary>Movement speed in world units per second.</summary>
        public float MoveSpeed = DefaultMoveSpeed;

        /// <summary>
        /// The NPC considers a waypoint reached when its XZ distance to the waypoint
        /// drops at or below this value.
        /// </summary>
        public float WaypointReachedThreshold = DefaultWaypointReachedThreshold;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private const string SpeedParam = "Speed";

        private CharacterController _controller;
        private Animator            _animator;
        private int                 _currentIndex;
        private float               _verticalVelocity;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _animator   = GetComponent<Animator>();
        }

        private void Start()
        {
            if (Waypoints == null || Waypoints.Length == 0)
            {
                Debug.LogWarning($"[NPCWaypointPatrol] '{name}' has no waypoints assigned. Component disabled.", this);
                enabled = false;
                return;
            }

            // Disable NPCWander so both scripts don't fight over CharacterController.Move().
            NPCWander wander = GetComponent<NPCWander>();
            if (wander != null)
                wander.enabled = false;
        }

        private void Update()
        {
            ApplyGravity();

            Transform target = Waypoints[_currentIndex];

            // Lock Y so the NPC stays grounded regardless of waypoint height.
            Vector3 targetFlat = new Vector3(target.position.x, transform.position.y, target.position.z);
            Vector3 moveDir    = (targetFlat - transform.position);
            float   distXZ     = new Vector2(moveDir.x, moveDir.z).magnitude;

            // Rotate to face movement direction.
            if (moveDir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(moveDir);

            if (distXZ > WaypointReachedThreshold)
            {
                _controller.Move(moveDir.normalized * MoveSpeed * Time.deltaTime);
                _animator?.SetFloat(SpeedParam, MoveSpeed);
            }
            else
            {
                _animator?.SetFloat(SpeedParam, 0f);
                _currentIndex = (_currentIndex + 1) % Waypoints.Length;
            }
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private void ApplyGravity()
        {
            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = GroundedDownForce;

            _verticalVelocity += Gravity * Time.deltaTime;
            _controller.Move(new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f));
        }
    }
}

using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Makes an NPC wander slowly within a radius around its spawn position.
    /// Picks a random target within the wander radius, walks toward it, then
    /// waits briefly before picking another. Does not require a NavMesh.
    /// Pauses automatically when dialogue is open via DialogueManager.
    /// Uses CharacterController for ground-pinned movement with gravity.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class NPCWander : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DefaultWanderRadius    = 5f;
        private const float DefaultMoveSpeed       = 1.2f;
        private const float DefaultRotationSpeed   = 180f;
        private const float DefaultArrivalDistance = 0.3f;
        private const float DefaultMinWaitTime     = 1f;
        private const float DefaultMaxWaitTime     = 3f;
        private const float Gravity                = -18f;
        private const float GroundedDownForce      = -2f;
        private const string SpeedParam            = "Speed";

        // ---------------------------------------------------------------------------
        // Public fields
        // ---------------------------------------------------------------------------

        /// <summary>Maximum distance from the spawn point the NPC may wander.</summary>
        public float WanderRadius = DefaultWanderRadius;

        /// <summary>Walk speed in world units per second.</summary>
        public float MoveSpeed = DefaultMoveSpeed;

        /// <summary>Degrees per second for rotation toward movement direction.</summary>
        public float RotationSpeed = DefaultRotationSpeed;

        /// <summary>Distance at which a waypoint is considered reached.</summary>
        public float ArrivalDistance = DefaultArrivalDistance;

        /// <summary>Minimum idle wait time in seconds between wanders.</summary>
        public float MinWaitTime = DefaultMinWaitTime;

        /// <summary>Maximum idle wait time in seconds between wanders.</summary>
        public float MaxWaitTime = DefaultMaxWaitTime;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private CharacterController _controller;
        private Animator            _animator;
        private Vector3             _spawnPosition;
        private Vector3             _targetPosition;
        private float               _waitTimer;
        private bool                _waiting;
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
            _spawnPosition  = transform.position;
            _targetPosition = transform.position;
            BeginWait();
        }

        private void Update()
        {
            ApplyGravity();

            // Pause movement while dialogue is active.
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueOpen)
                return;

            if (_waiting)
            {
                _animator?.SetFloat(SpeedParam, 0f);
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                    PickNewTarget();
                return;
            }

            MoveTowardTarget();
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

        private void PickNewTarget()
        {
            Vector2 randomCircle = Random.insideUnitCircle * WanderRadius;
            _targetPosition = new Vector3(
                _spawnPosition.x + randomCircle.x,
                _spawnPosition.y,
                _spawnPosition.z + randomCircle.y);
            _waiting = false;
        }

        private void MoveTowardTarget()
        {
            Vector3 flat = new Vector3(_targetPosition.x, transform.position.y, _targetPosition.z);
            float distance = Vector3.Distance(transform.position, flat);

            if (distance <= ArrivalDistance)
            {
                BeginWait();
                return;
            }

            Vector3 dir = (flat - transform.position).normalized;

            // Rotate to face movement direction.
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
            }

            _animator?.SetFloat(SpeedParam, MoveSpeed);
            _controller.Move(dir * MoveSpeed * Time.deltaTime);
        }

        private void BeginWait()
        {
            _waiting   = true;
            _waitTimer = Random.Range(MinWaitTime, MaxWaitTime);
        }
    }
}

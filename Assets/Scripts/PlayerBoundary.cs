using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Clamps the player inside a rectangular play area centred on the town.
    /// Attach this to the same GameObject as ThirdPersonPlayerController.
    ///
    /// When the player reaches the boundary edge their position is clamped
    /// back inside each frame, making the wall feel solid without needing
    /// physics colliders.  A soft-push force begins slightly before the hard
    /// limit so the player feels resistance rather than a sudden stop.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerBoundary : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        /// <summary>World-space centre of the play area (town centre).</summary>
        private static readonly Vector3 BoundaryCenter = new Vector3(-8.43f, 0f, -11.44f);

        /// <summary>
        /// Half-width (X) and half-depth (Z) of the hard boundary in world units.
        /// Ground world half-extent ≈ 1375 * 0.333 ≈ 458 units; town area is ~40 units.
        /// 120 units gives comfortable exploration without reaching the visual edge.
        /// </summary>
        private const float HalfExtentX = 120f;
        private const float HalfExtentZ = 120f;

        /// <summary>
        /// How many units before the hard limit the soft push-back begins.
        /// Makes the boundary feel like resistance rather than a sudden wall.
        /// </summary>
        private const float SoftZoneWidth = 15f;

        /// <summary>Maximum push-back speed applied in the soft zone (world units/s).</summary>
        private const float PushBackSpeed = 8f;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private CharacterController _controller;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void LateUpdate()
        {
            ApplyBoundary();
        }

        // ---------------------------------------------------------------------------
        // Boundary logic
        // ---------------------------------------------------------------------------

        private void ApplyBoundary()
        {
            Vector3 pos = transform.position;
            Vector3 clamped = pos;

            float relX = pos.x - BoundaryCenter.x;
            float relZ = pos.z - BoundaryCenter.z;

            // Hard clamp — CharacterController.Move is additive so we override position directly.
            clamped.x = BoundaryCenter.x + Mathf.Clamp(relX, -HalfExtentX, HalfExtentX);
            clamped.z = BoundaryCenter.z + Mathf.Clamp(relZ, -HalfExtentZ, HalfExtentZ);

            if (clamped != pos)
            {
                // Disable the controller temporarily so we can warp the position.
                _controller.enabled = false;
                transform.position  = clamped;
                _controller.enabled = true;
            }
        }

        // ---------------------------------------------------------------------------
        // Gizmo — visible in the editor so the boundary is easy to resize
        // ---------------------------------------------------------------------------

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            DrawBoundaryGizmo(HalfExtentX, HalfExtentZ, new Color(1f, 0.3f, 0.1f, 0.4f));
            DrawBoundaryGizmo(HalfExtentX - SoftZoneWidth, HalfExtentZ - SoftZoneWidth,
                              new Color(1f, 0.9f, 0.1f, 0.25f));
        }

        private static void DrawBoundaryGizmo(float hx, float hz, Color color)
        {
            Vector3 c = BoundaryCenter;
            float   y = 1f;

            Gizmos.color = color;
            // Four edges of the rectangle
            Gizmos.DrawLine(new Vector3(c.x - hx, y, c.z - hz), new Vector3(c.x + hx, y, c.z - hz));
            Gizmos.DrawLine(new Vector3(c.x + hx, y, c.z - hz), new Vector3(c.x + hx, y, c.z + hz));
            Gizmos.DrawLine(new Vector3(c.x + hx, y, c.z + hz), new Vector3(c.x - hx, y, c.z + hz));
            Gizmos.DrawLine(new Vector3(c.x - hx, y, c.z + hz), new Vector3(c.x - hx, y, c.z - hz));
        }
#endif
    }
}

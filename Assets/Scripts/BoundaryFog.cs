using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Drives Unity's built-in exponential-squared fog based on the player's proximity
    /// to the town boundary. Fog starts thin in the town centre and ramps up to a thick
    /// wall as the player approaches and crosses the boundary edge.
    /// Attach to any persistent scene object (e.g. the Lighting GameObject).
    /// </summary>
    public class BoundaryFog : MonoBehaviour
    {
        // -------------------------------------------------------------------------
        // Boundary — must match PlayerBoundary constants exactly.
        // -------------------------------------------------------------------------

        private static readonly Vector3 BoundaryCenter = new Vector3(-45.43f, 0f, -11.44f);
        private const float HalfExtentX   = 280f;
        private const float HalfExtentZ   = 200f;
        private const float SoftZoneWidth = 50f;   // ramp starts 50 units before the hard edge

        // -------------------------------------------------------------------------
        // Fog configuration
        // -------------------------------------------------------------------------

        /// <summary>Fog colour — cool grey matches an overcast sky.</summary>
        private static readonly Color FogColor = new Color(0.55f, 0.58f, 0.62f, 1f);

        /// <summary>Density when the player is well inside the town (light atmospheric haze).</summary>
        private const float FogDensityMin = 0.012f;

        /// <summary>Density at and beyond the hard boundary (impenetrable wall of fog).</summary>
        private const float FogDensityMax = 0.18f;

        /// <summary>How quickly the density reading tracks the target (lower = smoother).</summary>
        private const float SmoothSpeed = 3f;

        // -------------------------------------------------------------------------
        // Private state
        // -------------------------------------------------------------------------

        private Transform _player;
        private float     _currentDensity;

        // -------------------------------------------------------------------------
        // Unity lifecycle
        // -------------------------------------------------------------------------

        private void Start()
        {
            // Enable fog and set initial properties.
            RenderSettings.fog          = true;
            RenderSettings.fogMode      = FogMode.ExponentialSquared;
            RenderSettings.fogColor     = FogColor;
            RenderSettings.fogDensity   = FogDensityMin;
            _currentDensity             = FogDensityMin;

            // Locate the player by tag then fall back to name search.
            var playerGO = GameObject.FindWithTag("Player")
                        ?? GameObject.Find("UserPlayer");

            if (playerGO != null)
                _player = playerGO.transform;
            else
                Debug.LogWarning("[BoundaryFog] Could not find player — fog will stay at minimum density.");
        }

        private void Update()
        {
            if (_player == null) return;

            float targetDensity = ComputeTargetDensity(_player.position);
            _currentDensity     = Mathf.Lerp(_currentDensity, targetDensity, Time.deltaTime * SmoothSpeed);
            RenderSettings.fogDensity = _currentDensity;
        }

        // -------------------------------------------------------------------------
        // Density calculation
        // -------------------------------------------------------------------------

        /// <summary>
        /// Returns a fog density in [FogDensityMin, FogDensityMax] based on how close
        /// the player is to the boundary. The ramp starts SoftZoneWidth units before
        /// the hard edge and reaches maximum at the edge.
        /// </summary>
        private static float ComputeTargetDensity(Vector3 playerPos)
        {
            // Distance from the player to the nearest boundary edge on each axis.
            float distToEdgeX = HalfExtentX - Mathf.Abs(playerPos.x - BoundaryCenter.x);
            float distToEdgeZ = HalfExtentZ - Mathf.Abs(playerPos.z - BoundaryCenter.z);

            // Use the closer of the two axes — the player only needs to be near one wall.
            float distToNearestEdge = Mathf.Min(distToEdgeX, distToEdgeZ);

            // t = 0 when fully inside the soft zone, 1 when at or past the hard edge.
            float t = 1f - Mathf.Clamp01(distToNearestEdge / SoftZoneWidth);
            t = Mathf.SmoothStep(0f, 1f, t);

            return Mathf.Lerp(FogDensityMin, FogDensityMax, t);
        }
    }
}

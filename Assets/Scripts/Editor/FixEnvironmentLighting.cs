using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace CS4700.Editor
{
    /// <summary>
    /// Editor utility that reduces ambient/environment lighting intensity so the scene
    /// isn't washed out by the default sky-based GI.
    /// Runs automatically once per editor session via InitializeOnLoad, and is also
    /// available manually at Tools > CS4700 > Fix Environment Lighting.
    /// </summary>
    [InitializeOnLoad]
    public static class FixEnvironmentLighting
    {
        private const string MenuPath = "Tools/CS4700/Fix Environment Lighting";
        private const string AppliedSessionKey = "CS4700_EnvLightingFixed";

        static FixEnvironmentLighting()
        {
            // Auto-apply once per editor session to avoid overriding intentional changes.
            if (!SessionState.GetBool(AppliedSessionKey, false))
            {
                EditorApplication.delayCall += ApplyOnce;
            }
        }

        private static void ApplyOnce()
        {
            SessionState.SetBool(AppliedSessionKey, true);
            Apply();
        }

        /// <summary>
        /// Switches ambient mode to flat colour and reduces reflection intensity
        /// to prevent scene washout. Marks the active scene dirty.
        /// </summary>
        [MenuItem(MenuPath)]
        public static void Apply()
        {
            // Flat ambient removes the bright sky contribution from GI.
            RenderSettings.ambientMode = AmbientMode.Flat;

            // Neutral dark grey: visible fill light without blowing out surfaces.
            RenderSettings.ambientLight = new Color(0.18f, 0.18f, 0.20f, 1f);

            // Keep skybox for the camera clear but reduce its GI influence.
            RenderSettings.ambientIntensity = 1f;

            // Reduce reflection probes so flat opaque surfaces aren't over-lit.
            RenderSettings.reflectionIntensity = 0.4f;

            // Fog off — keeps the distant geometry readable.
            RenderSettings.fog = false;

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

            Debug.Log("[CS4700] Environment lighting fixed: Flat ambient (0.18 grey), reflection 0.4, fog off.");
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace CS4700
{
    public class DirectionRestore : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var coe = ChronicleOfEchoes.Instance;

            // FINAL LOOP
            if (coe.Has("HasLedger") && coe.Has("HasLocket") &&
                coe.Has("Knows_Abigail_Blackmail") && coe.Has("Knows_Caleb_Targeted"))
            {
                DirectionUI.Instance.SetDirection("Confront Caleb at the Overlook.");
                return;
            }

            // MID LOOP
            if (coe.Has("Knows_Abigail_Blackmail"))
            {
                DirectionUI.Instance.SetDirection("Someone is lying. Find proof.");
                return;
            }

            // AFTER DEATH (LoopStarted is always true after first run)
            if (coe.Has("LoopStarted"))
            {
                DirectionUI.Instance.SetDirection("Uncover the mystery. Ask around town.");
                return;
            }

            // FIRST TIME EVER
            DirectionUI.Instance.SetDirection("Head to the town gathering.");
        }
    }
}

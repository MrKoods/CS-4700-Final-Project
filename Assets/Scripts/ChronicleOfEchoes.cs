using UnityEngine;

namespace CS4700
{
    public class ChronicleOfEchoes : MonoBehaviour
    {
        public static ChronicleOfEchoes Instance;

        public int LoopCount = 1;

        // Knowledge flags
        public bool KnowsAbigailBlackmail;
        public bool HasLedger;
        public bool HasLocket;
        public bool KnowsCalebTargeted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ResetLoop()
        {
            LoopCount++;
            Debug.Log("Loop: " + LoopCount);
        }
    }
}
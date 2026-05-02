using UnityEngine;

namespace CS4700
{
    public class KnowledgeTrigger : MonoBehaviour
    {
        [Header("Knowledge Flag To Unlock")]
        public string flagName;

        [Header("Optional: Only trigger once")]
        public bool oneTime = true;

        private bool triggered = false;

        public void Trigger()
        {
            if (oneTime && triggered)
                return;

            triggered = true;

            ChronicleOfEchoes.Instance.Unlock(flagName);

            Debug.Log("Unlocked knowledge: " + flagName);
        }
    }
}

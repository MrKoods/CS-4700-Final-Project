using UnityEngine;

namespace CS4700
{
    public class OverlookTrigger : MonoBehaviour
    {
        private bool triggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (triggered) return;
            if (!other.CompareTag("Player")) return;

            triggered = true;

            OverlookController.Instance.StartConfrontation();
        }
    }
}

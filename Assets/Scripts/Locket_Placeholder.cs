using UnityEngine;

namespace CS4700
{
    public class Locket_Placeholder : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChronicleOfEchoes.Instance.Unlock("HasLocket");

                DialogueManager.Instance.OpenDialogue(
                    "A silver locket… Abigail’s. Danforth kept it hidden.",
                    "Silas"
                );

                Destroy(gameObject);
            }
        }
    }
}

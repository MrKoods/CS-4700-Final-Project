using UnityEngine;

namespace CS4700
{
    public class BlackmailNote_Placeholder : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChronicleOfEchoes.Instance.Unlock("Knows_Abigail_Blackmail");

                DialogueManager.Instance.OpenDialogue(
                    "A hidden note… Danforth threatened Abigail’s family unless she lied in court.",
                    "Silas"
                );

                Destroy(gameObject);
            }
        }
    }
}

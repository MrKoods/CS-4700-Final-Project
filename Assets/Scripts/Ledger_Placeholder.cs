using UnityEngine;

namespace CS4700
{
    public class Ledger_Placeholder : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChronicleOfEchoes.Instance.Unlock("HasLedger");

                // Open the dialogue box exactly like an NPC
                DialogueManager.Instance.OpenDialogue(
                    "A dusty ledger… forged signatures… Danforth planned everything.",
                    "Silas"
                );

                // Delay destruction so UI can render
                StartCoroutine(DestroyNextFrame());
            }
        }

        private System.Collections.IEnumerator DestroyNextFrame()
        {
            yield return null; // wait 1 frame
            Destroy(gameObject);
        }
    }
}

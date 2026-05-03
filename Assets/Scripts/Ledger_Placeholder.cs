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

                DialogueManager.Instance.OpenDialogue(
                    "A dusty ledger… forged signatures… Danforth planned everything.",
                    "Silas"
                );

                Destroy(gameObject);
            }
        }
    }
}

using UnityEngine;

namespace CS4700
{
    public class DanforthChest : MonoBehaviour
    {
        private bool opened = false;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (opened) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                opened = true;

                // Unlock the seal
                ChronicleOfEchoes.Instance.Unlock("HasDanforthSeal");

                // Dialogue popup
                DialogueManager.Instance.OpenDialogue(
                    "Inside the chest… Danforth’s personal wax seal. Proof of his forged documents.",
                    "Silas"
                );

                // Remove chest or leave it open
                Destroy(gameObject);
            }
        }
    }
}

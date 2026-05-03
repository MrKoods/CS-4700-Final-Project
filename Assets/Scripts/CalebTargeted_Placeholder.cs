using UnityEngine;

namespace CS4700
{
    public class CalebTargeted_Placeholder : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (Input.GetKeyDown(KeyCode.E))
            {
                ChronicleOfEchoes.Instance.Unlock("Knows_Caleb_Targeted");

                DialogueManager.Instance.OpenDialogue(
                    "Signs of struggle… someone was watching Caleb. He was being targeted.",
                    "Silas"
                );

                Destroy(gameObject);
            }
        }
    }
}

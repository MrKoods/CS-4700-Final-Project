using UnityEngine;

namespace CS4700
{
    public class Interactable : MonoBehaviour
    {
        [Header("Optional Dialogue")]
        [TextArea]
        public string dialogueText;
        public string speakerName = "Silas";

        public void Interact()
        {
            // 1. Trigger knowledge if present
            GetComponent<KnowledgeTrigger>()?.Trigger();

            // 2. Show dialogue if provided
            if (!string.IsNullOrEmpty(dialogueText))
            {
                DialogueManager.Instance.OpenDialogue(dialogueText, speakerName);
            }
        }
    }
}

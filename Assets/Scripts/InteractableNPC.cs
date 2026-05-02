using UnityEngine;

namespace CS4700
{
    public class InteractableNPC : MonoBehaviour
    {
        [Header("NPC Identity")]
        public string npcName;   // MUST match the name used in NarrativeController rules

        public void Interact()
        {
            // Ask the narrative system what this NPC should say
            DialogueLine line = NarrativeController.Instance.GetDialogueForNPC(npcName);

            // Send that line to the DialogueManager
            DialogueManager.Instance.OpenDialogue(line.text, npcName);
        }
    }
}

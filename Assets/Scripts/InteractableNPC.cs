using UnityEngine;

namespace CS4700
{
    public class InteractableNPC : MonoBehaviour
    {
        public string npcName = "NPC";
        public float interactDistance = 2f;

        private Transform player;

        void Start()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        void Update()
        {
            if (player == null) return;

            float dist = Vector3.Distance(player.position, transform.position);

            if (dist <= interactDistance && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        void Interact()
        {
            // 1. Get the correct narrative line for this NPC
            DialogueLine line = NarrativeController.Instance.GetDialogueForNPC(npcName);

            if (line == null)
            {
                DialogueManager.Instance.OpenDialogue("…", npcName);
                return;
            }

            // 2. Trigger knowledge if this line is marked as revealing something
            if (line.triggersKnowledge)
            {
                GetComponent<KnowledgeTrigger>()?.Trigger();
            }

            // 3. Show the dialogue using your DialogueManager
            DialogueManager.Instance.OpenDialogue(line.text, line.speaker);
        }
    }
}

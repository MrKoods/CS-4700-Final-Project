using UnityEngine;

namespace CS4700
{
    public class InteractableNPC : MonoBehaviour
    {
        public string npcName = "NPC";
        public float interactDistance = 2f;

        private Transform player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (player == null) return;

            float dist = Vector3.Distance(player.position, transform.position);

            if (dist <= interactDistance && Input.GetKeyDown(KeyCode.E))
            {
                string line = GetDialogueLine();
                DialogueManager.Instance.OpenDialogue(line, npcName);
            }
        }

        private string GetDialogueLine()
        {
            // Try to find a dialogue script on this NPC
            var dialogue = GetComponent<MonoBehaviour>();

            // Check each possible dialogue script
            if (TryGetComponent<AbigailDialogue>(out var abigail))
                return abigail.GetDialogue();

            if (TryGetComponent<FemaleVillagerDialogue>(out var femaleVillager))
                return femaleVillager.GetDialogue();

            if (TryGetComponent<CalebDialogue>(out var caleb))
                return caleb.GetDialogue();

            if (TryGetComponent<WeaverDialogue>(out var weaver))
                return weaver.GetDialogue();

            // Fallback
            return "...";
        }
    }
}

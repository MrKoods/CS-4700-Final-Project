using UnityEngine;

namespace CS4700
{
    public abstract class InteractableNPC : MonoBehaviour
    {
        public string npcName = "NPC";
        public float interactDistance = 2f;

        private Transform player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        private void Update()
        {
            if (player == null) return;

            float dist = Vector3.Distance(player.position, transform.position);

            if (dist <= interactDistance && Input.GetKeyDown(KeyCode.E))
            {
                string line = GetDialogue();
                DialogueManager.Instance.OpenDialogue(line, npcName);
            }
        }

        public abstract string GetDialogue();
    }
}

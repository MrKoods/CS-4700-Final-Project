using UnityEngine;

namespace CS4700
{
    public class InteractableNPC : MonoBehaviour
    {
        private const float DefaultInteractionRadius = 3f;

        public float InteractionRadius = DefaultInteractionRadius;

        [Header("Dialogue")]
        public string DialogueLine = "Hello.";

        // OPTIONAL override (leave empty to use GameObject name)
        public string OverrideName = "";

        private Transform _playerTransform;
        private bool _playerInRange;

        private string NPCName
        {
            get
            {
                if (!string.IsNullOrEmpty(OverrideName))
                    return OverrideName;

                return gameObject.name; // 🔥 AUTO USE OBJECT NAME
            }
        }

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _playerTransform = player.transform;
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            float dist = Vector3.Distance(transform.position, _playerTransform.position);
            _playerInRange = dist <= InteractionRadius;

            if (_playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                Interact();
            }
        }

        private void Interact()
        {
            // 🔥 MAIN STORY TRIGGER
            if (NPCName == "Caleb")
            {
                NarrativeController.Instance.StartOverlookScene();
                return;
            }

            DialogueManager.Instance.OpenDialogue(DialogueLine, NPCName);
        }

        private void OnGUI()
        {
            if (!_playerInRange) return;
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueOpen) return;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2f);
            if (screenPos.z < 0) return;

            float x = screenPos.x - 80;
            float y = Screen.height - screenPos.y - 15;

            GUI.Box(new Rect(x, y, 160, 30), $"[E] Talk: {NPCName}");
        }
    }
}
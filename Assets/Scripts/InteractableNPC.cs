using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Attach to any NPC to make it interactable. When the player enters the
    /// interaction radius, a "Press E to Talk" prompt appears. Pressing E opens
    /// a dialogue via <see cref="DialogueManager"/>.
    /// </summary>
    public class InteractableNPC : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DefaultInteractionRadius = 3f;
        private const string DefaultDialogueLine     = "Hello.";

        // ---------------------------------------------------------------------------
        // Public fields
        // ---------------------------------------------------------------------------

        /// <summary>Distance within which the player can interact with this NPC.</summary>
        public float InteractionRadius = DefaultInteractionRadius;

        /// <summary>The dialogue line this NPC speaks when interacted with.</summary>
        public string DialogueLine = DefaultDialogueLine;

        // ---------------------------------------------------------------------------
        // Private state
        // ---------------------------------------------------------------------------

        private Transform      _playerTransform;
        private bool           _playerInRange;
        private bool           _isDialogueOpen;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Start()
        {
            // Find the player by tag for decoupled lookup.
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                _playerTransform = playerObj.transform;
            else
                Debug.LogWarning("[InteractableNPC] No GameObject with tag 'Player' found.", this);
        }

        private void Update()
        {
            if (_playerTransform == null)
                return;

            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            _playerInRange = distance <= InteractionRadius;

            if (_playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                HandleInteraction();
            }
        }

        private void OnGUI()
        {
            // Only show the prompt when in range and dialogue is not already open.
            if (!_playerInRange)
                return;
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueOpen)
                return;

            DrawInteractionPrompt();
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private void HandleInteraction()
        {
            if (DialogueManager.Instance == null)
            {
                Debug.LogError("[InteractableNPC] DialogueManager not found in scene.", this);
                return;
            }

            if (!DialogueManager.Instance.IsDialogueOpen)
            {
                DialogueManager.Instance.OpenDialogue(DialogueLine);
            }
        }

        private void DrawInteractionPrompt()
        {
            // Convert NPC world position to screen position for label placement.
            Vector3 screenPos = Camera.main != null
                ? Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2.2f)
                : Vector3.zero;

            if (screenPos.z < 0f)
                return;

            float x = screenPos.x - 80f;
            float y = Screen.height - screenPos.y - 15f;

            GUIStyle style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize  = 14
            };

            GUI.Box(new Rect(x, y, 160f, 30f), "Press E to Talk", style);
        }
    }
}

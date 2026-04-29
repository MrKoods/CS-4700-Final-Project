using UnityEngine;

namespace CS4700
{
    /// <summary>
    /// Singleton that manages the dialogue UI using immediate-mode GUI (OnGUI).
    /// Call <see cref="OpenDialogue"/> to display a message; the player dismisses
    /// it with E or Space.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        // ---------------------------------------------------------------------------
        // Singleton
        // ---------------------------------------------------------------------------

        /// <summary>Global access point for the DialogueManager instance.</summary>
        public static DialogueManager Instance { get; private set; }

        // ---------------------------------------------------------------------------
        // Constants
        // ---------------------------------------------------------------------------

        private const float DialogueBoxWidth  = 500f;
        private const float DialogueBoxHeight = 120f;
        private const int   DialogueFontSize  = 18;
        private const int   HintFontSize      = 13;

        // ---------------------------------------------------------------------------
        // State
        // ---------------------------------------------------------------------------

        private string _currentText = string.Empty;
        private string _speakerName = string.Empty;

        /// <summary>True while a dialogue box is currently visible.</summary>
        public bool IsDialogueOpen { get; private set; }

        // ---------------------------------------------------------------------------
        // Events
        // ---------------------------------------------------------------------------

        /// <summary>Raised when dialogue opens. Parameter is the displayed text.</summary>
        public event System.Action<string> OnDialogueOpened;

        /// <summary>Raised when dialogue closes.</summary>
        public event System.Action OnDialogueClosed;

        // ---------------------------------------------------------------------------
        // Unity lifecycle
        // ---------------------------------------------------------------------------

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            if (!IsDialogueOpen)
                return;

            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
            {
                CloseDialogue();
            }
        }

        private void OnGUI()
        {
            if (!IsDialogueOpen)
                return;

            DrawDialogueBox();
        }

        // ---------------------------------------------------------------------------
        // Public API
        // ---------------------------------------------------------------------------

        /// <summary>Opens the dialogue box and displays the given text.</summary>
        public void OpenDialogue(string text, string speakerName = "")
        {
            _currentText   = text;
            _speakerName   = speakerName;
            IsDialogueOpen = true;

            SetPlayerMovement(false);
            OnDialogueOpened?.Invoke(text);
        }

        /// <summary>Closes the dialogue box and re-enables player movement.</summary>
        public void CloseDialogue()
        {
            IsDialogueOpen = false;
            _currentText   = string.Empty;
            _speakerName   = string.Empty;

            SetPlayerMovement(true);
            OnDialogueClosed?.Invoke();
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private void SetPlayerMovement(bool enabled)
        {
            ThirdPersonPlayerController player = FindFirstObjectByType<ThirdPersonPlayerController>();
            if (player != null)
                player.SetMovementEnabled(enabled);
        }

        private void DrawDialogueBox()
        {
            float x = (Screen.width  - DialogueBoxWidth)  * 0.5f;
            float y =  Screen.height - DialogueBoxHeight  - 40f;

            // Background box.
            GUI.Box(new Rect(x, y, DialogueBoxWidth, DialogueBoxHeight), string.Empty);

            // Speaker name (if provided).
            if (!string.IsNullOrEmpty(_speakerName))
            {
                GUIStyle nameStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize  = DialogueFontSize
                };
                GUI.Label(new Rect(x + 16f, y + 8f, DialogueBoxWidth - 32f, 24f), _speakerName, nameStyle);
            }

            // Dialogue text.
            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = DialogueFontSize,
                wordWrap = true
            };
            float textY = string.IsNullOrEmpty(_speakerName) ? y + 16f : y + 36f;
            GUI.Label(new Rect(x + 16f, textY, DialogueBoxWidth - 32f, 60f), _currentText, textStyle);

            // Dismiss hint.
            GUIStyle hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize  = HintFontSize,
                alignment = TextAnchor.LowerRight,
                fontStyle = FontStyle.Italic
            };
            GUI.Label(
                new Rect(x + 16f, y + DialogueBoxHeight - 26f, DialogueBoxWidth - 32f, 22f),
                "[E / Space] Close",
                hintStyle);
        }
    }
}

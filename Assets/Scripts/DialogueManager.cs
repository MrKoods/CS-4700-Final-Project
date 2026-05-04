using UnityEngine;

namespace CS4700
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private const float DialogueBoxWidth = 500f;
        private const float DialogueBoxHeight = 200f;

        private string _currentText = "";
        private string _speakerName = "";

        private bool _isDialogueOpen;
        public bool IsDialogueOpen => _isDialogueOpen;

        // Choices
        private string[] _choices;
        private System.Action<int> _onChoiceSelected;
        private bool _showingChoices;

        public event System.Action OnDialogueClosed;

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
            if (!_isDialogueOpen) return;

            // CHOICE INPUT
            if (_showingChoices)
            {
                for (int i = 0; i < _choices.Length; i++)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    {
                        SelectChoice(i);
                        return;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
                {
                    CloseDialogue();
                }
            }
        }

        private void OnGUI()
        {
            if (!_isDialogueOpen) return;

            float x = (Screen.width - DialogueBoxWidth) / 2;
            float y = Screen.height - DialogueBoxHeight - 40;

            GUI.Box(new Rect(x, y, DialogueBoxWidth, DialogueBoxHeight), "");

            // NAME
            if (!string.IsNullOrEmpty(_speakerName))
            {
                GUIStyle nameStyle = new GUIStyle(GUI.skin.label)
                {
                    fontStyle = FontStyle.Bold,
                    fontSize = 28
                };

                GUI.Label(new Rect(x + 10, y + 5, 400, 30), _speakerName, nameStyle);
            }

            // TEXT
            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                wordWrap = true
            };

            float textY = string.IsNullOrEmpty(_speakerName) ? y + 15 : y + 40;

            GUI.Label(new Rect(x + 10, textY, DialogueBoxWidth - 20, 100), _currentText, textStyle);

            // CHOICES
            if (_showingChoices && _choices != null)
            {
                for (int i = 0; i < _choices.Length; i++)
                {
                    GUI.Label(
                        new Rect(x + 20, y + 120 + (i * 22), 400, 20),
                        $"{i + 1}. {_choices[i]}"
                    );
                }
            }
            else
            {
                GUI.Label(new Rect(x + 10, y + DialogueBoxHeight - 25, 200, 20), "[E / Space]");
            }
        }

        // =========================================
        // NORMAL DIALOGUE
        // =========================================
        public void OpenDialogue(string text, string speaker = "")
        {
            _currentText = text;
            _speakerName = speaker;

            _showingChoices = false;
            _isDialogueOpen = true;

            SetPlayerMovement(false);
        }

        // =========================================
        // CHOICES
        // =========================================
        public void OpenChoices(string text, string speaker, string[] choices, System.Action<int> callback)
        {
            _currentText = text;
            _speakerName = speaker;

            _choices = choices;
            _onChoiceSelected = callback;
            _showingChoices = true;

            _isDialogueOpen = true;

            SetPlayerMovement(false);
        }

        // ⭐ FIXED VERSION — NO AUTO‑CLOSE ⭐
        private void SelectChoice(int index)
        {
            _showingChoices = false;

            // DO NOT close the dialogue here.
            // Let the callback open the next dialogue page normally.
            // This makes the player press E to continue.

            _onChoiceSelected?.Invoke(index);
        }

        public void CloseDialogue()
        {
            _isDialogueOpen = false;
            _currentText = "";
            _speakerName = "";

            SetPlayerMovement(true);

            OnDialogueClosed?.Invoke();
        }

        private void SetPlayerMovement(bool enabled)
        {
            ThirdPersonPlayerController player = FindFirstObjectByType<ThirdPersonPlayerController>();
            if (player != null)
                player.SetMovementEnabled(enabled);
        }
    }
}

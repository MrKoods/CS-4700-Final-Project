using UnityEngine;

namespace CS4700
{
    public class OverlookController : MonoBehaviour
    {
        public static OverlookController Instance;

        private ChronicleOfEchoes coe;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            coe = ChronicleOfEchoes.Instance;
        }

        public void StartConfrontation()
        {
            // Hide objective marker
            ObjectiveMarker.Instance?.Hide();

            // Decide which loop we are in
            if (!coe.Has("Knows_Abigail_Blackmail"))
            {
                Loop1_ForcedTragedy();
            }
            else if (!FinalConditionsMet())
            {
                MidLoop_PartialKnowledge();
            }
            else
            {
                FinalLoop_TruePath();
            }
        }

        private bool FinalConditionsMet()
        {
            return coe.Has("HasLedger") &&
                   coe.Has("HasLocket") &&
                   coe.Has("Knows_Caleb_Targeted");
        }

        // ---------------- LOOP 1 ----------------
        private void Loop1_ForcedTragedy()
        {
            DialogueManager.Instance.OpenDialogue(
                "The traveler has no shadow in the noon sun! Abigail—tell them what the Devil whispered!",
                "Caleb"
            );

            DialogueManager.Instance.OnDialogueClosed += AbigailLoop1;
        }

        private void AbigailLoop1()
        {
            DialogueManager.Instance.OnDialogueClosed -= AbigailLoop1;

            DialogueManager.Instance.OpenDialogue(
                "He… he came to me in the night… He brought the black book… Silas signed it… in blood!",
                "Abigail"
            );

            DialogueManager.Instance.OnDialogueClosed += ExecutionCutscene;
        }

        // ---------------- MID LOOP ----------------
        private void MidLoop_PartialKnowledge()
        {
            DialogueManager.Instance.OpenDialogue(
                "The traveler has no shadow—Abigail, speak!",
                "Caleb"
            );

            DialogueManager.Instance.OnDialogueClosed += MidLoop_Choices;
        }

        private void MidLoop_Choices()
        {
            DialogueManager.Instance.OnDialogueClosed -= MidLoop_Choices;

            DialogueManager.Instance.OpenChoices(
                "Silas must respond:",
                "Silas",
                new string[]
                {
                    "Caleb, she’s being forced!",
                    "Danforth is threatening her family!",
                    "Abigail, you don’t have to do this!"
                },
                MidLoop_ChoiceSelected
            );
        }

        private void MidLoop_ChoiceSelected(int index)
        {
            DialogueManager.Instance.OpenDialogue(
                "You shout truth into a sealed chamber. The man hears not—his grief is armor.",
                "Weaver"
            );

            DialogueManager.Instance.OnDialogueClosed += ResetLoop;
        }

        // ---------------- FINAL LOOP ----------------
        private void FinalLoop_TruePath()
        {
            DialogueManager.Instance.OpenDialogue(
                "The traveler has no shadow! Abigail, testify!",
                "Caleb"
            );

            DialogueManager.Instance.OnDialogueClosed += PresentLedger;
        }

        private void PresentLedger()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLedger;

            DialogueManager.Instance.OpenChoices(
                "Silas must act:",
                "Silas",
                new string[]
                {
                    "Present Ledger",
                    "Appeal to Abigail",
                    "Stay silent"
                },
                FinalChoiceSelected
            );
        }

        private void FinalChoiceSelected(int index)
        {
            if (index == 0)
            {
                DialogueManager.Instance.OpenDialogue(
                    "Caleb… before you condemn me—look at this.",
                    "Silas"
                );

                DialogueManager.Instance.OnDialogueClosed += PresentLocket;
            }
            else
            {
                DialogueManager.Instance.OpenDialogue(
                    "The loop shatters only when truth is wielded.",
                    "Weaver"
                );

                DialogueManager.Instance.OnDialogueClosed += ResetLoop;
            }
        }

        private void PresentLocket()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLocket;

            DialogueManager.Instance.OpenDialogue(
                "And this—Abigail’s locket. Danforth kept it.",
                "Silas"
            );

            DialogueManager.Instance.OnDialogueClosed += AbigailBreaks;
        }

        private void AbigailBreaks()
        {
            DialogueManager.Instance.OnDialogueClosed -= AbigailBreaks;

            DialogueManager.Instance.OpenDialogue(
                "It was him! Danforth made me say the names!",
                "Abigail"
            );

            DialogueManager.Instance.OnDialogueClosed += EndingSequence;
        }

        // ---------------- ENDING ----------------
        private void EndingSequence()
        {
            DialogueManager.Instance.OnDialogueClosed -= EndingSequence;

            DialogueManager.Instance.OpenDialogue(
                "Ah… the thread holds. Truth, at last, cuts deeper than fear.",
                "Weaver"
            );

            // No reset — loop breaks
        }

        // ---------------- RESET ----------------
        private void ExecutionCutscene()
        {
            DialogueManager.Instance.OnDialogueClosed -= ExecutionCutscene;

            DialogueManager.Instance.OpenDialogue(
                "A storm, and you bring no shelter.",
                "Weaver"
            );

            DialogueManager.Instance.OnDialogueClosed += ResetLoop;
        }

        private void ResetLoop()
        {
            DialogueManager.Instance.OnDialogueClosed -= ResetLoop;

            coe.ResetLoop();

            // Reload scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
    }
}

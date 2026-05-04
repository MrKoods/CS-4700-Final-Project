using UnityEngine;
using UnityEngine.SceneManagement;

namespace CS4700
{
    public class OverlookDialogueController : MonoBehaviour
    {
        private ChronicleOfEchoes coe;
        private bool dialogueStarted = false;

        private string storedWeaverLine = "";

        private void Start()
        {
            coe = ChronicleOfEchoes.Instance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (dialogueStarted) return;

            dialogueStarted = true;
            StartDialogue();
        }

        private void StartDialogue()
        {
            if (coe.Has("HasLedger") && coe.Has("HasLocket") &&
                coe.Has("Knows_Abigail_Blackmail") && coe.Has("Knows_Caleb_Targeted"))
            {
                FinalLoop_Start();
                return;
            }

            if (coe.Has("Knows_Abigail_Blackmail"))
            {
                MidLoop_Start();
                return;
            }

            Loop1_Start();
        }

        // ============================
        // LOOP 1 — FAILURE
        // ============================
        private void Loop1_Start()
        {
            DialogueManager.Instance.OpenChoices(
                "The traveler has no shadow in the noon sun! I saw it plain as iron!\nAbigail—tell them what the Devil whispered!",
                "Caleb",
                new string[]
                {
                    "That’s a lie!",
                    "Abigail, why are you doing this?",
                    "Stay silent"
                },
                Loop1_Choice
            );
        }

        private void Loop1_Choice(int index)
        {
            KillPlayer(
                "Take them both. The gallows will sort truth from lies.",
                WeaverLine_Loop1()
            );
        }

        // ============================
        // MID LOOP — FAILURE
        // ============================
        private void MidLoop_Start()
        {
            DialogueManager.Instance.OpenChoices(
                "The traveler has no shadow—Abigail, speak!",
                "Caleb",
                new string[]
                {
                    "Caleb, she’s being forced!",
                    "Danforth is threatening her family!",
                    "Abigail, you don’t have to do this!"
                },
                MidLoop_Choice
            );
        }

        private void MidLoop_Choice(int index)
        {
            KillPlayer(
                "You twist truth like a serpent. That is proof enough.",
                WeaverLine_MidLoop()
            );
        }

        // ============================
        // FINAL LOOP — TRUE PATH
        // ============================
        private void FinalLoop_Start()
        {
            DialogueManager.Instance.OpenChoices(
                "The traveler has no shadow! Abigail, testify!",
                "Caleb",
                new string[]
                {
                    "Present Ledger",
                    "Appeal to Abigail",
                    "Stay silent"
                },
                FinalLoop_Choice
            );
        }

        private void FinalLoop_Choice(int index)
        {
            if (index == 0)
            {
                PresentLedger();
                return;
            }

            if (index == 1)
            {
                AppealToAbigail();
                return;
            }

            KillPlayer(
                "The gallows will sort truth from lies.",
                WeaverLine_MidLoop()
            );
        }

        // ============================
        // APPEAL TO ABIGAIL (FAILURE)
        // ============================
        private void AppealToAbigail()
        {
            DialogueManager.Instance.OpenDialogue(
                "Abigail… look at me. You don’t have to do this.",
                "Silas"
            );

            DialogueManager.Instance.OnDialogueClosed += AppealToAbigail_Continue;
        }

        private void AppealToAbigail_Continue()
        {
            DialogueManager.Instance.OnDialogueClosed -= AppealToAbigail_Continue;

            DialogueManager.Instance.OpenDialogue(
                "Silas, stop! He’ll hear you!",
                "Abigail"
            );

            DialogueManager.Instance.OnDialogueClosed += AppealToAbigail_Fail;
        }

        private void AppealToAbigail_Fail()
        {
            DialogueManager.Instance.OnDialogueClosed -= AppealToAbigail_Fail;

            KillPlayer(
                "You twist truth like a serpent. That is proof enough.",
                WeaverLine_MidLoop()
            );
        }

        // ============================
        // PRESENT LEDGER (SUCCESS)
        // ============================
        private void PresentLedger()
        {
            DialogueManager.Instance.OpenDialogue(
                "Caleb… before you condemn me—look at this.",
                "Silas"
            );

            DialogueManager.Instance.OnDialogueClosed += PresentLedger_Continue;
        }

        private void PresentLedger_Continue()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLedger_Continue;

            DialogueManager.Instance.OpenDialogue(
                "…This is the Clerk’s seal.",
                "Caleb"
            );

            DialogueManager.Instance.OnDialogueClosed += PresentLedger_Date;
        }

        private void PresentLedger_Date()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLedger_Date;

            DialogueManager.Instance.OpenDialogue(
                "Check the date. He signed your smithy into seizure—three days before your accusation.",
                "Silas"
            );

            DialogueManager.Instance.OnDialogueClosed += PresentLedger_Shaken;
        }

        private void PresentLedger_Shaken()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLedger_Shaken;

            DialogueManager.Instance.OpenDialogue(
                "That’s… impossible…",
                "Caleb"
            );

            DialogueManager.Instance.OnDialogueClosed += PresentLocket_Start;
        }

        private void PresentLocket_Start()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLocket_Start;

            DialogueManager.Instance.OpenDialogue(
                "And this—the locket he claimed the Devil stole from you.",
                "Silas"
            );

            DialogueManager.Instance.OnDialogueClosed += PresentLocket_AbigailReact;
        }

        private void PresentLocket_AbigailReact()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLocket_AbigailReact;

            DialogueManager.Instance.OpenDialogue(
                "He… he said it was gone…",
                "Abigail"
            );

            DialogueManager.Instance.OnDialogueClosed += PresentLocket_Reveal;
        }

        private void PresentLocket_Reveal()
        {
            DialogueManager.Instance.OnDialogueClosed -= PresentLocket_Reveal;

            DialogueManager.Instance.OpenDialogue(
                "It was in Danforth’s desk. He lies. He owns your fear.",
                "Silas"
            );

            DialogueManager.Instance.OnDialogueClosed += FinalConfession;
        }

        private void FinalConfession()
        {
            DialogueManager.Instance.OnDialogueClosed -= FinalConfession;

            DialogueManager.Instance.OpenDialogue(
                "It was him! Danforth made me say the names! He said he’d take my family if I didn’t!",
                "Abigail"
            );

            DialogueManager.Instance.OnDialogueClosed += WeaverSuccess;
        }

        private void WeaverSuccess()
        {
            DialogueManager.Instance.OnDialogueClosed -= WeaverSuccess;

            DialogueManager.Instance.OpenDialogue(
                "Ah… the thread holds.\nTruth, at last, cuts deeper than fear.",
                "Weaver"
            );

            DialogueManager.Instance.OnDialogueClosed += Ending;
        }

        private void Ending()
        {
            DialogueManager.Instance.OnDialogueClosed -= Ending;
            SceneManager.LoadScene("CreditsScene");
        }

        // ============================
        // EXECUTION / RESET
        // ============================
        private void KillPlayer(string calebLine, string weaverLine)
        {
            storedWeaverLine = weaverLine;

            DialogueManager.Instance.OpenDialogue(calebLine, "Caleb");
            DialogueManager.Instance.OnDialogueClosed += WeaverCommentary;
        }

        private void WeaverCommentary()
        {
            DialogueManager.Instance.OnDialogueClosed -= WeaverCommentary;

            DialogueManager.Instance.OpenDialogue(storedWeaverLine, "Weaver");
            DialogueManager.Instance.OnDialogueClosed += ResetLoop;
        }

        private void ResetLoop()
        {
            DialogueManager.Instance.OnDialogueClosed -= ResetLoop;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ============================
        // WEAVER LINES
        // ============================
        private string WeaverLine_Loop1()
        {
            return "A storm, and you bring no shelter.\nYou stand upon the board, yet know not the game.";
        }

        private string WeaverLine_MidLoop()
        {
            return "You shout truth into a sealed chamber.\nThe man hears not—his grief is armor.\nBring not words… bring something that breaks.";
        }
    }
}

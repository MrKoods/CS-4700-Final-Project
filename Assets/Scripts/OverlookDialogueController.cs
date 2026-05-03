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
            // FINAL LOOP — all key knowledge
            if (coe.Has("HasLedger") && coe.Has("HasLocket") &&
                coe.Has("Knows_Abigail_Blackmail") && coe.Has("Knows_Caleb_Targeted"))
            {
                FinalLoop_Start();
                return;
            }

            // MID LOOP — knows Abigail is being blackmailed
            if (coe.Has("Knows_Abigail_Blackmail"))
            {
                MidLoop_Start();
                return;
            }

            // LOOP 1 — no knowledge
            Loop1_Start();
        }

        // ============================
        // LOOP 1 — The Descent of Shadow (FAILURE)
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
            // No matter what you pick in Loop 1, you die.
            KillPlayer(
                "Take them both. The gallows will sort truth from lies.",
                WeaverLine_Loop1()
            );
        }

        // ============================
        // MID LOOP — The Bitter Counsel (FAILURE)
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
            // Still failure: Caleb’s grief is armor.
            KillPlayer(
                "You twist truth like a serpent. That is proof enough.",
                WeaverLine_MidLoop()
            );
        }

        // ============================
        // FINAL LOOP — The Breaking of the Loom (TRUE PATH)
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

            // Staying silent in the final loop is still failure.
            KillPlayer(
                "The gallows will sort truth from lies.",
                WeaverLine_MidLoop() // still about pulling without proof
            );
        }

        // ============================
        // APPEAL TO ABIGAIL (FAILURE PATH)
        // ============================
        private void AppealToAbigail()
        {
            DialogueManager.Instance.OpenDialogue(
                "Abigail… look at me. You don’t have to do this.",
                "Silas"
            );

            Invoke(nameof(AppealToAbigail_Continue), 2f);
        }

        private void AppealToAbigail_Continue()
        {
            DialogueManager.Instance.OpenDialogue(
                "Silas, stop! He’ll hear you!",
                "Abigail"
            );

            Invoke(nameof(AppealToAbigail_Fail), 2f);
        }

        private void AppealToAbigail_Fail()
        {
            KillPlayer(
                "You twist truth like a serpent. That is proof enough.",
                WeaverLine_MidLoop()
            );
        }

        // ============================
        // PRESENT LEDGER (SUCCESS PATH)
        // ============================
        private void PresentLedger()
        {
            DialogueManager.Instance.OpenDialogue(
                "Caleb… before you condemn me—look at this.",
                "Silas"
            );

            Invoke(nameof(PresentLedger_Continue), 2f);
        }

        private void PresentLedger_Continue()
        {
            DialogueManager.Instance.OpenDialogue(
                "…This is the Clerk’s seal.",
                "Caleb"
            );

            Invoke(nameof(PresentLedger_Date), 2f);
        }

        private void PresentLedger_Date()
        {
            DialogueManager.Instance.OpenDialogue(
                "Check the date. He signed your smithy into seizure—three days before your accusation.",
                "Silas"
            );

            Invoke(nameof(PresentLedger_Shaken), 2f);
        }

        private void PresentLedger_Shaken()
        {
            DialogueManager.Instance.OpenDialogue(
                "That’s… impossible…",
                "Caleb"
            );

            Invoke(nameof(PresentLocket_Start), 2f);
        }

        private void PresentLocket_Start()
        {
            DialogueManager.Instance.OpenDialogue(
                "And this—the locket he claimed the Devil stole from you.",
                "Silas"
            );

            Invoke(nameof(PresentLocket_AbigailReact), 2f);
        }

        private void PresentLocket_AbigailReact()
        {
            DialogueManager.Instance.OpenDialogue(
                "He… he said it was gone…",
                "Abigail"
            );

            Invoke(nameof(PresentLocket_Reveal), 2f);
        }

        private void PresentLocket_Reveal()
        {
            DialogueManager.Instance.OpenDialogue(
                "It was in Danforth’s desk. He lies. He owns your fear.",
                "Silas"
            );

            Invoke(nameof(FinalConfession), 2f);
        }

        private void FinalConfession()
        {
            DialogueManager.Instance.OpenDialogue(
                "It was him! Danforth made me say the names! He said he’d take my family if I didn’t!",
                "Abigail"
            );

            Invoke(nameof(WeaverSuccess), 3f);
        }

        private void WeaverSuccess()
        {
            DialogueManager.Instance.OpenDialogue(
                "Ah… the thread holds.\nTruth, at last, cuts deeper than fear.",
                "Weaver"
            );

            Invoke(nameof(Ending), 3f);
        }

        private void Ending()
        {
            SceneManager.LoadScene("CreditsScene");
        }

        // ============================
        // EXECUTION / RESET
        // ============================
        private void KillPlayer(string calebLine, string weaverLine)
        {
            storedWeaverLine = weaverLine;

            DialogueManager.Instance.OpenDialogue(calebLine, "Caleb");
            Invoke(nameof(WeaverCommentary), 2f);
        }

        private void WeaverCommentary()
        {
            DialogueManager.Instance.OpenDialogue(storedWeaverLine, "Weaver");
            Invoke(nameof(ResetLoop), 3f);
        }

        private void ResetLoop()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // ============================
        // WEAVER LINES (FROM MASTER SCRIPT)
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

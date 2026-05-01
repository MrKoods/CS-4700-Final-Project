using UnityEngine;
using System.Collections;

namespace CS4700
{
    public class NarrativeController : MonoBehaviour
    {
        public static NarrativeController Instance;

        private DialogueManager D;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            D = DialogueManager.Instance;
        }

        public void StartOverlookScene()
        {
            StartCoroutine(RunScene());
        }

        IEnumerator RunScene()
        {
            yield return new WaitForSeconds(0.5f);

            if (ChronicleOfEchoes.Instance.HasLedger &&
                ChronicleOfEchoes.Instance.HasLocket &&
                ChronicleOfEchoes.Instance.KnowsCalebTargeted)
            {
                yield return FinalLoop();
            }
            else if (ChronicleOfEchoes.Instance.KnowsAbigailBlackmail)
            {
                yield return MidLoop();
            }
            else
            {
                yield return Loop1();
            }
        }

        // =========================================
        // LOOP 1
        // =========================================
        IEnumerator Loop1()
        {
            D.OpenDialogue("The traveler has no shadow! Abigail, speak!", "Caleb");
            yield return Wait();

            D.OpenDialogue("He... he came to me in the night...", "Abigail");
            yield return Wait();

            D.OpenChoices(
                "What do you say?",
                "Silas",
                new string[]
                {
                    "That's a lie!",
                    "Abigail, why are you doing this?",
                    "Stay silent"
                },
                OnLoop1Choice
            );
        }

        void OnLoop1Choice(int choice)
        {
            if (choice == 2)
                D.OpenDialogue("See how he hides behind silence?", "Caleb");
            else
                D.OpenDialogue("Silence, wretch!", "Caleb");

            StartCoroutine(Fail());
        }

        // =========================================
        // MID LOOP
        // =========================================
        IEnumerator MidLoop()
        {
            D.OpenDialogue("Abigail, speak!", "Caleb");
            yield return Wait();

            D.OpenChoices(
                "You know more now...",
                "Silas",
                new string[]
                {
                    "Caleb, she’s being forced!",
                    "Danforth is threatening her family!",
                    "Abigail, you don’t have to do this!"
                },
                OnMidChoice
            );
        }

        void OnMidChoice(int choice)
        {
            D.OpenDialogue("You twist truth like a serpent.", "Caleb");
            StartCoroutine(Fail());
        }

        // =========================================
        // FINAL LOOP
        // =========================================
        IEnumerator FinalLoop()
        {
            D.OpenDialogue("Caleb... look at this ledger.", "Silas");
            yield return Wait();

            D.OpenDialogue("This is the Clerk’s seal...", "Caleb");
            yield return Wait();

            D.OpenDialogue("He planned your downfall.", "Silas");
            yield return Wait();

            D.OpenDialogue("And this locket—he stole it.", "Silas");
            yield return Wait();

            D.OpenDialogue("It was him! Danforth made me!", "Abigail");
            yield return Wait();

            D.OpenDialogue("Fetch him.", "Caleb");
            yield return Wait();

            D.OpenDialogue("The loop is broken.", "Weaver");
        }

        // =========================================
        IEnumerator Fail()
        {
            yield return Wait();

            D.OpenDialogue("You failed again.", "Weaver");
            yield return Wait();

            ChronicleOfEchoes.Instance.ResetLoop();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        IEnumerator Wait()
        {
            while (DialogueManager.Instance.IsDialogueOpen)
                yield return null;
        }
    }
}
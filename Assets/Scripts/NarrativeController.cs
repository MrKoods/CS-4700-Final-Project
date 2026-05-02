using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CS4700
{
    public class NarrativeController : MonoBehaviour
    {
        public static NarrativeController Instance;

        private DialogueManager D;

        // ============================
        // NEW NARRATIVE SYSTEM
        // ============================
        public int currentLoop = 1;
        public string currentStoryBeat = "Day1_Start";

        public ChronicleOfEchoes knowledge;

        private Dictionary<string, List<DialogueRule>> npcDialogueRules;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            D = DialogueManager.Instance;

            if (knowledge == null)
                knowledge = ChronicleOfEchoes.Instance;

            BuildDialogueRules();
        }

        // ============================
        // NPCs call this to get correct dialogue
        // ============================
        public DialogueLine GetDialogueForNPC(string npcName)
        {
            if (!npcDialogueRules.ContainsKey(npcName))
                return new DialogueLine("I have nothing to say right now.");

            foreach (DialogueRule rule in npcDialogueRules[npcName])
            {
                if (rule.ConditionMet(knowledge))
                    return rule.line;
            }

            return new DialogueLine("…");
        }

        // ============================
        // Build narrative logic
        // ============================
        private void BuildDialogueRules()
        {
            npcDialogueRules = new Dictionary<string, List<DialogueRule>>();

            // CALEB
            npcDialogueRules["Caleb"] = new List<DialogueRule>()
            {
                new DialogueRule(
                    k => !k.Has("Knows_Caleb_Grief"),
                    new DialogueLine("I don't want to talk about it.")
                ),

                new DialogueRule(
                    k => k.Has("Knows_Caleb_Grief") && !k.Has("Knows_Danforth_Manipulation"),
                    new DialogueLine("I miss her… I can't move on.")
                ),

                new DialogueRule(
                    k => k.Has("Knows_Danforth_Manipulation"),
                    new DialogueLine("Danforth… he used us. All of us.")
                )
            };

            // ABIGAIL
            npcDialogueRules["Abigail"] = new List<DialogueRule>()
            {
                new DialogueRule(
                    k => !k.Has("Knows_Abigail_Lying"),
                    new DialogueLine("I saw the devil… I swear it.")
                ),

                new DialogueRule(
                    k => k.Has("Knows_Abigail_Lying"),
                    new DialogueLine("Please… don’t tell anyone what I told you.")
                )
            };

            // WEAVER
            npcDialogueRules["Weaver"] = new List<DialogueRule>()
            {
                new DialogueRule(
                    k => true,
                    new DialogueLine("Truth bends, but it does not break. Follow the threads.")
                )
            };
        }

        // ============================
        // LOOP / DAY CONTROL
        // ============================
        public void StartDayOne()
        {
            currentLoop = 1;
            currentStoryBeat = "Day1_Start";
        }

        public void ResetLoop()
        {
            currentLoop++;
            currentStoryBeat = "Day1_Start";
        }

        // ============================
        // YOUR ORIGINAL OVERLOOK SCENE
        // ============================
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

        // LOOP 1
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

        // MID LOOP
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

        // FINAL LOOP
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

        // FAIL
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

    // ============================
    // SUPPORTING CLASSES
    // ============================
    public class DialogueRule
    {
        public System.Func<ChronicleOfEchoes, bool> ConditionMet;
        public DialogueLine line;

        public DialogueRule(System.Func<ChronicleOfEchoes, bool> condition, DialogueLine line)
        {
            ConditionMet = condition;
            this.line = line;
        }
    }

    public class DialogueLine
    {
        public string text;

        public DialogueLine(string t)
        {
            text = t;
        }
    }
}

using UnityEngine;

namespace CS4700
{
    public class NarrativeController : MonoBehaviour
    {
        public static NarrativeController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public DialogueLine GetDialogueForNPC(string npcName)
        {
            switch (npcName)
            {
                case "Abigail":
                    return GetAbigailLine();

                case "Weaver":
                    return GetWeaverLine();

                case "FemaleVillager":
                    return GetFemaleVillagerLine();

                default:
                    return new DialogueLine("…", npcName);
            }
        }

        // ---------------- ABIGAIL ----------------

        private DialogueLine GetAbigailLine()
        {
            var coe = ChronicleOfEchoes.Instance;

            // She is still hiding the truth, no confession yet
            if (!coe.Has("Knows_Abigail_Lying"))
            {
                // If player already knows about blackmail, push her to confess
                if (coe.Has("Knows_Abigail_Blackmail"))
                {
                    // This is the confession moment – triggers knowledge
                    return new DialogueLine(
                        "Fine… I lied. I didn’t see Caleb that night. I was scared. I didn’t know what else to say.",
                        "Abigail",
                        true // triggersKnowledge -> Knows_Abigail_Lying via KnowledgeTrigger on Abigail
                    );
                }

                // Early loop – she’s evasive
                return new DialogueLine(
                    "I told you what I saw. Caleb was there. Isn’t that enough?",
                    "Abigail"
                );
            }

            // After confession
            return new DialogueLine(
                "I never wanted this. I just wanted it all to stop.",
                "Abigail"
            );
        }

        // ---------------- WEAVER ----------------

        private DialogueLine GetWeaverLine()
        {
            var coe = ChronicleOfEchoes.Instance;

            // Has the locket and hasn't heard Weaver's grief yet
            if (coe.Has("HasLocket") && !coe.Has("Knows_Caleb_Grief"))
            {
                return new DialogueLine(
                    "Caleb… he wasn’t himself. He carried a weight I couldn’t lift. I should’ve done more.",
                    "Weaver",
                    true // triggersKnowledge -> Knows_Caleb_Grief via KnowledgeTrigger on Weaver
                );
            }

            // After grief reveal
            if (coe.Has("Knows_Caleb_Grief"))
            {
                return new DialogueLine(
                    "Grief twists the truth. Be careful what you think you know.",
                    "Weaver"
                );
            }

            // Default Weaver line
            return new DialogueLine(
                "Threads fray where the town refuses to look.",
                "Weaver"
            );
        }

        // ---------------- FEMALE VILLAGER ----------------

        private DialogueLine GetFemaleVillagerLine()
        {
            var coe = ChronicleOfEchoes.Instance;

            // She reveals Caleb was being targeted
            if (!coe.Has("Knows_Caleb_Targeted"))
            {
                return new DialogueLine(
                    "Someone was watching Caleb. Following him. He told me he felt hunted.",
                    "Villager",
                    true // triggersKnowledge -> Knows_Caleb_Targeted via KnowledgeTrigger on this NPC
                );
            }

            // After reveal
            return new DialogueLine(
                "I still see him in the square sometimes… in my mind.",
                "Villager"
            );
        }
    }
}

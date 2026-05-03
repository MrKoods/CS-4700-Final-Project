using UnityEngine;

namespace CS4700
{
    [System.Serializable]
    public class DialogueLine
    {
        public string text;
        public string speaker;

        // If true, InteractableNPC will try to trigger a KnowledgeTrigger on this NPC
        public bool triggersKnowledge;

        public DialogueLine(string text, string speaker, bool triggersKnowledge = false)
        {
            this.text = text;
            this.speaker = speaker;
            this.triggersKnowledge = triggersKnowledge;
        }
    }
}

using UnityEngine;

namespace CS4700
{
    public class WeaverDialogue : InteractableNPC
    {
        public override string GetDialogue()
        {
            if (LoopState.FinalLoop())
                return "The loom quiets… the pattern nears its end. You’ve tugged every thread, Silas.";

            if (LoopState.MidLoop())
                return "The pattern shifts. You feel it, don’t you? Something frays beneath the surface.";

            return "Threads cross whether we wish them to or not.";
        }
    }
}

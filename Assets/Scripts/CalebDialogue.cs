using UnityEngine;

namespace CS4700
{
    public class CalebDialogue : InteractableNPC
    {
        public override string GetDialogue()
        {
            if (LoopState.FinalLoop())
                return "So you’ve seen it too… the lies, the fear. Danforth’s shadow reaches farther than any of us knew.";

            if (LoopState.MidLoop())
                return "You walk strange paths, traveler. If you’re digging for truth, be ready for what you find.";

            return "The forge keeps me busy. Busy keeps the mind from wandering.";
        }
    }
}

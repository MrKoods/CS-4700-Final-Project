using UnityEngine;

namespace CS4700
{
    public class FemaleMedievalNPCDialogue : InteractableNPC
    {
        public override string GetDialogue()
        {
            if (LoopState.FinalLoop())
                return "Folk whisper that the truth is finally surfacing… I pray they’re right.";

            return "Something’s wrong in this village… I feel it in my bones.";
        }
    }
}

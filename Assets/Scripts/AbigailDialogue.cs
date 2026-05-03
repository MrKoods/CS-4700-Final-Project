using UnityEngine;

namespace CS4700
{
    public class AbigailDialogue : InteractableNPC
    {
        public override string GetDialogue()
        {
            var coe = ChronicleOfEchoes.Instance;

            if (LoopState.FinalLoop())
                return "You uncovered everything… the ledger, the locket… I can’t hide anymore. Danforth forced my hand, Silas.";

            if (LoopState.MidLoop())
                return "You’re digging into things that will get us both hurt. Danforth doesn’t forgive curiosity.";

            if (!coe.Has("Knows_Abigail_Blackmail"))
            {
                coe.Unlock("Knows_Abigail_Blackmail");
                return "He said he’d take my family if I didn’t speak the names… I never wanted anyone hurt.";
            }

            return "Please… I don’t want trouble. Folk here are quick to judge and slow to listen.";
        }
    }
}

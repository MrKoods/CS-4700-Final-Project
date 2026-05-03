using UnityEngine;

namespace CS4700
{
    public class DanforthDialogue : InteractableNPC
    {
        public override string GetDialogue()
        {
            var coe = ChronicleOfEchoes.Instance;

            if (LoopState.FinalLoop())
                return "You think those scraps of evidence will save this village? Truth is a blade, Silas… and you’re holding it by the edge.";

            if (LoopState.MidLoop())
                return "You pry into matters far above your station. Best you forget what you think you’ve found.";

            return "Good day, traveler. Folk here respect order — it keeps chaos at bay.";
        }
    }
}

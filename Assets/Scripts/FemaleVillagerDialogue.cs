using UnityEngine;

namespace CS4700
{
    public class FemaleVillagerDialogue : InteractableNPC
    {
        public override string GetDialogue()
        {
            var coe = ChronicleOfEchoes.Instance;

            if (!coe.Has("Knows_Caleb_Targeted"))
            {
                coe.Unlock("Knows_Caleb_Targeted");
                return "Caleb told me he felt watched… hunted. Someone had eyes on him long before the accusations.";
            }

            if (LoopState.FinalLoop())
                return "You’ve stirred the nest, traveler. Folk feel it. Something’s coming.";

            return "Some footsteps at night don’t belong to any villager.";
        }
    }
}

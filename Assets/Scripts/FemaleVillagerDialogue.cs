using UnityEngine;

namespace CS4700
{
    public class FemaleVillagerDialogue : MonoBehaviour
    {
        public string GetDialogue()
        {
            var coe = ChronicleOfEchoes.Instance;

            if (!coe.Has("Knows_Caleb_Targeted"))
            {
                coe.Unlock("Knows_Caleb_Targeted");
                return "I heard whispers… the Clerk had plans for Caleb’s land long before the accusations.";
            }

            return "Poor Caleb… he never saw it coming.";
        }
    }
}

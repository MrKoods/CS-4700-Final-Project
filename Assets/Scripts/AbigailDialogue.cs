using UnityEngine;

namespace CS4700
{
    public class AbigailDialogue : MonoBehaviour
    {
        public string GetDialogue()
        {
            var coe = ChronicleOfEchoes.Instance;

            if (!coe.Has("Knows_Abigail_Blackmail"))
            {
                coe.Unlock("Knows_Abigail_Blackmail");
                return "Please… don’t ask me. He said he’d take my family if I didn’t say the names.";
            }

            return "I never wanted to lie… I’m sorry.";
        }
    }
}

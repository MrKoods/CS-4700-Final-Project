using UnityEngine;

namespace CS4700
    {
        public class MaleMedievalNPCDialogue : InteractableNPC
        {
            public override string GetDialogue()
            {
                if (LoopState.FinalLoop())
                    return "Feels like a storm’s about to break. Not weather… something worse.";

                return "People look over their shoulders more than they look ahead.";
            }
        }
    }

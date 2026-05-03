using UnityEngine;

namespace CS4700
{
    public static class LoopState
    {
        public static bool EarlyLoop()
        {
            var coe = ChronicleOfEchoes.Instance;
            return !coe.Has("HasLedger") && !coe.Has("HasLocket");
        }

        public static bool MidLoop()
        {
            var coe = ChronicleOfEchoes.Instance;
            return coe.Has("HasLedger") || coe.Has("HasLocket");
        }

        public static bool FinalLoop()
        {
            var coe = ChronicleOfEchoes.Instance;
            return coe.Has("HasLedger") &&
                   coe.Has("HasLocket") &&
                   coe.Has("Knows_Abigail_Blackmail") &&
                   coe.Has("Knows_Caleb_Targeted");
        }
    }
}

using UnityEngine;

namespace CS4700
{
    public static class LoopState
    {
        // LOOP 1 — no blackmail discovered
        public static bool EarlyLoop()
        {
            var coe = ChronicleOfEchoes.Instance;
            return !coe.Has("Knows_Abigail_Blackmail");
        }

        // MID LOOP — blackmail known, but not all clues
        public static bool MidLoop()
        {
            var coe = ChronicleOfEchoes.Instance;
            return coe.Has("Knows_Abigail_Blackmail") &&
                   !coe.AllCluesFound();
        }

        // FINAL LOOP — all clues found
        public static bool FinalLoop()
        {
            var coe = ChronicleOfEchoes.Instance;
            return coe.AllCluesFound();
        }
    }
}

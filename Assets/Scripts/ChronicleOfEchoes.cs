using UnityEngine;
using System.Collections.Generic;

namespace CS4700
{
    public class ChronicleOfEchoes : MonoBehaviour
    {
        public static ChronicleOfEchoes Instance;

        // ============================
        // OLD FIELDS (kept for Overlook scene)
        // ============================
        public bool HasLedger = false;
        public bool HasLocket = false;
        public bool KnowsCalebTargeted = false;
        public bool KnowsAbigailBlackmail = false;

        // ============================
        // NEW KNOWLEDGE FLAG SYSTEM
        // ============================
        private HashSet<string> knowledgeFlags = new HashSet<string>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ============================
        // PUBLIC API
        // ============================

        // Unlock a new piece of knowledge
        public void Unlock(string flag)
        {
            if (!knowledgeFlags.Contains(flag))
                knowledgeFlags.Add(flag);
        }

        // Check if Silas knows something
        public bool Has(string flag)
        {
            return knowledgeFlags.Contains(flag);
        }

        // Reset loop but KEEP knowledge
        public void ResetLoop()
        {
            // Loop resets world state, but knowledge persists
            // (Do NOT clear knowledgeFlags)
        }

        // Hard reset (if you ever need it)
        public void ResetAll()
        {
            knowledgeFlags.Clear();

            HasLedger = false;
            HasLocket = false;
            KnowsCalebTargeted = false;
            KnowsAbigailBlackmail = false;
        }
    }
}

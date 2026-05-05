using UnityEngine;
using System.Collections.Generic;

namespace CS4700
{
    public class ChronicleOfEchoes : MonoBehaviour
    {
        public static ChronicleOfEchoes Instance;

        // ============================
        // OLD FIELDS (visible in Inspector)
        // ============================
        [Header("Legacy Flags (Inspector Only)")]
        public bool HasLedger = false;
        public bool HasLocket = false;
        public bool KnowsCalebTargeted = false;
        public bool KnowsAbigailBlackmail = false;

        // ============================
        // NEW KNOWLEDGE FLAG SYSTEM
        // ============================
        [Header("Active Knowledge Flags (Debug Only)")]
        [SerializeField] private List<string> debugFlags = new List<string>();

        private HashSet<string> knowledgeFlags = new HashSet<string>();

        // ============================
        // FIXED SINGLETON
        // ============================
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        // ============================
        // FLAG UNLOCKING
        // ============================
        public void Unlock(string flag)
        {
            if (!knowledgeFlags.Contains(flag))
            {
                knowledgeFlags.Add(flag);
                Debug.Log("UNLOCKED FLAG: " + flag);

                // Sync to Inspector list for debugging
                debugFlags.Clear();
                debugFlags.AddRange(knowledgeFlags);

                // Sync legacy booleans for compatibility
                if (flag == "HasLedger") HasLedger = true;
                if (flag == "HasLocket") HasLocket = true;
                if (flag == "Knows_Caleb_Targeted") KnowsCalebTargeted = true;
                if (flag == "Knows_Abigail_Blackmail") KnowsAbigailBlackmail = true;
            }
        }

        public bool Has(string flag)
        {
            return knowledgeFlags.Contains(flag);
        }

        // ============================
        // LOOP RESET (knowledge persists)
        // ============================
        public void ResetLoop()
        {
            // Knowledge persists across loops
        }

        public void ResetAll()
        {
            knowledgeFlags.Clear();
            debugFlags.Clear();

            HasLedger = false;
            HasLocket = false;
            KnowsCalebTargeted = false;
            KnowsAbigailBlackmail = false;
        }

        // ============================
        // CHECK IF ALL CLUES ARE FOUND
        // ============================
        public bool AllCluesFound()
        {
            return Has("HasLedger") &&
                   Has("HasLocket") &&
                   Has("Knows_Caleb_Targeted") &&
                   Has("Knows_Abigail_Blackmail");
        }
    }
}

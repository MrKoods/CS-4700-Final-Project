using UnityEngine;
using TMPro;

namespace CS4700
{
    public class DirectionUI : MonoBehaviour
    {
        public static DirectionUI Instance;

        [SerializeField] private TMP_Text directionText;

        private void Awake()
        {
            // Always assign instance — no singleton destruction
            Instance = this;

            // Auto-find the text if not assigned
            if (directionText == null)
                directionText = GetComponentInChildren<TMP_Text>(true);
        }

        public void SetDirection(string text)
        {
            if (directionText != null)
                directionText.text = text;
            else
                Debug.LogError("DirectionUI: directionText is NULL!");
        }
    }
}

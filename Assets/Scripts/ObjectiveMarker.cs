using UnityEngine;

namespace CS4700
{
    public class ObjectiveMarker : MonoBehaviour
    {
        public static ObjectiveMarker Instance;

        public float floatSpeed = 1f;
        public float floatHeight = 0.2f;
        public float pulseSpeed = 2f;
        public float pulseAmount = 0.1f;

        private Vector3 startPos;
        private Vector3 startScale;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            startPos = transform.localPosition;
            startScale = transform.localScale;
        }

        private void Update()
        {
            // Floating up/down
            float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            transform.localPosition = startPos + new Vector3(0, yOffset, 0);

            // Pulsing scale
            float scaleOffset = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = startScale * scaleOffset;

            // Always face the player
            if (Camera.main != null)
                transform.LookAt(Camera.main.transform);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}

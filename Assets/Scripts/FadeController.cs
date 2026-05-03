using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CS4700
{
    public class FadeController : MonoBehaviour
    {
        public static FadeController Instance;

        public Image fadeImage;
        public float fadeDuration = 1f;

        private void Awake()
        {
            Instance = this;
        }

        public IEnumerator FadeOutRoutine()
        {
            float t = 0f;
            Color c = fadeImage.color;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }
        }
    }
}

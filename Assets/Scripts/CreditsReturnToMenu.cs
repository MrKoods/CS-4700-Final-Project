using UnityEngine;
using UnityEngine.SceneManagement;

namespace CS4700
{
    public class CreditsReturnToMenu : MonoBehaviour
    {
        public float delay = 5f; // seconds before returning to menu

        private void Start()
        {
            Invoke(nameof(ReturnToMenu), delay);
        }

        private void ReturnToMenu()
        {
            SceneManager.LoadScene("StartMenu");
        }
    }
}

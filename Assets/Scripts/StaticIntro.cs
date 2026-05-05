using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticIntro : MonoBehaviour
{
    public string nextScene = "MainScene";
    public float minDisplayTime = 2f;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;

        // After 2 seconds, allow ANY key to continue
        if (timer >= minDisplayTime && Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}

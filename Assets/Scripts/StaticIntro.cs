using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticIntro : MonoBehaviour
{
    public string nextScene = "MainScene";
    public float minDisplayTime = 2f;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= minDisplayTime && Input.anyKeyDown)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}

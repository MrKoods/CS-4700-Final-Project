using UnityEngine;

public class PageTurnSound : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioSource audioSource;

    public AudioClip clip;

    public float volume = 1;
    
    public void TurnPage()
    {
        if (audioSource == null) return;

        audioSource.PlayOneShot(clip, volume);
    }
}

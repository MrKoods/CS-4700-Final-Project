using UnityEngine;

public class MusicLooper : MonoBehaviour {
public AudioSource audioSource;
public AudioClip intro;
public AudioClip loop;

void Start()
{
    StartCoroutine(PlayMusic());
}

System.Collections.IEnumerator PlayMusic()
{
    audioSource.clip = intro;
    audioSource.loop = false;
    audioSource.Play();

    yield return new WaitForSeconds(intro.length - (float)0.5);

    audioSource.clip = loop;
    audioSource.loop = true;
    audioSource.Play();
}
}
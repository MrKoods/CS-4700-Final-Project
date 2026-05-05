using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;

public class OutdoorIndoor : MonoBehaviour
{
    public FootstepAudio footstepAudio;
    public AudioMixer musicMixer;
    public AudioMixerSnapshot[] snapshots; 
    private float[] indoorWeights = new float[2];
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        indoorWeights[0] = 0f;
        indoorWeights[1] = 1f;
        footstepAudio.isIndoor = true;
        UnityEngine.Debug.Log("Entered house");
        musicMixer.TransitionToSnapshots(snapshots, indoorWeights, 1);
    }

    void OnTriggerExit(Collider other)
    {
        indoorWeights[0] = 1f;
        indoorWeights[1] = 0f;
        footstepAudio.isIndoor = false;
        UnityEngine.Debug.Log("Left House");
        musicMixer.TransitionToSnapshots(snapshots, indoorWeights, 1);
    }
}

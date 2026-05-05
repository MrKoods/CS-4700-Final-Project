using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public Animator animator;
    public AudioClip leftFootstepOutdoor;
    public AudioClip rightFootstepOutdoor;

    public AudioClip leftFootstepIndoor;
    
    public AudioClip rightFootstepIndoor;
    public float walkStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;

    public float walkVolume = 0.8f;
    public float sprintVolume = 1f;
    private float stepTimer;

    private float stepVolume;
    private bool isLeftStep = true;

    public bool isIndoor = false;

    void Update()
    {
        float interval = GetCurrentStepInterval();

        if (interval <= 0f)
        {
            stepTimer = 0f;
            return;
        }

        stepTimer -= Time.deltaTime;

        if (stepTimer <= 0f)
        {
            PlayFootstep();
            stepTimer = interval;
        }
    }

    float GetCurrentStepInterval()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Sprint")) {
            stepVolume = sprintVolume;
           return sprintStepInterval;
        }
        
        if (!state.IsName("Idle") && !state.IsName("Jump")) {
            stepVolume = walkVolume;
            return walkStepInterval;
        }
        return 0f;
    }

    void PlayFootstep()
    {
        if (audioSource == null) return;
        AudioClip clip;
        if(isIndoor) {
            clip = isLeftStep ? leftFootstepIndoor : rightFootstepIndoor;
                    audioSource.PlayOneShot(clip, stepVolume/2);
        }
        else {
            clip = isLeftStep ? leftFootstepOutdoor : rightFootstepOutdoor;
                    audioSource.PlayOneShot(clip, stepVolume);
        }



        isLeftStep = !isLeftStep;
    }
}
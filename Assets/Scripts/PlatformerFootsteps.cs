using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlatformerFootsteps : MonoBehaviour
{
    [Header("Platformer Running Audio")]
    public AudioClip runningClip;
    [Range(0f, 1f)]
    public float volume = 0.6f;

    private AudioSource audioSource;
    private Animator anim;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.clip = runningClip;

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (runningClip == null) return;

        bool isMovingLeftOrRight =
            Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        bool isGrounded = anim != null ? anim.GetBool("IsGrounded") : true;

        if (isMovingLeftOrRight && isGrounded)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.clip = runningClip;
                audioSource.volume = volume;
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}

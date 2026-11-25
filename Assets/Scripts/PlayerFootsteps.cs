using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Footstep Audio")]
    public AudioClip walkingClip; // Drag your walking sound clip here
    [Range(0f, 1f)]
    public float volume = 0.6f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
        audioSource.clip = walkingClip;
    }

    void Update()
    {
        if (walkingClip == null) return;

        // Checks WASD and all 4 Arrow keys
        bool isPressingMoveKey =
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        if (isPressingMoveKey)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = volume;
                audioSource.Play();
            }
        }
        else
        {
            // Cuts the sound off immediately the exact frame you stop moving
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}

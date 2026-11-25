using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour
{
    [Header("Music Settings")]
    public AudioClip bgmClip;
    [Range(0f, 1f)]
    public float targetVolume = 0.5f;
    public float fadeInDuration = 2.0f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.volume = 0f;
            audioSource.Play();
            StartCoroutine(FadeInMusic());
        }
    }

    IEnumerator FadeInMusic()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeInDuration);
            yield return null;
        }
        audioSource.volume = targetVolume;
    }
}

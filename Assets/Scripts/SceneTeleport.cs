using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTeleport : MonoBehaviour
{
    [Header("Destination")]
    public string sceneToLoad;

    [Header("Fade Settings")]
    public CanvasGroup fadeCanvasGroup; // Drag a pure black UI Panel (with CanvasGroup) here
    public float fadeDuration = 0.8f;

    private static bool shouldFadeIn = false;
    private static float savedFadeDuration = 0.8f;
    private bool isTransitioning = false;

    void Start()
    {
        // Automatically fades back in from black when the new scene loads
        if (shouldFadeIn && fadeCanvasGroup != null)
        {
            shouldFadeIn = false;
            StartCoroutine(FadeIn());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;
            StartCoroutine(FadeOutAndLoad());
        }
    }

    IEnumerator FadeOutAndLoad()
    {
        savedFadeDuration = fadeDuration;

        // Optionally lock player movement while fading out
        PlayerController player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerController>();
        if (player != null) player.enabled = false;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            fadeCanvasGroup.alpha = 1f;
        }

        shouldFadeIn = true;
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < savedFadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / savedFadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}

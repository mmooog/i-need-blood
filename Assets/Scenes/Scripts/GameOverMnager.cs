using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI & Fade Settings")]
    public CanvasGroup gameOverCanvasGroup;
    public Button tryAgainButton;
    public float delayBeforeFade = 1.0f;
    public float fadeDuration = 1.2f;
    public float respawnFadeInDuration = 0.8f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gameOverSound;

    private static bool isRespawning = false;
    private bool isGameOver = false;

    void Start()
    {
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(RestartScene);
        }

        // If the scene just reloaded from clicking "Try Again?", fade in from black!
        if (isRespawning && gameOverCanvasGroup != null)
        {
            isRespawning = false;
            StartCoroutine(FadeInFromBlack());
        }
    }

    IEnumerator FadeInFromBlack()
    {
        // Hide the Game Over text and button while fading in so only pure black shows
        foreach (Transform child in gameOverCanvasGroup.transform)
        {
            child.gameObject.SetActive(false);
        }

        gameOverCanvasGroup.alpha = 1f;
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.blocksRaycasts = false;

        float elapsed = 0f;
        while (elapsed < respawnFadeInDuration)
        {
            elapsed += Time.deltaTime;
            gameOverCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / respawnFadeInDuration);
            yield return null;
        }

        gameOverCanvasGroup.alpha = 0f;

        // Re-enable the text and button children silently for the next Game Over
        foreach (Transform child in gameOverCanvasGroup.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (gameOverCanvasGroup != null)
            {
                gameOverCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            }
            yield return null;
        }

        if (gameOverCanvasGroup != null)
        {
            gameOverCanvasGroup.alpha = 1f;
            gameOverCanvasGroup.interactable = true;
            gameOverCanvasGroup.blocksRaycasts = true;
        }
    }

    public void RestartScene()
    {
        isRespawning = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gameOverSound;

    private bool isGameOver = false;

    void Start()
    {
        // Automatically connects your Try Again button click without needing the Inspector menu
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(RestartScene);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

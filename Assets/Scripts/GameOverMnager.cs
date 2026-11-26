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

    [Header("Audio & BGM Crossfade")]
    public AudioSource audioSource;
    public AudioClip gameOverSound;
    [Range(0f, 1f)]
    public float gameOverVolume = 0.8f;
    public AudioSource levelBgmSource; // Optional: auto-detects BGMManager if left empty

    private static bool isRespawning = false;
    private bool isGameOver = false;

    void Start()
    {
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(RestartScene);
        }

        if (levelBgmSource == null)
        {
#pragma warning disable CS0618
            BGMManager bgm = FindObjectOfType<BGMManager>();
#pragma warning restore CS0618
            if (bgm != null)
            {
                levelBgmSource = bgm.GetComponent<AudioSource>();
            }
        }

        if (isRespawning && gameOverCanvasGroup != null)
        {
            isRespawning = false;
            StartCoroutine(FadeInFromBlack());
        }
    }

    IEnumerator FadeInFromBlack()
    {
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
            audioSource.clip = gameOverSound;
            audioSource.volume = 0f;
            audioSource.Play();
        }

        float startBgmVolume = levelBgmSource != null ? levelBgmSource.volume : 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            if (gameOverCanvasGroup != null)
            {
                gameOverCanvasGroup.alpha = t;
            }

            if (levelBgmSource != null)
            {
                levelBgmSource.volume = Mathf.Lerp(startBgmVolume, 0f, t);
            }

            if (audioSource != null && gameOverSound != null)
            {
                audioSource.volume = Mathf.Lerp(0f, gameOverVolume, t);
            }

            yield return null;
        }

        if (levelBgmSource != null)
        {
            levelBgmSource.Stop();
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

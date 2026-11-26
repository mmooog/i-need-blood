using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SimpleMainMenu : MonoBehaviour
{
    [Header("Scene Transition")]
    public string targetSceneName = "prologue inside";
    public CanvasGroup blackFadePanel;
    public float blackFadeDuration = 1.0f;

    [Header("Buttons")]
    public Button playButton;
    public Button helpButton;
    public Button backButton;
    public Button exitButton;

    [Header("Help Panel Fade")]
    public CanvasGroup helpPanel;
    public float helpFadeDuration = 0.35f;

    [Header("Button Click Audio (Zero-Delay Trim)")]
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float clickVolume = 0.8f;
    [Tooltip("Skips silence at the start of your audio clip (in seconds, e.g., 0.05 or 0.1)")]
    public float clickStartOffset = 0.05f;

    private AudioSource audioSource;
    private bool isLoading = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.clip = clickSound;
    }

    void Start()
    {
        if (helpPanel != null)
        {
            helpPanel.alpha = 0f;
            helpPanel.interactable = false;
            helpPanel.blocksRaycasts = false;
        }

        if (playButton != null) playButton.onClick.AddListener(() => { PlayClick(); OnPlayPressed(); });
        if (helpButton != null) helpButton.onClick.AddListener(() => { PlayClick(); OpenHelp(); });
        if (backButton != null) backButton.onClick.AddListener(() => { PlayClick(); CloseHelp(); });
        if (exitButton != null) exitButton.onClick.AddListener(() => { PlayClick(); StartCoroutine(ExitAfterClick()); });

        if (blackFadePanel != null)
        {
            StartCoroutine(FadeBlack(1f, 0f, false));
        }
    }

    void PlayClick()
    {
        if (audioSource == null || clickSound == null) return;

        audioSource.Stop();
        audioSource.clip = clickSound;
        audioSource.volume = clickVolume;
        // Jumps straight past any silent gap at the start of the audio file!
        audioSource.time = Mathf.Clamp(clickStartOffset, 0f, clickSound.length - 0.01f);
        audioSource.Play();
    }

    public void OnPlayPressed()
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(FadeOutAndLoad());
    }

    IEnumerator FadeOutAndLoad()
    {
        if (blackFadePanel != null)
        {
            yield return StartCoroutine(FadeBlack(0f, 1f, true));
        }
        SceneManager.LoadScene(targetSceneName);
    }

    public void OpenHelp()
    {
        if (helpPanel == null) return;
        StopCoroutine(nameof(FadeHelp));
        StartCoroutine(FadeHelp(0f, 1f, true));
    }

    public void CloseHelp()
    {
        if (helpPanel == null) return;
        StopCoroutine(nameof(FadeHelp));
        StartCoroutine(FadeHelp(1f, 0f, false));
    }

    IEnumerator FadeHelp(float startAlpha, float endAlpha, bool opening)
    {
        if (!opening)
        {
            helpPanel.interactable = false;
            helpPanel.blocksRaycasts = false;
        }

        float elapsed = 0f;
        while (elapsed < helpFadeDuration)
        {
            elapsed += Time.deltaTime;
            helpPanel.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / helpFadeDuration);
            yield return null;
        }

        helpPanel.alpha = endAlpha;

        if (opening)
        {
            helpPanel.interactable = true;
            helpPanel.blocksRaycasts = true;
        }
    }

    IEnumerator FadeBlack(float startAlpha, float endAlpha, bool blockClicks)
    {
        blackFadePanel.alpha = startAlpha;
        blackFadePanel.blocksRaycasts = true;

        float elapsed = 0f;
        while (elapsed < blackFadeDuration)
        {
            elapsed += Time.deltaTime;
            blackFadePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / blackFadeDuration);
            yield return null;
        }

        blackFadePanel.alpha = endAlpha;
        blackFadePanel.blocksRaycasts = blockClicks;
    }

    IEnumerator ExitAfterClick()
    {
        // Waits 0.2s so the Exit click sound actually plays before closing the game!
        yield return new WaitForSeconds(0.2f);
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}


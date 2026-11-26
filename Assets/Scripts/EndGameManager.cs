using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class EndGameManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string levelSceneName = "level1";      // Exact name of the level you want "Restart" to load
    public string mainMenuSceneName = "MainMenu"; // Exact name of your Main Menu scene

    [Header("Buttons")]
    public Button restartButton;
    public Button mainMenuButton;
    public Button exitButton;

    [Header("Fade-Out on Button Press")]
    public CanvasGroup blackFadePanel; // Drag FadeInPanel here for a smooth fade-out
    public float fadeDuration = 0.8f;

    [Header("Button Click Audio (Zero-Delay Trim)")]
    public AudioClip clickSound;
    [Range(0f, 1f)]
    public float clickVolume = 0.8f;
    public float clickStartOffset = 0.05f;

    private AudioSource audioSource;
    private bool isTransitioning = false;

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
        if (restartButton != null)
            restartButton.onClick.AddListener(() => { PlayClick(); LoadWithFade(levelSceneName); });

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(() => { PlayClick(); LoadWithFade(mainMenuSceneName); });

        if (exitButton != null)
            exitButton.onClick.AddListener(() => { PlayClick(); StartCoroutine(ExitAfterClick()); });
    }

    void PlayClick()
    {
        if (audioSource == null || clickSound == null) return;

        audioSource.Stop();
        audioSource.clip = clickSound;
        audioSource.volume = clickVolume;
        audioSource.time = Mathf.Clamp(clickStartOffset, 0f, clickSound.length - 0.01f);
        audioSource.Play();
    }

    void LoadWithFade(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (blackFadePanel != null)
        {
            blackFadePanel.blocksRaycasts = true;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                blackFadePanel.alpha = Mathf.Clamp01(elapsed / fadeDuration);
                yield return null;
            }
            blackFadePanel.alpha = 1f;
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator ExitAfterClick()
    {
        yield return new WaitForSeconds(0.2f);
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    public string nextSceneName = "prologue outside";
    public float fadeDuration = 1f;

    private static Image fadeOverlay;
    private static SceneDoorRunner runner;
    private bool isTransitioning = false;

    private class SceneDoorRunner : MonoBehaviour { }

    void Awake()
    {
        if (fadeOverlay == null)
        {
            GameObject canvasObj = new GameObject("AutoFadeCanvas");
            DontDestroyOnLoad(canvasObj);
            runner = canvasObj.AddComponent<SceneDoorRunner>();

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;

            GameObject imgObj = new GameObject("FadeBlack");
            imgObj.transform.SetParent(canvasObj.transform, false);

            fadeOverlay = imgObj.AddComponent<Image>();
            fadeOverlay.color = new Color(0f, 0f, 0f, 0f);
            fadeOverlay.raycastTarget = false;

            RectTransform rect = fadeOverlay.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fadeOverlay != null && fadeOverlay.color.a > 0f && runner != null)
        {
            runner.StartCoroutine(FadeRoutine(1f, 0f, 1f));
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Door touched by: " + other.name + " | Tag: " + other.tag);

        if (!isTransitioning && other.CompareTag("Player"))
        {
            isTransitioning = true;
            runner.StartCoroutine(FadeOutAndLoad(nextSceneName, fadeDuration));
        }
    }

    private static IEnumerator FadeOutAndLoad(string sceneName, float duration)
    {
        yield return FadeRoutine(0f, 1f, duration);
        SceneManager.LoadScene(sceneName);
    }

    private static IEnumerator FadeRoutine(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color c = fadeOverlay.color;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            fadeOverlay.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadeOverlay.color = c;
    }
}

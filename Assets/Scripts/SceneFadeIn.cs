using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SceneFadeIn : MonoBehaviour
{
    public float fadeDuration = 1.0f;

    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}

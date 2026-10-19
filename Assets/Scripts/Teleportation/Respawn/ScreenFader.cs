using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    public Image fadeImage;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void FadeOut(System.Action onComplete = null)
    {
        StartCoroutine(Fade(0f, 1f, onComplete));
    }

    public IEnumerator FadeRoutine(float from, float to)
    {
        if (fadeImage == null)
            yield break;

        // Включаем GameObject перед затемнением
        fadeImage.gameObject.SetActive(true);

        float time = 0f;
        Color c = fadeImage.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(from, to, time / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        // Финальное значение
        fadeImage.color = new Color(c.r, c.g, c.b, to);

        // ❗ Только если fade-out полностью завершён — отключаем
        if (to == 0f)
            fadeImage.gameObject.SetActive(false);
    }





    public void FadeIn(System.Action onComplete = null)
    {
        StartCoroutine(Fade(1f, 0f, onComplete));
    }

    private IEnumerator Fade(float from, float to, System.Action onComplete)
    {
        if (fadeImage != null)
            fadeImage.gameObject.SetActive(true);

        float time = 0f;
        Color c = fadeImage.color;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(from, to, time / fadeDuration);
            fadeImage.color = new Color(c.r, c.g, c.b, alpha);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, to);

        if (to == 0f)
            fadeImage.gameObject.SetActive(false);

        onComplete?.Invoke();
    }
}

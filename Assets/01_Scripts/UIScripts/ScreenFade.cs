using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage; // 암전 효과를 위한 이미지
    public float fadeDuration = 1.0f; // 암전 및 회복 시간

    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 0);
        }
    }

    // 화면을 서서히 어둡게 한 후 서서히 밝게 하는 메서드
    public void FadeOutAndIn()
    {
        StartCoroutine(FadeOutAndInCoroutine());
    }

    // 화면을 서서히 어둡게 만든 후 서서히 밝게 만드는 코루틴
    IEnumerator FadeOutAndInCoroutine()
    {
        // Fade Out
        float elapsedTime = 0;
        Color color = fadeImage.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1;
        fadeImage.color = color;
        GameManager.instance.PlayerRecovery();
        // Fade In
        elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            color.a = 1 - Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0;
        fadeImage.color = color;
    }
}

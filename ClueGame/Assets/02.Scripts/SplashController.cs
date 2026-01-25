using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SplashController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float splashDuration = 5f;
    [SerializeField] private string gameSceneName = "MainScene";
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;

    [Header("UI References")]
    [SerializeField] private Image fadeImage;

    private void Start()
    {
        // 페이드 이미지가 없으면 페이드 없이 진행
        if (fadeImage != null)
        {
            StartCoroutine(SplashSequenceWithFade());
        }
        else
        {
            StartCoroutine(LoadGameSceneAfterDelay());
        }
    }

    private IEnumerator SplashSequenceWithFade()
    {
        // 1. 페이드 인 (검은 화면 → 밝아짐)
        yield return StartCoroutine(FadeIn());

        // 2. 스플래시 화면 표시
        float displayTime = splashDuration - fadeInDuration - fadeOutDuration;
        yield return new WaitForSeconds(displayTime);

        // 3. 페이드 아웃 (밝은 화면 → 검어짐)
        yield return StartCoroutine(FadeOut());

        // 4. 게임 씬 로드
        SceneManager.LoadScene(gameSceneName);
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeInDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 0f;
        fadeImage.color = color;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeOutDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
    }

    private IEnumerator LoadGameSceneAfterDelay()
    {
        yield return new WaitForSeconds(splashDuration);
        SceneManager.LoadScene(gameSceneName);
    }

    private void Update()
    {
        // 스페이스바나 마우스 클릭으로 스킵
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            SceneManager.LoadScene(gameSceneName);
        }
    }
}
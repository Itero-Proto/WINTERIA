using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class PageTurnSceneLoader : MonoBehaviour
{
    [Header("UI")]
    public Image pageOverlay;
    public CanvasGroup menuCanvasGroup;
    [Header("Canvas Zoom")]
    public float zoomDuration = 2f;
    public float canvasZoomScale = 1.2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip startChapterSound;

    [Header("Timing")]
    public float menuFadeDuration = 1f;
    public float screenFadeDuration = 2f;

    private bool isLoading = false;
    public void StartPageTurn()
    {
        if (isLoading) return;
        isLoading = true;

        if (audioSource != null && startChapterSound != null)
            audioSource.PlayOneShot(startChapterSound);

        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
            StartCoroutine(FadeMenuOut());
        }

        StartCoroutine(FadeScreenAndLoad());
    }
    private IEnumerator FadeMenuOut()
    {
        float t = 0f;
        float startAlpha = menuCanvasGroup.alpha;

        while (t < menuFadeDuration)
        {
            t += Time.deltaTime;
            menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / menuFadeDuration);
            yield return null;
        }

        menuCanvasGroup.alpha = 0f;
    }

    private IEnumerator FadeScreenAndLoad()
    {
        float t = 0f;
        Color c = pageOverlay.color;
        c.a = 0f;
        pageOverlay.color = c;

        while (t < screenFadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / screenFadeDuration);
            pageOverlay.color = c;
            yield return null;
        }
        SceneManager.LoadScene("GameScene_1");
    }
}

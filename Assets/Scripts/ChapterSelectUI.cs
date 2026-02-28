using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChapterSelectUI : MonoBehaviour
{
    [Header("Panel")]
    public CanvasGroup chapterPanel;
    [Header("Chapters")]
    public Button chapter1Button;
    [Header("Fade")]
    public Image fadeOverlay;
    public float menuFadeDuration = 1f;
    public RectTransform menuRoot;
    [Header("Book")]
    public Animator bookAnimator;
    [Header("Transition")]
    public float transitionDuration = 1.5f;
    public float targetFadeAlpha = 1f;
    public float canvasZoomScale = 1.2f;

    public CanvasGroup menuCanvasGroup;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip pageTurnSound;
    public AudioClip lockedSound;
    public AudioClip startChapterSound;
    private bool isOpen = false;

    private void Start()
    {
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
        }
    }
    public void OpenChapterPanel()
    {
        if (isOpen) return;
        isOpen = true;
        bookAnimator.SetTrigger("Turn");
        audioSource.PlayOneShot(pageTurnSound);

        StartCoroutine(OpenSequence());
    }

    private IEnumerator OpenSequence()
    {
        yield return StartCoroutine(FadeIn());

        chapterPanel.alpha = 1f;
        chapterPanel.interactable = true;
        chapterPanel.blocksRaycasts = true;
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = fadeOverlay.color;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 0.6f, t / transitionDuration); // 0.6 — мягкое затемнение
            fadeOverlay.color = c;
            yield return null;
        }
    }

    private IEnumerator ZoomAndFade()
    {
        float t = 0f;

        Vector3 startScale = menuRoot.localScale;
        Vector3 targetScale = Vector3.one * canvasZoomScale;

        Color fadeColor = fadeOverlay.color;
        float startAlpha = fadeColor.a;

        while (t < transitionDuration)
        {
            t += Time.deltaTime;
            float k = t / transitionDuration;

            // ЗУМ
            menuRoot.localScale = Vector3.Lerp(startScale, targetScale, k);

            // ЗАТЕМНЕНИЕ
            fadeColor.a = Mathf.Lerp(startAlpha, targetFadeAlpha, k);
            fadeOverlay.color = fadeColor;

            yield return null;
        }
    }

    public void OnChapterClicked()
    {
        StartCoroutine(StartChapterRoutine("GameScene_1"));
    }

    private IEnumerator StartChapterRoutine(string sceneName)
    {
        chapterPanel.interactable = false;
        chapterPanel.blocksRaycasts = false;

        if (audioSource && startChapterSound)
            audioSource.PlayOneShot(startChapterSound);

        yield return StartCoroutine(ZoomAndFade());

        SceneManager.LoadScene("1_Scene");
    }

}

using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FinalSceneSound : MonoBehaviour
{
    [Header("Звук мальчика")]
    public AudioSource audioSource;
    public AudioClip boySound;

    [Header("UI")]
    public TMP_Text messageText; // Текст внизу экрана

    private bool soundPlayed = false;
    private bool canExit = false;
    private Coroutine blinkRoutine;

    private void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
            var color = messageText.color;
            color.a = 0f;
            messageText.color = color;
        }

        StartCoroutine(EnableExitAfterDelay());
    }

    public void PlayBoySound()
    {
        if (!soundPlayed && audioSource != null && boySound != null)
        {
            audioSource.PlayOneShot(boySound);
            soundPlayed = true;
        }
    }

    private IEnumerator EnableExitAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        canExit = true;

        if (messageText != null)
        {
            messageText.text = "НАЖМИТЕ ЛЮБУЮ КНОПКУ ДЛЯ ВЫХОДА";
            messageText.gameObject.SetActive(true);
            blinkRoutine = StartCoroutine(BlinkText());
        }
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            // плавное появление
            for (float t = 0; t < 1f; t += Time.deltaTime)
            {
                SetTextAlpha(t);
                yield return null;
            }

            // плавное исчезновение
            for (float t = 1f; t > 0f; t -= Time.deltaTime)
            {
                SetTextAlpha(t);
                yield return null;
            }
        }
    }

    private void SetTextAlpha(float alpha)
    {
        if (messageText == null) return;
        Color color = messageText.color;
        color.a = alpha;
        messageText.color = color;
    }

    private void Update()
    {
        if (canExit && Input.anyKeyDown)
            StartCoroutine(ExitToMainMenu());
    }

    private IEnumerator ExitToMainMenu()
    {
        canExit = false;
        if (blinkRoutine != null) StopCoroutine(blinkRoutine);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainMenuScene");
    }
}

using UnityEngine;
using System.Collections;

public class SnowDestructible : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    [Header("Danger Blink")]
    public bool enableBlink = true;
    public float blinkSpeed = 3f;
    private Material objectMaterial;
    public AudioClip shakeSound;
    [Range(0f, 1f)]
    private AudioSource audioSource;
    [Header("Idle Shake")]
    public bool enableIdleShake = true;
    public float minShakeInterval = 2f;
    public float maxShakeInterval = 5f;
    [Header("Effects")]
    public GameObject destroyEffect;
    public AudioClip destroySound;
    [Header("Reflection")]
    public GameObject reflectionPrefab;
    [Header("Visual Feedback")]
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.1f;
    public Color damagedColor = new Color(0.8f, 0.8f, 0.8f);

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Renderer objectRenderer;
    private Color originalColor;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        currentHealth = maxHealth;
        originalScale = transform.localScale;
        originalPosition = transform.position;
        if (enableIdleShake)
        {
            StartCoroutine(IdleShakeRoutine());
        }
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            objectMaterial = objectRenderer.material;
            originalColor = objectMaterial.color;
        }
    }
    private void Update()
    {
        if (!enableBlink || objectMaterial == null) return;

        float pulse = (Mathf.Sin(Time.time * blinkSpeed) + 1f) / 2f;

        Color dangerColor = Color.red;
        objectMaterial.color = Color.Lerp(originalColor, dangerColor, pulse * 0.5f);
    }
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        // Меняем цвет
        if (objectRenderer != null)
        {
            float damagePercent = 1f - (float)currentHealth / maxHealth;
            objectRenderer.material.color = Color.Lerp(originalColor, damagedColor, damagePercent);
        }

        // Тряска
        StartCoroutine(Shake());

        if (currentHealth <= 0)
        {
            DestroyObject();
        }
    }
    private IEnumerator IdleShakeRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minShakeInterval, maxShakeInterval);
            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(Shake());
        }
    }
    private IEnumerator Shake()
    {
        if (shakeSound != null)
        {
            audioSource.PlayOneShot(shakeSound, 0.5f);
        }
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeStrength;
            transform.position = startPos + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
    }
    private void DestroyObject()
    {
        if (destroySound != null)
        {
            GameObject tempAudio = new GameObject("TempDestroySound");
            tempAudio.transform.position = transform.position;

            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = destroySound;
            tempSource.spatialBlend = 0f;
            tempSource.Play();
            tempSource.volume = 0.3f;
            Destroy(tempAudio, destroySound.length);
        }
        // 👉 Создаём отражение
        if (reflectionPrefab != null)
        {
            Instantiate(reflectionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
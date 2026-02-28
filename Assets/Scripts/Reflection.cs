using UnityEngine;
using System.Collections;
public class Reflection : MonoBehaviour
{
    public Transform player;
    public GameObject snowballPrefab;
    public Transform throwPoint;
    public float snowballSpeed = 15f;
    private Animator animator;
    public float flightTime = 0.7f; // время полета снежка до игрока
    [Header("Ambient Sound")]
    public AudioClip reflectionSound;
    public float minSoundInterval = 6f;
    public float maxSoundInterval = 12f;
    [Header("Sound VFX")]
    public GameObject reflectionVFX;
    public float vfxHeightOffset = 2f;
    private AudioSource audioSource;
    [Header("Death")]
    public AudioClip deathSound;
    public GameObject destroyEffectPrefab;
    public AudioClip deceptionSound;
    public GameObject deceptionVFX;
    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerClickMovement3D>().transform;
            audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(reflectionSound, 1f);
            Vector3 spawnPos = transform.position + Vector3.up * vfxHeightOffset;
            GameObject vfx = Instantiate(reflectionVFX, spawnPos, Quaternion.identity);
            Destroy(vfx, 2f);
            audioSource.spatialBlend = 0f;
            StartCoroutine(RandomSoundRoutine());
            animator = GetComponent<Animator>();
    }
    private IEnumerator RandomSoundRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSoundInterval, maxSoundInterval);
            yield return new WaitForSeconds(waitTime);

            if (reflectionSound != null)
            {
                audioSource.PlayOneShot(reflectionSound, 1f);

                if (reflectionVFX != null)
                {
                    Vector3 spawnPos = transform.position + Vector3.up * vfxHeightOffset;

                    GameObject vfx = Instantiate(reflectionVFX, spawnPos, Quaternion.identity);

                    Destroy(vfx, 3f); // удалить через 3 секунды (или длительность эффекта)
                }
            }
        }
    }
    private void OnEnable()
    {
        PlayerClickMovement3D.OnPlayerThrow += ReactToThrow;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            audioSource.PlayOneShot(deceptionSound);
            Vector3 spawnPos = transform.position + Vector3.up * vfxHeightOffset;
            GameObject vfx = Instantiate(deceptionVFX, spawnPos, Quaternion.identity);

            Destroy(vfx, 3f); // удалить через 3 секунды (или длительность эффекта)
        }
    }
        
    public void Die()
    {
        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Отключаем Collider и MeshRenderer у всех детей
        Collider childCollider = GetComponentInChildren<Collider>();
        if (childCollider != null)
            childCollider.enabled = false;

        MeshRenderer childRenderer = GetComponentInChildren<MeshRenderer>();
        if (childRenderer != null)
            childRenderer.enabled = false;

        // Уничтожаем объект через полсекунды
        Destroy(gameObject, 0.5f);

        // Визуальный эффект
        if (destroyEffectPrefab != null)
        {
            Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity);
        }
    }
    private void OnDisable()
    {
        PlayerClickMovement3D.OnPlayerThrow -= ReactToThrow;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    void ReactToThrow()
    {
        if (animator != null)
            animator.SetTrigger("Throw");
        // снежок появится через Animation Event
    }
    public void SpawnSnowball()
    {
        if (player == null || snowballPrefab == null || throwPoint == null) return;

        GameObject snowball = Instantiate(
            snowballPrefab,
            throwPoint.position,
            Quaternion.identity
        );

        Rigidbody snowballRb = snowball.GetComponent<Rigidbody>();

        // Направление строго в игрока
        Vector3 direction = (player.position - throwPoint.position).normalized;

        // Задаём прямую скорость
        snowballRb.linearVelocity = direction * snowballSpeed;
    }
}
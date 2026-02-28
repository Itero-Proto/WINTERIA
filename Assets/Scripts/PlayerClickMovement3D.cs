using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerClickMovement3D : MonoBehaviour
{
    public static System.Action OnPlayerThrow;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public LayerMask groundLayer;
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool isThrowing = false;
    public float bounceForce = 12f;
    public AudioClip bounceSound;
    [Header("Footsteps")]
    public AudioClip footstepSound;

    [Header("Snowball")]
    public GameObject snowballPrefab;
    public Transform throwPoint;
    public float throwForce = 12f;
    public float throwSpeed = 15f;
    public AudioClip throwSound;
    private Vector3 throwTarget;

    [Header("Death")]
    public bool isDead = false;
    public AudioClip deathSound;
    public Image fadeImage; // Чёрное изображение на UI
    public float fadeDuration = 1f;
    [Header("Death Visual")]
    public Material deathMaterial;

    private Renderer playerRenderer;
    [Header("Entrance")]
    public AudioClip entranceSound;
    public float entranceFadeDuration = 2f;
    private bool isEntering = false;
    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;

    void Awake()
    {
        playerRenderer = GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        targetPosition = transform.position;

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0); // прозрачное в начале
    }

    void Update()
    {
        if (isThrowing || isDead) return;

        // ЛКМ — движение
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                targetPosition = hit.point;
                isMoving = true;
            }
        }

        // ПКМ — бросок
        if (Input.GetMouseButtonDown(1))
        {
            OnPlayerThrow?.Invoke();
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                throwTarget = hit.point;
                isMoving = false;
                animator.SetBool("isWalking", false);

                Vector3 lookDir = throwTarget - transform.position;
                lookDir.y = 0f;
                if (lookDir != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(lookDir);

                isThrowing = true;
                animator.SetTrigger("Throw");
            }
        }
    }

    void FixedUpdate()
    {
        if (!isMoving || isThrowing || isDead)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.fixedDeltaTime);
        }

        if (Vector3.Distance(rb.position, targetPosition) < 0.05f)
        {
            rb.MovePosition(targetPosition);
            isMoving = false;
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("SnowDestructible")
            || collision.gameObject.CompareTag("Reflection")
            || collision.gameObject.CompareTag("REFLECTION_ONLY"))
        {
            Die();
        }

        if (collision.gameObject.CompareTag("ToThe3Level"))
        {
            Bounce();
        }
    }
    private void Bounce()
    {
        // Обнуляем текущую вертикальную скорость
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        // Проиграть звук
        if (entranceSound != null)
            audioSource.PlayOneShot(bounceSound);
        // Добавляем импульс вверх
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Entrance") && !isEntering && !isDead)
        {
            StartCoroutine(EnterSequence());
        }
        if (other.CompareTag("DestroyZone"))
        {
            Die();
        }
    }
    private IEnumerator EnterSequence()
    {
        isEntering = true;

        // Остановить игрока
        isMoving = false;
        isThrowing = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        animator.SetBool("isWalking", false);

        // Проиграть звук
        if (entranceSound != null)
            audioSource.PlayOneShot(entranceSound);

        // Плавное затемнение 2 секунды
        float t = 0f;
        while (t < entranceFadeDuration)
        {
            t += Time.deltaTime;

            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0f, 1f, t / entranceFadeDuration);
                fadeImage.color = c;
            }

            yield return null;
        }

        // Загрузка следующей сцены
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayFootstep()
    {
        if (footstepSound != null)
            audioSource.PlayOneShot(footstepSound, 0.25f);
    }

    public void SpawnSnowball()
    {
        if (throwSound != null)
            audioSource.PlayOneShot(throwSound, 0.4f);

        GameObject snowball = Instantiate(snowballPrefab, throwPoint.position, Quaternion.identity);
        Rigidbody snowballRb = snowball.GetComponent<Rigidbody>();
        Vector3 dir = (throwTarget - throwPoint.position).normalized;
        snowballRb.linearVelocity = dir * throwSpeed;
    }

    public void EndThrow()
    {
        isThrowing = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        isMoving = false;
        isThrowing = false;
        
        animator.SetTrigger("Die");

        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);
        if (playerRenderer != null && deathMaterial != null)
        {
            playerRenderer.material = deathMaterial;
        }
    }

    // Этот метод вызывать через Animation Event в конце анимации смерти
    public void StartFadeAndRestart()
    {
        StartCoroutine(FadeAndRestart());
    }

    private IEnumerator FadeAndRestart()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
                fadeImage.color = c;
            }
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
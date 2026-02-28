using UnityEngine;
using System.Collections;

public class Snowball : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public float lifeTime = 5f;

    public AudioClip hitSound;
    public float hitPauseDuration = 0.1f;
    public float hitTimeScale = 0.2f;

    private AudioSource audioSource;
    private bool hasHit = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, lifeTime);
    }
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        hasHit = true;
        SnowDestructible destructible = other.gameObject.GetComponent<SnowDestructible>();

        if (destructible != null && !other.CompareTag("REFLECTION_ONLY"))
        {
            destructible.TakeDamage(1);
        }

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, 0.5f);
        }

        StartCoroutine(HitPause());

        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        Destroy(gameObject, 0.25f);
    }
    IEnumerator HitPause()
    {
        Time.timeScale = hitTimeScale;
        yield return new WaitForSecondsRealtime(hitPauseDuration);
        Time.timeScale = 1f;
    }
}
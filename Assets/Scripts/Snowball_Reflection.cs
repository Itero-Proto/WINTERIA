using UnityEngine;
using System.Collections;

public class Snowball_Reflection : MonoBehaviour
{
    public GameObject hitEffectPrefab;
    public float lifeTime = 5f;
    public AudioClip hitSound;
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
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerClickMovement3D player = other.gameObject.GetComponent<PlayerClickMovement3D>();
            if (player != null)
            {
                player.Die();
            }
        }
        SnowDestructible destructible = other.gameObject.GetComponent<SnowDestructible>();
        if (other.gameObject.CompareTag("SnowDestructible"))
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 0.9f);
        }
        if (other.gameObject.CompareTag("REFLECTION_ONLY"))
            {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 0.9f);
            Destroy(other.gameObject);
        }
        Reflection reflection = other.gameObject.GetComponent<Reflection>();
        if (other.gameObject.CompareTag("Reflection"))
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 0.9f);
            reflection.Die();
        }
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position, 0.5f);
        }
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
}
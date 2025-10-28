using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbientArea : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeSpeed = 1f; // ความเร็วในการ fade in/out

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float baseVolume = 1f; // ระดับเสียงพื้นฐานของโซนนี้

    private AudioSource audioSource;
    private bool playerInside = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f; // เริ่มเงียบ
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        float targetVolume = playerInside ? baseVolume : 0f;
        audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);

        if (playerInside && !audioSource.isPlaying)
            audioSource.Play();

        if (!playerInside && audioSource.volume <= 0.01f && audioSource.isPlaying)
            audioSource.Stop();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }
}

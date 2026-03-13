using UnityEngine;

public class PlayAudioAfterADelay : MonoBehaviour
{
    [Header("Timing")]
    public float delay = 1.5f;

    [Header("Audio")]
    public AudioSource audioSource;

    private void Start()
    {
        StartCoroutine(PlayAfterDelay());
    }

    private System.Collections.IEnumerator PlayAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }
}

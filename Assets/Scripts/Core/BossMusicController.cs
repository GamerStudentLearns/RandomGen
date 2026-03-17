using UnityEngine;

public class BossMusicController : MonoBehaviour
{
    [Header("Boss Music Pool")]
    public AudioClip[] bossSongs;

    [Header("Audio Source (same one used by RandomMusicPlayer)")]
    public AudioSource audioSource;

    private AudioClip savedNormalTrack;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayRandomBossTrack()
    {
        if (bossSongs == null || bossSongs.Length == 0)
        {
            Debug.LogWarning("No boss songs assigned!");
            return;
        }

        // Save whatever normal track was playing
        savedNormalTrack = audioSource.clip;

        // Pick random boss track
        int index = Random.Range(0, bossSongs.Length);
        audioSource.clip = bossSongs[index];
        audioSource.loop = true;
        audioSource.Play();
    }

    public void RestoreNormalMusic()
    {
        if (savedNormalTrack == null)
        {
            Debug.LogWarning("No saved normal track to restore!");
            return;
        }

        audioSource.clip = savedNormalTrack;
        audioSource.loop = true;
        audioSource.Play();
    }
}

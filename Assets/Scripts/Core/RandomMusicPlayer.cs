using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    [Header("Assign your audio clips here")]
    public AudioClip[] songs;

    [Header("Audio Source used for playback")]
    public AudioSource audioSource;

    void Start()
    {
        if (songs.Length == 0)
        {
            Debug.LogWarning("No songs assigned to RandomMusicPlayer.");
            return;
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        PlayRandomSong();
    }

    void PlayRandomSong()
    {
        int index = Random.Range(0, songs.Length);
        audioSource.clip = songs[index];
        audioSource.loop = true;
        audioSource.Play();
    }
}

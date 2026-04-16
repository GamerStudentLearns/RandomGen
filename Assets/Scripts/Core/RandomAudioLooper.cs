using UnityEngine;

public class RandomAudioLooper : MonoBehaviour
{
    [Header("Assign your audio source + audio clips")]
    public AudioSource source;
    public AudioClip[] clips;

    private int lastIndex = -1;

    void Start()
    {
        if (source == null)
        {
            Debug.LogError("RandomAudioLooper: No AudioSource assigned!");
            return;
        }

        PlayRandomClip();
    }

    void Update()
    {
        if (source != null && !source.isPlaying)
        {
            PlayRandomClip();
        }
    }

    void PlayRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return;

        int index = GetRandomIndex();

        source.clip = clips[index];
        source.Play();

        lastIndex = index;
    }

    int GetRandomIndex()
    {
        if (clips.Length == 1)
            return 0;

        int index;
        do
        {
            index = Random.Range(0, clips.Length);
        }
        while (index == lastIndex);

        return index;
    }
}

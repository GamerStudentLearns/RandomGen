using UnityEngine;

public class TimedAudioMuter : MonoBehaviour
{
    [System.Serializable]
    public class TimedAudio
    {
        public AudioSource source;     // The audio source to mute
        public float muteAfter = 3f;   // Time before it mutes
        [HideInInspector] public float timer;
        [HideInInspector] public bool muted;
    }

    [Header("Add as many audio sources as you want")]
    public TimedAudio[] audioSources;

    private void Update()
    {
        foreach (var audio in audioSources)
        {
            if (audio.muted || audio.source == null)
                continue;

            audio.timer += Time.deltaTime;

            if (audio.timer >= audio.muteAfter)
            {
                audio.source.mute = true;
                audio.muted = true;
            }
        }
    }
}

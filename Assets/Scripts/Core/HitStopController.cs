using UnityEngine;
using System.Collections;

public class HitStopController : MonoBehaviour
{
    public static HitStopController instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void Stop(float duration)
    {
        StartCoroutine(HitStop(duration));
    }

    IEnumerator HitStop(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
    }
}

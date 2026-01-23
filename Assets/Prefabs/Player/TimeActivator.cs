using UnityEngine;
using System.Collections;

public class TimeActivator : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The GameObject to activate temporarily.")]
    public GameObject targetObject;

    [Tooltip("How long (in seconds) to keep the object active.")]
    public float duration = 2.5f;

    [Header("Trigger Settings")]
    [Tooltip("Optional tag filter. Only objects with this tag will activate.")]
    public string triggerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Optional: only activate for objects with a specific tag
        if (string.IsNullOrEmpty(triggerTag) || other.CompareTag(triggerTag))
        {
            Activate();
        }
    }

    public void Activate()
    {
        if (targetObject != null)
        {
            StopAllCoroutines(); // restart if already active
            StartCoroutine(ActivateRoutine());
        }
        else
        {
            Debug.LogWarning("TimedActivator: No target object assigned!");
        }
    }

    private IEnumerator ActivateRoutine()
    {
        targetObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        targetObject.SetActive(false);
    }
}


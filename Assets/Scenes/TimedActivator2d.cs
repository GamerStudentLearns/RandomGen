using UnityEngine;
using System.Collections;


public class TimedActivator2D : MonoBehaviour
{
    [Header("Target to Activate")]
    [SerializeField] private GameObject targetObject;

    [Header("Activation Duration (seconds)")]
    [SerializeField] private float activeTime = 2f;

    [Header("Tag that can trigger activation")]
    [SerializeField] private string triggeringTag = "Player";

    private Coroutine activationRoutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(triggeringTag))
            Activate();
    }

    public void Activate()
    {
        if (activationRoutine != null)
            StopCoroutine(activationRoutine);

        activationRoutine = StartCoroutine(ActivateForTime());
    }

    private IEnumerator ActivateForTime()
    {
        targetObject.SetActive(true);
        yield return new WaitForSeconds(activeTime);
        targetObject.SetActive(false);

        activationRoutine = null;
    }
}

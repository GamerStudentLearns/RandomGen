using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public Transform playerDestination;
    public Transform cameraDestination;
    public float cameraSlideDuration = 0.4f;
    public float reenableDelay = 0.5f;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Teleport the player instantly
            collision.transform.position = playerDestination.position;

            // Disable trigger to prevent double‑firing
            triggerCollider.enabled = false;

            // Start smooth camera slide
            CoroutineRunner.Instance.StartCoroutine(SlideCamera());

            // Re-enable trigger after delay
            CoroutineRunner.Instance.StartCoroutine(ReenableTrigger());
        }
    }

    private System.Collections.IEnumerator SlideCamera()
    {
        Transform cam = Camera.main.transform;
        Vector3 startPos = cam.position;
        Vector3 endPos = new Vector3(
            cameraDestination.position.x,
            cameraDestination.position.y,
            startPos.z
        );

        float t = 0f;

        while (t < cameraSlideDuration)
        {
            t += Time.deltaTime;
            float lerp = t / cameraSlideDuration;

            // Smoothstep for nicer easing
            lerp = lerp * lerp * (3f - 2f * lerp);

            cam.position = Vector3.Lerp(startPos, endPos, lerp);
            yield return null;
        }

        cam.position = endPos;
    }

    private System.Collections.IEnumerator ReenableTrigger()
    {
        yield return new WaitForSeconds(reenableDelay);
        if (this != null)
            triggerCollider.enabled = true;
    }
}

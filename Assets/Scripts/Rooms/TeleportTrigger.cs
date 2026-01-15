using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public Transform playerDestination;
    public Transform cameraDestination;
    public float reenableDelay = 0.5f; // time before the trigger turns back on

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Teleport the player
            collision.transform.position = playerDestination.position;

            // Move the camera
            Camera.main.transform.position = new Vector3(
                cameraDestination.position.x,
                cameraDestination.position.y,
                Camera.main.transform.position.z
            );

            // Disable the trigger so it doesn't fire again immediately
            triggerCollider.enabled = false;

            // Re-enable after a short delay
            StartCoroutine(ReenableTrigger());
        }
    }

    private System.Collections.IEnumerator ReenableTrigger()
    {
        yield return new WaitForSeconds(reenableDelay);
        triggerCollider.enabled = true;
    }
}

using UnityEngine;

public class HeartPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float floatAmplitude = 0.25f;
    public float floatSpeed = 2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Floating effect
        transform.position =
            startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player == null)
            return;

        // ❌ Do nothing if player is already at full health
        if (player.currentHearts >= player.maxHearts)
            return;

        // ✅ Heal and consume pickup
        player.Heal(1);
        Destroy(gameObject);
    }
}

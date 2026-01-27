using UnityEngine;

public class SoulHeartPickup : MonoBehaviour
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

        player.AddSoulHearts(1);
        Destroy(gameObject);
    }
}

using UnityEngine;

public class RibRunner : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public Vector2 initialDirection = new Vector2(1, 1);

    private Vector2 moveDir;

    void Start()
    {
        moveDir = initialDirection.normalized;
    }

    void Update()
    {
        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;

        // Reflect direction like a DVD logo
        moveDir = Vector2.Reflect(moveDir, normal).normalized;
    }
}

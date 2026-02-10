using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject tearPrefab;
    public float tearSpeed = 10f;

    // fireRate = shots per second
    public float fireRate = 3f;

    private float fireCooldown;

    void Update()
    {
        fireCooldown -= Time.deltaTime;

        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow)) direction = Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;
        if (Input.GetKey(KeyCode.LeftArrow)) direction = Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) direction = Vector2.right;

        if (direction != Vector2.zero && fireCooldown <= 0f)
        {
            Shoot(direction);

            // Convert shots-per-second into delay
            fireCooldown = 1f / fireRate;
        }
    }

    void Shoot(Vector2 direction)
    {
        GameObject tear = Instantiate(tearPrefab, transform.position, Quaternion.identity);
        tear.GetComponent<Rigidbody2D>().linearVelocity = direction * tearSpeed;
    }
}

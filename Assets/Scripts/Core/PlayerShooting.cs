using UnityEngine;
public class PlayerShooting : MonoBehaviour
{
    public GameObject tearPrefab;
    public float tearSpeed = 10f;
    public float fireRate = 0.3f;
    private float fireCooldown;
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        Vector2 direction = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) direction = Vector2.up;
        if (Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;
        if (Input.GetKey(KeyCode.LeftArrow)) direction = Vector2.left;
        if (Input.GetKey(KeyCode.RightArrow)) direction = Vector2.right;
        if (direction != Vector2.zero && fireCooldown <= 0)
        {
            Shoot(direction);
            fireCooldown = fireRate;
        }
    }
    void Shoot(Vector2 direction)
    {
        GameObject tear = Instantiate(tearPrefab, transform.position, Quaternion.identity);
        tear.GetComponent<Rigidbody2D>().linearVelocity = direction * tearSpeed;
    }
}
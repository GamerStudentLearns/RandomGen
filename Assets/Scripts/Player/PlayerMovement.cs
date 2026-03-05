using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;

    private SpriteRenderer spriteRenderer;
    private TearSpawner tearSpawner;

    private PlayerStats stats;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tearSpawner = GetComponent<TearSpawner>();
        stats = GetComponent<PlayerStats>();   // NEW
    }

    void Update()
    {
        movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) movement.y += 1;
        if (Input.GetKey(KeyCode.S)) movement.y -= 1;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;

        if (movement != Vector2.zero && tearSpawner.CurrentShootDirection == Vector2.zero)
        {
            UpdateSprite(movement);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * stats.moveSpeed; // UPDATED
    }

    void UpdateSprite(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            spriteRenderer.sprite = dir.x > 0 ? rightSprite : leftSprite;
        else
            spriteRenderer.sprite = dir.y > 0 ? upSprite : downSprite;
    }
}

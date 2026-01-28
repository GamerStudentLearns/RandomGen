using UnityEngine;

public class TearSpawner : MonoBehaviour
{
    public GameObject tearPrefab;
    private PlayerStats playerStats;

    private SpriteRenderer spriteRenderer;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    // Expose current shoot direction so movement can check it
    public Vector2 CurrentShootDirection { get; private set; }

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CurrentShootDirection = Vector2.zero;
    }

    // MUST be public so other scripts can call it
    public void SpawnTear(Vector2 dir)
    {
        if (tearPrefab == null || playerStats == null)
            return;

        var tear = Instantiate(tearPrefab, transform.position, Quaternion.identity)
            .GetComponent<Tear>();

        if (tear == null)
            return;

        tear.damage = playerStats.damage;
        tear.speed = playerStats.shotSpeed;
        tear.range = playerStats.range;

        var rb = tear.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = dir.normalized * playerStats.shotSpeed;
    }

    // Keep sprite updating accessible externally
    public void UpdateSprite(Vector2 dir)
    {
        if (spriteRenderer == null) return;

        // remember last shoot direction
        CurrentShootDirection = dir;

        if (dir == Vector2.up) spriteRenderer.sprite = upSprite;
        else if (dir == Vector2.down) spriteRenderer.sprite = downSprite;
        else if (dir == Vector2.left) spriteRenderer.sprite = leftSprite;
        else if (dir == Vector2.right) spriteRenderer.sprite = rightSprite;
    }
}


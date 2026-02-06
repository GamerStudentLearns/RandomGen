using UnityEngine;

public class TearSpawner : MonoBehaviour
{
    public GameObject tearPrefab;
    private PlayerStats playerStats;
    private float cooldown;

    private SpriteRenderer spriteRenderer;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    // Expose shooting direction so movement can check it
    public Vector2 CurrentShootDirection { get; private set; }

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        CurrentShootDirection = GetShootDirection();

        if (CurrentShootDirection != Vector2.zero)
        {
            UpdateSprite(CurrentShootDirection);

            if (cooldown <= 0)
            {
                SpawnTear(CurrentShootDirection);
                cooldown = playerStats.fireRate;
            }
        }
    }

    void SpawnTear(Vector2 dir)
    {
        var tear = Instantiate(tearPrefab, transform.position, Quaternion.identity)
            .GetComponent<Tear>();

        tear.damage = playerStats.damage;
        tear.speed = playerStats.shotSpeed;
        tear.range = playerStats.range;

        tear.GetComponent<Rigidbody2D>().linearVelocity = dir * playerStats.shotSpeed;
    }

    void UpdateSprite(Vector2 dir)
    {
        if (dir == Vector2.up) spriteRenderer.sprite = upSprite;
        else if (dir == Vector2.down) spriteRenderer.sprite = downSprite;
        else if (dir == Vector2.left) spriteRenderer.sprite = leftSprite;
        else if (dir == Vector2.right) spriteRenderer.sprite = rightSprite;
    }

    Vector2 GetShootDirection()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(KeyCode.UpArrow)) direction = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow)) direction = Vector2.down;
        else if (Input.GetKey(KeyCode.LeftArrow)) direction = Vector2.left;
        else if (Input.GetKey(KeyCode.RightArrow)) direction = Vector2.right;

        return direction;
    }
}


using UnityEngine;

public class JesterHusk : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2.2f;
    public float changeDirInterval = 0.7f;

    [Header("Combat")]
    public float fireInterval = 1.1f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite walkUp, walkDown, walkLeft, walkRight;

    private Vector2 moveDir;
    private float moveTimer;
    private float fireTimer;

    void Start()
    {
        PickRandomDirection();
        moveTimer = changeDirInterval;
        fireTimer = fireInterval;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        UpdateSprite();
    }

    void HandleMovement()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            PickRandomDirection();
            moveTimer = changeDirInterval;
        }

        transform.position += (Vector3)(moveDir * moveSpeed * Time.deltaTime);
    }

    void PickRandomDirection()
    {
        moveDir = Random.insideUnitCircle.normalized;
    }

    void HandleShooting()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            if (projectilePrefab && firePoint)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                proj.GetComponent<Projectile>().SetDirection(dir);
            }
            fireTimer = fireInterval;
        }
    }

    void UpdateSprite()
    {
        if (!spriteRenderer) return;

        Vector2 dir = moveDir;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir = new Vector2(Mathf.Sign(dir.x), 0);
        else
            dir = new Vector2(0, Mathf.Sign(dir.y));

        if (dir.x != 0)
            spriteRenderer.sprite = dir.x > 0 ? walkRight : walkLeft;
        else
            spriteRenderer.sprite = dir.y > 0 ? walkUp : walkDown;
    }
}

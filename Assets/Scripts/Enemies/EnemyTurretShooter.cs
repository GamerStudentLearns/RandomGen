using UnityEngine;
public class EnemyTurretShooter : MonoBehaviour
{
    [Header("Combat")]
    public float detectionRange = 7f;
    public float fireRate = 1.5f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    [Header("Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite shootUp;
    public Sprite shootDown;
    public Sprite shootLeft;
    public Sprite shootRight;
    private Transform player;
    private float fireTimer;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Update()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= detectionRange)
        {
            AimAndShoot();
        }
    }
    void AimAndShoot()
    {
        fireTimer -= Time.deltaTime;
        Vector2 dir = (player.position - transform.position).normalized;
        UpdateSprite(dir);
        if (fireTimer <= 0f)
        {
            Shoot(dir);
            fireTimer = fireRate;
        }
    }
    void Shoot(Vector2 direction)
    {
        if (!projectilePrefab || !firePoint) return;
        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );
        proj.GetComponent<Projectile>().SetDirection(direction);
    }
    void UpdateSprite(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            spriteRenderer.sprite = dir.x > 0 ? shootRight : shootLeft;
        else
            spriteRenderer.sprite = dir.y > 0 ? shootUp : shootDown;
    }
}
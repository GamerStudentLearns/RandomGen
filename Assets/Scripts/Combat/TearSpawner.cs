using UnityEngine;
using UnityEngine.InputSystem;

public class TearSpawner : MonoBehaviour
{
    private PlayerControls controls;
    private PlayerStats stats;

    private Vector2 shootInput;
    private float cooldown;

    public GameObject tearPrefab;

    public SpriteRenderer shooterSR;

    public Sprite shootUpSprite;
    public Sprite shootDownSprite;
    public Sprite shootLeftSprite;
    public Sprite shootRightSprite;

    public Vector2 CurrentShootDirection => shootInput;

    public PlayerMovement playerMovement; // assign in inspector

    void Awake()
    {
        controls = new PlayerControls();
        stats = GetComponent<PlayerStats>();

        // Gamepad right stick
        controls.Player.Shoot.performed += ctx => shootInput = Snap(ctx.ReadValue<Vector2>());
        controls.Player.Shoot.canceled += ctx => shootInput = Vector2.zero;

        // Keyboard arrow keys
        controls.Player.ShootKeyboard.performed += ctx => shootInput = Snap(ctx.ReadValue<Vector2>());
        controls.Player.ShootKeyboard.canceled += ctx => shootInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        UpdateShooterSprite();

        cooldown -= Time.deltaTime;

        if (shootInput != Vector2.zero && cooldown <= 0)
        {
            SpawnTear(shootInput);
            cooldown = stats.fireRate;
        }
    }

    Vector2 Snap(Vector2 v)
    {
        if (v.magnitude < 0.5f) return Vector2.zero;

        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            return v.x > 0 ? Vector2.right : Vector2.left;
        else
            return v.y > 0 ? Vector2.up : Vector2.down;
    }

    void UpdateShooterSprite()
    {
        Vector2 dir = shootInput;

        // If not shooting, use player's facing direction
        if (dir == Vector2.zero)
            dir = playerMovement.LastFacingDirection;

        // Choose sprite
        if (dir.x > 0)
            shooterSR.sprite = shootRightSprite;
        else if (dir.x < 0)
            shooterSR.sprite = shootLeftSprite;
        else if (dir.y > 0)
            shooterSR.sprite = shootUpSprite;
        else
            shooterSR.sprite = shootDownSprite;
    }

    void SpawnTear(Vector2 dir)
    {
        var tear = Instantiate(tearPrefab, transform.position, Quaternion.identity)
            .GetComponent<Tear>();

        tear.damage = stats.damage;
        tear.speed = stats.shotSpeed;
        tear.range = stats.range;

        tear.GetComponent<Rigidbody2D>().linearVelocity = dir * stats.shotSpeed;
    }
}

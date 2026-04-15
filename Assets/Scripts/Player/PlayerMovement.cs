using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody2D rb;
    private PlayerStats stats;

    private Vector2 moveInput;
    private Vector2 lastDir = Vector2.down;

    public TearSpawner tearSpawner; // assign in inspector
    public SpriteRenderer sr;       // assign in inspector
    public Vector2 LastFacingDirection => lastDir;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();

        // Gamepad left stick
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Keyboard WASD
        controls.Player.MoveKeyboard.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.MoveKeyboard.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        UpdateSpriteFacing();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * stats.moveSpeed;
    }

    void UpdateSpriteFacing()
    {
        Vector2 shootDir = tearSpawner.CurrentShootDirection;

        // SHOOTING OVERRIDES MOVEMENT
        if (shootDir != Vector2.zero)
        {
            lastDir = shootDir;
        }
        else if (moveInput != Vector2.zero)
        {
            lastDir = moveInput;
        }

        // Choose sprite
        if (Mathf.Abs(lastDir.x) > Mathf.Abs(lastDir.y))
        {
            // Horizontal
            if (lastDir.x > 0)
                sr.sprite = rightSprite;
            else
                sr.sprite = leftSprite;
        }
        else
        {
            // Vertical
            if (lastDir.y > 0)
                sr.sprite = upSprite;
            else
                sr.sprite = downSprite;
        }
    }
}

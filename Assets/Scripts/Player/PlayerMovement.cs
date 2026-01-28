using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    private SpriteRenderer spriteRenderer;
    private TearSpawner tearSpawner;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        tearSpawner = GetComponent<TearSpawner>();
    }

    void Update()
    {
        movement = Vector2.zero;

        // Keyboard input (WASD)
        if (Input.GetKey(KeyCode.W)) movement.y += 1;
        if (Input.GetKey(KeyCode.S)) movement.y -= 1;
        if (Input.GetKey(KeyCode.A)) movement.x -= 1;
        if (Input.GetKey(KeyCode.D)) movement.x += 1;

        // Legacy joystick / controller axes (works with Unity's Input Manager)
        float legacyH = Input.GetAxisRaw("Horizontal");
        float legacyV = Input.GetAxisRaw("Vertical");
        movement.x += legacyH;
        movement.y += legacyV;

        // New Input System: prefer explicit gamepad left stick if available
        var gp = Gamepad.current;
        if (gp != null)
        {
            Vector2 leftStick = gp.leftStick.ReadValue();
            // If stick has input, override the current movement with it (prevents additive accumulation)
            if (leftStick.sqrMagnitude > 0.001f)
                movement = leftStick;
        }

        // Only update sprite if NOT shooting
        if (movement != Vector2.zero && tearSpawner != null && tearSpawner.CurrentShootDirection == Vector2.zero)
        {
            UpdateSprite(movement);
        }
    }

    void FixedUpdate()
    {
        // normalize movement to avoid faster diagonal movement
        Vector2 vel = movement;
        if (vel.sqrMagnitude > 1f) vel = vel.normalized;
        rb.linearVelocity = vel * moveSpeed;
    }

    void UpdateSprite(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            spriteRenderer.sprite = dir.x > 0 ? rightSprite : leftSprite;
        else
            spriteRenderer.sprite = dir.y > 0 ? upSprite : downSprite;
    }
}

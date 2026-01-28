using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    [Tooltip("Reference to the TearSpawner component (on same GameObject by default).")]
    public TearSpawner tearSpawner;

    private PlayerStats playerStats;
    private float cooldown;

    void Awake()
    {
        if (tearSpawner == null)
            tearSpawner = GetComponent<TearSpawner>();

        playerStats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        cooldown -= Time.unscaledDeltaTime; // unscaled so pause/timeScale won't freeze firing timer

        Vector2 shootDir = GetShootDirection();
        if (shootDir != Vector2.zero)
        {
            Vector2 cardinal;
            if (Mathf.Abs(shootDir.x) > Mathf.Abs(shootDir.y))
                cardinal = shootDir.x > 0 ? Vector2.right : Vector2.left;
            else
                cardinal = shootDir.y > 0 ? Vector2.up : Vector2.down;

            if (tearSpawner != null)
                tearSpawner.UpdateSprite(cardinal);

            if (cooldown <= 0f)
            {
                if (tearSpawner != null && playerStats != null)
                    tearSpawner.SpawnTear(cardinal);

                cooldown = playerStats != null ? playerStats.fireRate : 0.25f;
            }
        }
    }

    private Vector2 GetShootDirection()
    {
        var gp = Gamepad.current;
        if (gp != null)
        {
            Vector2 right = gp.rightStick.ReadValue();
            if (right.sqrMagnitude > 0.04f) return right;
        }

        Vector2 kb = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow)) kb = Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow)) kb = Vector2.down;
        else if (Input.GetKey(KeyCode.LeftArrow)) kb = Vector2.left;
        else if (Input.GetKey(KeyCode.RightArrow)) kb = Vector2.right;

        if (kb != Vector2.zero) return kb;

        float h = Input.GetAxisRaw("Horizontal2");
        float v = Input.GetAxisRaw("Vertical2");
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f) return new Vector2(h, v);

        return Vector2.zero;
    }
}
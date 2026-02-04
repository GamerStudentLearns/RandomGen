using UnityEngine;

public class GravityWellBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public float pullStrength = 3f;
    public float pullDuration = 1.2f;
    public float cooldown = 3f;

    private float timer;
    private bool pulling;

    public GameObject projectilePrefab;
    public int burstCount = 16;
    public float projectileSpeed = 7f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        timer = cooldown;
    }

    private void Update()
    {
        if (!isAwake) return;
        if (player == null) return;

        timer -= Time.deltaTime;

        if (pulling)
        {
            PullPlayer();
            if (timer <= 0)
            {
                pulling = false;
                ShootBurst();
                timer = cooldown;
            }
        }
        else
        {
            if (timer <= 0)
            {
                pulling = true;
                timer = pullDuration;
            }
        }
    }

    private void PullPlayer()
    {
        Vector2 dir = (transform.position - player.position).normalized;
        player.position += (Vector3)dir * pullStrength * Time.deltaTime;
    }

    private void ShootBurst()
    {
        float step = 360f / burstCount;

        for (int i = 0; i < burstCount; i++)
        {
            float angle = i * step;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
            proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * projectileSpeed;
        }
    }

    public void WakeUp()
    {
        isAwake = true;
        timer = cooldown;
    }
}

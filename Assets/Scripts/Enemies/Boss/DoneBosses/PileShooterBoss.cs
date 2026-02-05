using UnityEngine;

public class PileShooterBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public float actionCooldown = 2f;
    private float actionTimer;

    public GameObject projectilePrefab;
    public GameObject minionPrefab;

    private bool spawnNext = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        actionTimer = actionCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0)
        {
            if (spawnNext)
                SpawnMinion();
            else
                FireVolley();

            spawnNext = !spawnNext;
            actionTimer = actionCooldown;
        }
    }

    private void FireVolley()
    {
        // Direction from boss to player
        Vector2 toPlayer = (player.position - transform.position).normalized;

        // Base angle pointing toward the player
        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;

        for (int i = -2; i <= 2; i++)
        {
            float angle = baseAngle + (i * 10f); // spread around the player direction
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);

            // Use the rotated direction instead of Vector2.right
            proj.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 5f;
        }
    }


    private void SpawnMinion()
    {
        Vector2 offset = Random.insideUnitCircle * 2f;
        Instantiate(minionPrefab, transform.position + (Vector3)offset, Quaternion.identity);
    }

    public void WakeUp() => isAwake = true;
}

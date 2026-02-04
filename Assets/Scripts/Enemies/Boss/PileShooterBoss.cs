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
        for (int i = -2; i <= 2; i++)
        {
            float angle = i * 10f;
            Quaternion rot = Quaternion.Euler(0, 0, angle);
            GameObject proj = Instantiate(projectilePrefab, transform.position, rot);
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

using UnityEngine;

public class SummonerBoss : MonoBehaviour, IBoss
{
    public float moveSpeed = 1f;
    public Transform player;

    public float summonCooldown = 3f;
    private float summonTimer;

    public GameObject minionPrefab;
    public bool isAwake = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        summonTimer = summonCooldown;
    }

    private void Update()
    {
        if (!isAwake) return;
        if (player == null) return;

        MoveTowardPlayer();
        HandleSummoning();
    }

    private void MoveTowardPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void HandleSummoning()
    {
        summonTimer -= Time.deltaTime;

        if (summonTimer <= 0)
        {
            Instantiate(minionPrefab, transform.position, Quaternion.identity);
            summonTimer = summonCooldown;
        }
    }

    public void WakeUp()
    {
        isAwake = true;
        summonTimer = summonCooldown;
    }
}

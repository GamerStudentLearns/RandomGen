using UnityEngine;

public class TunnelWormBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public float burrowTime = 1.5f;
    public float travelSpeed = 6f;
    public float emergeCooldown = 2f;

    public GameObject tearPrefab;

    private float timer;
    private bool burrowed = false;
    private Vector3 travelDir;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        timer = emergeCooldown;
    }

    private void Update()
    {
        if (!isAwake || player == null) return;

        timer -= Time.deltaTime;

        if (burrowed)
        {
            transform.position += travelDir * travelSpeed * Time.deltaTime;

            if (timer <= 0)
                Emerge();
        }
        else
        {
            if (timer <= 0)
                Burrow();
        }
    }

    private void Burrow()
    {
        burrowed = true;
        timer = burrowTime;
        travelDir = (player.position - transform.position).normalized;
        gameObject.SetActive(false);
    }

    private void Emerge()
    {
        gameObject.SetActive(true);
        burrowed = false;
        timer = emergeCooldown;

        FireRing();
    }

    private void FireRing()
    {
        int count = 12;
        float step = 360f / count;

        for (int i = 0; i < count; i++)
        {
            float angle = i * step;
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject tear = Instantiate(tearPrefab, transform.position, rot);
            tear.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * 4f;
        }
    }

    public void WakeUp() => isAwake = true;
}

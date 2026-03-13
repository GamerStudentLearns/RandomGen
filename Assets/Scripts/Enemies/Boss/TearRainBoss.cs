using UnityEngine;

public class TearRainBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public GameObject tearPrefab;

    [Header("Craig Tear Rain Settings")]
    public float shotInterval = 0.15f;   // Craig's 0.15s
    public int tearsPerBurst = 5;        // Craig fires 5 per tick

    [Header("Cycle Settings")]
    public float burstDuration = 3f;     // how long it rains like Craig
    public float pauseDuration = 2f;     // how long it rests

    private float shotTimer;
    private float cycleTimer;
    private bool isRaining = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        shotTimer = shotInterval;
        cycleTimer = burstDuration;
    }

    private void Update()
    {
        if (!isAwake) return;

        cycleTimer -= Time.deltaTime;

        if (isRaining)
        {
            // --- TEAR RAIN ACTIVE ---
            shotTimer -= Time.deltaTime;

            if (shotTimer <= 0)
            {
                FireRain();
                shotTimer = shotInterval;
            }

            // Switch to pause mode
            if (cycleTimer <= 0)
            {
                isRaining = false;
                cycleTimer = pauseDuration;
            }
        }
        else
        {
            // --- PAUSE MODE ---
            if (cycleTimer <= 0)
            {
                isRaining = true;
                cycleTimer = burstDuration;
                shotTimer = shotInterval; // reset Craig timing cleanly
            }
        }
    }

    private void FireRain()
    {
        for (int i = 0; i < tearsPerBurst; i++)
        {
            float angle = Random.Range(0f, 360f);
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject tear = Instantiate(tearPrefab, transform.position, rot);
            tear.GetComponent<Rigidbody2D>().linearVelocity =
                rot * Vector2.right * Random.Range(3f, 6f);
        }
    }

    public void WakeUp() => isAwake = true;
}

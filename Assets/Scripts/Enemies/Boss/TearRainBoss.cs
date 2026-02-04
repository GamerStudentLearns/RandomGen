using UnityEngine;

public class TearRainBoss : MonoBehaviour, IBoss
{
    public Transform player;
    public bool isAwake = false;

    public GameObject tearPrefab;

    public float rainCooldown = 0.15f;
    private float rainTimer;

    public int tearsPerBurst = 5;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rainTimer = rainCooldown;
    }

    private void Update()
    {
        if (!isAwake) return;

        rainTimer -= Time.deltaTime;

        if (rainTimer <= 0)
        {
            FireRain();
            rainTimer = rainCooldown;
        }
    }

    private void FireRain()
    {
        for (int i = 0; i < tearsPerBurst; i++)
        {
            float angle = Random.Range(0f, 360f);
            Quaternion rot = Quaternion.Euler(0, 0, angle);

            GameObject tear = Instantiate(tearPrefab, transform.position, rot);
            tear.GetComponent<Rigidbody2D>().linearVelocity = rot * Vector2.right * Random.Range(3f, 6f);
        }
    }

    public void WakeUp() => isAwake = true;
}

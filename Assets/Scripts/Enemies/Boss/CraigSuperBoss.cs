using UnityEngine;

public class CraigSuperBoss : MonoBehaviour, IBoss
{
    public enum BossPhase
    {
        Monstro,
        Chub,
        Gurdy,
        HushFan,
        RapidBarrage,
        TearRain,
        DVDBounce,
        HolyBeams,
        MegaMaw,
        TunnelWorm,
        EyeLord,
        Fatty
    }

    [Header("General")]
    public bool isAwake = false;
    public Transform player;

    public GameObject tearPrefab;
    public GameObject minionPrefab;
    public GameObject beamPrefab;
    public GameObject eyePrefab;

    [Header("Phase Switching")]
    public float normalPhaseDuration = 8f;
    public float lowHPPhaseDuration = 2f;   // when below 10%
    private float phaseTimer;

    private int phaseCount;
    public BossPhase currentPhase;

    // Internal timers
    private float timerA;
    private float timerB;

    // DVD Bounce
    private Vector2 dvdDirection = new Vector2(1, 1);

    // EyeLord
    private GameObject leftEye;
    private GameObject rightEye;

    // Tunnel Worm
    private bool wormBurrowed = false;
    private Vector3 wormDir;

    // Chub
    private bool chubCharging = false;
    private Vector2 chubDir;

    // Monstro
    private bool monstroHopping = false;
    private Vector3 monstroTarget;

    // Gurdy
    private bool gurdySpawnNext = false;

    // Hush Fan
    private float hushAngle = -60f;
    private bool hushRight = true;

    // Mega Maw
    private float mawAngle = 0f;

    // Holy Beams
    private bool beamVertical = true;

    private SpriteRenderer spriteRenderer;
    private Collider2D col2D;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        phaseTimer = normalPhaseDuration;
        phaseCount = System.Enum.GetValues(typeof(BossPhase)).Length;

        dvdDirection = dvdDirection.normalized;

        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();

        // Create orbiting eyes for EyeLord phase, parented to boss
        leftEye = Instantiate(eyePrefab, transform.position, Quaternion.identity, transform);
        rightEye = Instantiate(eyePrefab, transform.position, Quaternion.identity, transform);

        timerA = 1f;
        timerB = 1f;
    }

    private void Update()
    {
        if (!isAwake || player == null)
            return;

        HandlePhaseSwitching();

        switch (currentPhase)
        {
            case BossPhase.Monstro: Phase_Monstro(); break;
            case BossPhase.Chub: Phase_Chub(); break;
            case BossPhase.Gurdy: Phase_Gurdy(); break;
            case BossPhase.HushFan: Phase_HushFan(); break;
            case BossPhase.RapidBarrage: Phase_RapidBarrage(); break;
            case BossPhase.TearRain: Phase_TearRain(); break;
            case BossPhase.DVDBounce: Phase_DVDBounce(); break;
            case BossPhase.HolyBeams: Phase_HolyBeams(); break;
            case BossPhase.MegaMaw: Phase_MegaMaw(); break;
            case BossPhase.TunnelWorm: Phase_TunnelWorm(); break;
            case BossPhase.EyeLord: Phase_EyeLord(); break;
            case BossPhase.Fatty: Phase_Fatty(); break;
        }

        ClampToRoom();
    }

    // ============================================================
    // PHASE SWITCHING
    // ============================================================

    private void HandlePhaseSwitching()
    {
        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0)
        {
            SwitchToRandomPhase();
            phaseTimer = normalPhaseDuration;
        }
    }

    private void SwitchToRandomPhase()
    {
        int next = Random.Range(0, phaseCount);

        while ((BossPhase)next == currentPhase)
            next = Random.Range(0, phaseCount);

        currentPhase = (BossPhase)next;

        // ⭐ Reset phase timers so phases don't inherit broken values
        timerA = 1f;
        timerB = 1f;
    }


    // Called by EnemyHealth.TakeDamage()
    public void OnDamaged(float current, float max)
    {
        float hpPercent = current / max;

        if (hpPercent <= 0.10f)
            phaseTimer = Mathf.Min(phaseTimer, lowHPPhaseDuration);
    }

    // ============================================================
    // PHASES
    // ============================================================

    // MONSTRO — Hop + Fan Shot (center aimed at player)
    private void Phase_Monstro()
    {
        timerA -= Time.deltaTime;

        if (monstroHopping)
        {
            transform.position = Vector3.MoveTowards(transform.position, monstroTarget, 6f * Time.deltaTime);

            if (Vector3.Distance(transform.position, monstroTarget) < 0.1f)
            {
                monstroHopping = false;
                FireFan(3, 15f, 6f);
            }
        }
        else
        {
            if (timerA <= 0)
            {
                monstroTarget = player.position + Vector3.up * 2f;
                monstroHopping = true;
                timerA = 2f;
            }
        }
    }

    // CHUB — Slow move + Charge + Aimed shot
    private void Phase_Chub()
    {
        timerA -= Time.deltaTime;

        if (chubCharging)
        {
            transform.position += (Vector3)(chubDir * 8f * Time.deltaTime);

            if (timerA <= 0)
            {
                chubCharging = false;
                timerA = 1.5f;

                // Fire one aimed shot after charge
                FireAimed(7f);
            }
        }
        else
        {
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)(dir * 2f * Time.deltaTime);

            if (timerA <= 0)
            {
                chubCharging = true;
                chubDir = dir;
                timerA = 1f;
            }
        }
    }

    // GURDY — Spawn + Fan Shot
    private void Phase_Gurdy()
    {
        timerA -= Time.deltaTime;

        if (timerA <= 0)
        {
            if (gurdySpawnNext)
                SpawnMinion();
            else
                FireFan(5, 10f, 5f);

            gurdySpawnNext = !gurdySpawnNext;
            timerA = 2f;
        }
    }

    // HUSH FAN — Sweeping Bullet Hell (not aimed)
    private void Phase_HushFan()
    {
        timerA -= Time.deltaTime;

        if (timerA <= 0)
        {
            if (hushRight)
            {
                hushAngle += 8f;
                if (hushAngle >= 60f) hushRight = false;
            }
            else
            {
                hushAngle -= 8f;
                if (hushAngle <= -60f) hushRight = true;
            }

            FireAngle(hushAngle, 4f);
            timerA = 0.1f;
        }
    }

    // RAPID BARRAGE — Fast Aimed Shots
    private void Phase_RapidBarrage()
    {
        timerA -= Time.deltaTime;

        if (timerA <= 0)
        {
            FireAimed(8f);
            timerA = 0.12f;
        }
    }

    // TEAR RAIN — Random Bullet Hell
    private void Phase_TearRain()
    {
        timerA -= Time.deltaTime;

        if (timerA <= 0)
        {
            for (int i = 0; i < 5; i++)
                FireAngle(Random.Range(0f, 360f), Random.Range(3f, 6f));

            timerA = 0.15f;
        }
    }

    // DVD BOUNCE — Bouncing Movement
    private void Phase_DVDBounce()
    {
        transform.position += (Vector3)(dvdDirection * 3f * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Wall"))
        {
            Vector2 normal = col.contacts[0].normal;
            dvdDirection = Vector2.Reflect(dvdDirection, normal).normalized;
        }
    }

    // HOLY BEAMS — Vertical/Horizontal Lines (not aimed)
    private void Phase_HolyBeams()
    {
        timerA -= Time.deltaTime;

        if (timerA <= 0)
        {
            if (beamVertical)
            {
                for (int i = -2; i <= 2; i++)
                    Instantiate(beamPrefab, new Vector3(i * 2f, transform.position.y, 0), Quaternion.identity);
            }
            else
            {
                for (int i = -2; i <= 2; i++)
                    Instantiate(beamPrefab, new Vector3(transform.position.x, i * 2f, 0), Quaternion.identity);
            }

            beamVertical = !beamVertical;
            timerA = 3f;
        }
    }

    // MEGA MAW — Rotating Bullet Hell
    private void Phase_MegaMaw()
    {
        mawAngle += 120f * Time.deltaTime;

        timerA -= Time.deltaTime;
        if (timerA <= 0)
        {
            FireAngle(mawAngle, 5f);
            timerA = 0.25f;
        }
    }

    // TUNNEL WORM — Burrow + Ring Burst (bullet hell)
    private void Phase_TunnelWorm()
    {
        timerA -= Time.deltaTime;

        if (wormBurrowed)
        {
            transform.position += wormDir * 6f * Time.deltaTime;

            // Keep worm inside room even while burrowed
            ClampToRoom();

            if (timerA <= 0)
            {
                wormBurrowed = false;

                if (spriteRenderer != null) spriteRenderer.enabled = true;
                if (col2D != null) col2D.enabled = true;

                FireRing(12, 4f);
                timerA = 2f;
            }
        }
        else
        {
            if (timerA <= 0)
            {
                wormBurrowed = true;
                wormDir = (player.position - transform.position).normalized;

                if (spriteRenderer != null) spriteRenderer.enabled = false;
                if (col2D != null) col2D.enabled = false;

                timerA = 1.5f;
            }
        }
    }

    // EYE LORD — Orbiting Eyes + Diagonal Shots (not aimed)
    private void Phase_EyeLord()
    {
        float orbitSpeed = 90f;
        float radius = 2f;

        float t = Time.time * orbitSpeed * Mathf.Deg2Rad;

        // Local positions so they follow the boss
        if (leftEye != null)
            leftEye.transform.localPosition = new Vector3(Mathf.Cos(t), Mathf.Sin(t)) * radius;

        if (rightEye != null)
            rightEye.transform.localPosition = new Vector3(Mathf.Cos(t + Mathf.PI), Mathf.Sin(t + Mathf.PI)) * radius;

        timerA -= Time.deltaTime;
        if (timerA <= 0)
        {
            if (leftEye != null)
                FireDiagonal(leftEye.transform.position);

            if (rightEye != null)
                FireDiagonal(rightEye.transform.position);

            timerA = 1.5f;
        }
    }

    // FATTY — Vacuum + Fan Shot
    private void Phase_Fatty()
    {
        timerA -= Time.deltaTime;

        Vector2 pull = (transform.position - player.position).normalized;
        player.position += (Vector3)(pull * 3f * Time.deltaTime);

        if (timerA <= 0)
        {
            FireFan(7, 10f, 7f);
            timerA = 3f;
        }
    }

    // ============================================================
    // ATTACK HELPERS
    // ============================================================

    // Aimed directly at player
    private void FireAimed(float speed)
    {
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject t = Instantiate(tearPrefab, transform.position, Quaternion.identity);
        t.GetComponent<Rigidbody2D>().linearVelocity = dir * speed;
    }

    // Fan centered on player (middle shot aimed)
    private void FireFan(int count, float angleStep, float speed)
    {
        Vector2 dir = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        int half = count / 2;
        for (int i = -half; i <= half; i++)
        {
            float angle = baseAngle + (i * angleStep);
            FireAngle(angle, speed);
        }
    }

    // Free-angle shot (for bullet hell)
    private void FireAngle(float angle, float speed)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

        GameObject t = Instantiate(tearPrefab, transform.position, Quaternion.identity);
        t.GetComponent<Rigidbody2D>().linearVelocity = dir * speed;
    }

    private void FireRing(int count, float speed)
    {
        float step = 360f / count;
        for (int i = 0; i < count; i++)
            FireAngle(i * step, speed);
    }

    private void FireDiagonal(Vector3 pos)
    {
        Vector2[] dirs = {
            new Vector2(1,1).normalized,
            new Vector2(-1,1).normalized,
            new Vector2(1,-1).normalized,
            new Vector2(-1,-1).normalized
        };

        foreach (var d in dirs)
        {
            GameObject t = Instantiate(tearPrefab, pos, Quaternion.identity);
            t.GetComponent<Rigidbody2D>().linearVelocity = d * 5f;
        }
    }

    private void SpawnMinion()
    {
        Vector2 offset = Random.insideUnitCircle * 2f;
        Instantiate(minionPrefab, transform.position + (Vector3)offset, Quaternion.identity);
    }

    // IBoss
    public void WakeUp() => isAwake = true;

    // ============================================================
    // ROOM CLAMP
    // ============================================================

    private void ClampToRoom()
    {
        float halfWidth = 15f;   // 30 / 2
        float halfHeight = 6f;   // 12 / 2

        Vector3 center = transform.parent != null ? transform.parent.position : Vector3.zero;
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, center.x - halfWidth, center.x + halfWidth);
        pos.y = Mathf.Clamp(pos.y, center.y - halfHeight, center.y + halfHeight);

        transform.position = pos;
    }
}

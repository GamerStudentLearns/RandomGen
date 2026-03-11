using UnityEngine;

public class DuoBoss : MonoBehaviour, IBoss
{
    [Header("General")]
    public bool isAwake = false;
    public Transform player;

    private EnemyHealth health;

    [Header("Fighters")]
    public GameObject fighterA;          // ButtersBoss
    public GameObject fighterB;          // MargarineBoss

    public MonoBehaviour fighterAScript; // ButtersBossAttack
    public MonoBehaviour fighterBScript; // MargarineBossAttack

    private enum BossPhase
    {
        Both,
        OnlyA,
        OnlyB
    }

    private BossPhase currentPhase = BossPhase.Both;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        health = GetComponent<EnemyHealth>();
        if (health == null)
        {
            Debug.LogError("DuoBoss requires EnemyHealth on the same GameObject!");
            return;
        }

        // Fighters start disabled until WakeUp() is called
        fighterAScript.enabled = false;
        fighterBScript.enabled = false;
    }

    private void Update()
    {
        // EXACTLY like CraigSuperBoss: do nothing until awakened
        if (!isAwake || player == null)
            return;

        HandlePhaseSwitching();
    }

    // ============================================================
    // PHASE LOGIC
    // ============================================================

    private void HandlePhaseSwitching()
    {
        float hpPercent = health.CurrentHealth / health.maxHealth;

        switch (currentPhase)
        {
            case BossPhase.Both:
                if (hpPercent <= 0.50f)
                    SwitchToOnlyA();
                break;

            case BossPhase.OnlyA:
                if (hpPercent <= 0.25f)
                    SwitchToOnlyB();
                break;

            case BossPhase.OnlyB:
                // Final phase
                break;
        }
    }

    private void SwitchToOnlyA()
    {
        currentPhase = BossPhase.OnlyA;

        // Butters active, Margarine inactive
        fighterAScript.enabled = true;
        fighterBScript.enabled = false;

        // Wake Butters
        if (fighterAScript is ButtersBoss a)
            a.isAwake = true;

        // Put Margarine to sleep
        if (fighterBScript is MargarineBoss b)
            b.isAwake = false;

        Debug.Log("DuoBoss switched to ButtersBoss only!");
    }

    private void SwitchToOnlyB()
    {
        currentPhase = BossPhase.OnlyB;

        // Margarine active, Butters inactive
        fighterAScript.enabled = false;
        fighterBScript.enabled = true;

        // Wake Margarine
        if (fighterBScript is MargarineBoss b)
            b.isAwake = true;

        // Put Butters to sleep
        if (fighterAScript is ButtersBoss a)
            a.isAwake = false;

        Debug.Log("DuoBoss switched to MargarineBoss only!");
    }

    // ============================================================
    // IBoss INTERFACE
    // ============================================================

    public void WakeUp()
    {
        isAwake = true;

        // Wake both fighters initially
        if (fighterAScript is ButtersBoss a)
            a.isAwake = true;

        if (fighterBScript is MargarineBoss b)
            b.isAwake = true;

        // Enable both scripts for the starting phase
        fighterAScript.enabled = true;
        fighterBScript.enabled = true;

        Debug.Log("DuoBoss awakened!");
    }

    public void Die()
    {
        // Optional cleanup
        if (fighterA != null) Destroy(fighterA);
        if (fighterB != null) Destroy(fighterB);
    }
}

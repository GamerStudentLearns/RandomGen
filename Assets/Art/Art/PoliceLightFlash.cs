using UnityEngine;

public class PoliceLightFlash : MonoBehaviour
{
    [Header("Assign your red + blue images")]
    public GameObject redLight;
    public GameObject blueLight;

    [Header("Flash Settings")]
    public float flashInterval = 0.2f;   // Time between flashes
    public float totalDuration = 3f;     // How long the flashing lasts

    private float flashTimer;
    private float durationTimer;
    private bool isFlashing = true;

    private void Start()
    {
        SetLights(true, false);
    }

    private void Update()
    {
        if (!isFlashing)
            return;

        durationTimer += Time.deltaTime;

        // Stop completely after duration
        if (durationTimer >= totalDuration)
        {
            isFlashing = false;
            SetLights(false, false); // turn everything off
            return;
        }

        // Flash logic
        flashTimer += Time.deltaTime;
        if (flashTimer >= flashInterval)
        {
            ToggleLights();
            flashTimer = 0f;
        }
    }

    private void ToggleLights()
    {
        bool redActive = redLight.activeSelf;
        SetLights(!redActive, redActive);
    }

    private void SetLights(bool redOn, bool blueOn)
    {
        if (redLight != null) redLight.SetActive(redOn);
        if (blueLight != null) blueLight.SetActive(blueOn);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class HoldToReload : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "MyScene";
    [SerializeField] private float holdDuration = 4f;

    private float holdTimer = 0f;
    private PlayerControls controls;
    private bool isHolding = false;

    void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Reload.started += ctx =>
        {
            isHolding = true;
            holdTimer = 0f;
        };

        controls.Player.Reload.canceled += ctx =>
        {
            isHolding = false;
            holdTimer = 0f;
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}

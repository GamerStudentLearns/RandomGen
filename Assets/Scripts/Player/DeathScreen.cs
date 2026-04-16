using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject deathScreenUI;
    public GameObject singleButton;

    [Header("Scene To Load")]
    public string sceneToLoad = "Level1";

    private PlayerControls controls;
    private bool usingController = false;

    void Awake()
    {
        controls = new PlayerControls();

        // Handle Submit (A / Cross / Enter / Space)
        controls.UI.Submit.performed += ctx =>
        {
            if (deathScreenUI.activeSelf)
                LoadScene();
        };
    }

    void OnEnable()
    {
        controls.Enable();
        InputSystem.onEvent += OnInputEvent;
    }

    void OnDisable()
    {
        controls.Disable();
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device is Gamepad)
        {
            usingController = true;
            Cursor.visible = false;
        }
        else if (device is Mouse)
        {
            usingController = false;
            Cursor.visible = true;
        }
    }

    public void ShowDeathScreen()
    {
        deathScreenUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !usingController;

        StartCoroutine(SelectNextFrame(singleButton));
    }

    private IEnumerator SelectNextFrame(GameObject button)
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void LoadScene()
    {
        // FULL INPUT SYSTEM RESET
        controls.Disable();                 // Disable UI map
        PlayerControls fresh = new PlayerControls();
        fresh.Player.Enable();              // Enable gameplay map

        // Optional: ensure time scale is normal
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneToLoad);
    }
}

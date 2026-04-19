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
    public GameObject restartButton;
    public GameObject menuButton;

    [Header("Scenes")]
    public string restartScene = "Level1";
    public string menuScene = "MainMenu";

    private PlayerControls controls;
    private bool usingController = false;

    void Awake()
    {
        controls = new PlayerControls();

        // Submit (A / Cross / Enter / Space)
        controls.UI.Submit.performed += ctx =>
        {
            if (!deathScreenUI.activeSelf) return;

            GameObject selected = EventSystem.current.currentSelectedGameObject;

            if (selected == restartButton)
                RestartGame();
            else if (selected == menuButton)
                GoToMenu();
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

    void Update()
    {
        if (!deathScreenUI.activeSelf) return;
        if (!usingController) return;

        Gamepad pad = Gamepad.current;
        if (pad == null) return;

        // D-Pad LEFT
        if (pad.dpad.left.wasPressedThisFrame)
            Select(restartButton);

        // D-Pad RIGHT
        if (pad.dpad.right.wasPressedThisFrame)
            Select(menuButton);

        // Left stick LEFT
        if (pad.leftStick.left.wasPressedThisFrame)
            Select(restartButton);

        // Left stick RIGHT
        if (pad.leftStick.right.wasPressedThisFrame)
            Select(menuButton);
    }

    public void ShowDeathScreen()
    {
        deathScreenUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !usingController;

        StartCoroutine(SelectNextFrame(restartButton));
    }

    private IEnumerator SelectNextFrame(GameObject button)
    {
        yield return null;
        Select(button);
    }

    private void Select(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    // -------------------------
    // BUTTON ACTIONS
    // -------------------------

    public void RestartGame()
    {
        ResetInputToPlayerMode();
        Time.timeScale = 1f;
        SceneManager.LoadScene(restartScene);
    }

    public void GoToMenu()
    {
        ResetInputToPlayerMode();
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }

    private void ResetInputToPlayerMode()
    {
        controls.Disable();

        PlayerControls fresh = new PlayerControls();
        fresh.Player.Enable();
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject firstSelectedButton;

    public static bool GameIsPaused = false;

    private PlayerControls controls;
    private bool usingController = false;

    private Mouse virtualMouse;
    private const float cursorSpeed = 1200f;

    void Awake()
    {
        controls = new PlayerControls();

        // FIX: Listen to Player.Pause, not UI.Cancel
        controls.Player.Pause.performed += ctx =>
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        };
    }

    void OnEnable()
    {
        controls.Enable();
        InputSystem.onEvent += OnInputEvent;

        // Create virtual mouse if needed
        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            InputUser.PerformPairingWithDevice(virtualMouse);
        }
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
        if (usingController)
            UpdateVirtualMouse();
    }

    private void UpdateVirtualMouse()
    {
        Vector2 stick = Gamepad.current.leftStick.ReadValue();
        Vector2 delta = stick * cursorSpeed * Time.unscaledDeltaTime;

        Vector2 currentPos = virtualMouse.position.ReadValue();
        Vector2 newPos = currentPos + delta;

        newPos.x = Mathf.Clamp(newPos.x, 0, Screen.width);
        newPos.y = Mathf.Clamp(newPos.y, 0, Screen.height);

        InputState.Change(virtualMouse.position, newPos);
        InputState.Change(virtualMouse.delta, delta);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        pauseMenuUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;

        pauseMenuUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = !usingController;

        // FIX: Delay selection by 1 frame
        StartCoroutine(SelectButtonNextFrame());
    }

    private IEnumerator SelectButtonNextFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

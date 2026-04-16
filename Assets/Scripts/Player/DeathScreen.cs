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
    public GameObject singleButton;   // Only one button now

    [Header("Scene To Load")]
    public string sceneToLoad;

    private PlayerControls controls;
    private bool usingController = false;

    void Awake()
    {
        controls = new PlayerControls();

        // Submit (Cross / A / Enter / Space)
        controls.UI.Submit.performed += ctx =>
        {
            if (!deathScreenUI.activeSelf) return;
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
        SceneManager.LoadScene(sceneToLoad);
    }
}

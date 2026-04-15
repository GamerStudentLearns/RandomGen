using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputDeviceSwitcher : MonoBehaviour
{
    public GameObject virtualMouse; // assign your virtual mouse UI/Prefab

    void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>()) return;

        // Gamepad input: Only switch if a button is pressed or stick moved
        if (device is Gamepad gamepad)
        {
            // Check for button press or stick movement
            if (gamepad.leftStick.ReadValue().sqrMagnitude > 0.01f ||
                gamepad.rightStick.ReadValue().sqrMagnitude > 0.01f ||
                gamepad.buttonSouth.isPressed ||
                gamepad.buttonNorth.isPressed ||
                gamepad.buttonEast.isPressed ||
                gamepad.buttonWest.isPressed ||
                gamepad.leftShoulder.isPressed ||
                gamepad.rightShoulder.isPressed ||
                gamepad.leftTrigger.ReadValue() > 0.1f ||
                gamepad.rightTrigger.ReadValue() > 0.1f ||
                gamepad.dpad.ReadValue().sqrMagnitude > 0.01f)
            {
                Debug.Log("Gamepad input detected");
                SetModeGamepad();
                return;
            }
        }

        // Mouse input
        if (device is Mouse)
        {
            Debug.Log("Mouse input detected");
            SetModeMouseKeyboard();
            return;
        }
        // Keyboard input
        if (device is Keyboard)
        {
            Debug.Log("Keyboard input detected");
            SetModeMouseKeyboard();
            return;
        }
    }

    void SetModeGamepad()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        if (virtualMouse) virtualMouse.SetActive(true);
    }

    void SetModeMouseKeyboard()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (virtualMouse) virtualMouse.SetActive(false);
    }
}
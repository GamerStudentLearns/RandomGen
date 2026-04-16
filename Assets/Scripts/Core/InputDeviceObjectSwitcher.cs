using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputDeviceObjectSwitcher : MonoBehaviour
{
    [Header("Objects to Toggle")]
    public GameObject controllerObject;   // Shown when using controller
    public GameObject keyboardMouseObject; // Shown when using KB/M

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        // -------------------------
        // CONTROLLER DETECTED
        // -------------------------
        if (device is Gamepad gamepad)
        {
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
                SetControllerMode();
                return;
            }
        }

        // -------------------------
        // MOUSE DETECTED
        // -------------------------
        if (device is Mouse)
        {
            SetKeyboardMouseMode();
            return;
        }

        // -------------------------
        // KEYBOARD DETECTED
        // -------------------------
        if (device is Keyboard)
        {
            SetKeyboardMouseMode();
            return;
        }
    }

    private void SetControllerMode()
    {
        if (controllerObject) controllerObject.SetActive(true);
        if (keyboardMouseObject) keyboardMouseObject.SetActive(false);
    }

    private void SetKeyboardMouseMode()
    {
        if (controllerObject) controllerObject.SetActive(false);
        if (keyboardMouseObject) keyboardMouseObject.SetActive(true);
    }
}

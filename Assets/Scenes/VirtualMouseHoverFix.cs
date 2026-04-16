using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class VirtualMouseHoverFix : MonoBehaviour
{
    private Mouse virtualMouse;

    void Update()
    {
        // If we haven't found the virtual mouse yet, try to find it
        if (virtualMouse == null)
        {
            foreach (var device in InputSystem.devices)
            {
                if (device is Mouse m && m.displayName.Contains("Virtual"))
                {
                    virtualMouse = m;
                    break;
                }
            }

            // Still not found? Stop here
            if (virtualMouse == null)
                return;
        }

        // Force a pointer update every frame
        Vector2 pos = virtualMouse.position.ReadValue();
        InputState.Change(virtualMouse.position, pos);
        InputState.Change(virtualMouse.delta, Vector2.zero);
    }
}

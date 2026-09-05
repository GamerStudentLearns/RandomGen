using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.LowLevel;

public class VirtualMouseUI : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;
    private VirtualMouseInput virtualMouseInput;

    private void Awake()
    {
        if (FindObjectsByType<VirtualMouseUI>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        virtualMouseInput = GetComponent<VirtualMouseInput>();
    }

    public void Update()
    {
        transform.localScale = Vector3.one * 1f / canvasRectTransform.localScale.x;
        transform.SetAsLastSibling();
    }

    private void LateUpdate()
    {
        if (virtualMouseInput == null)
            return;

        var mouse = virtualMouseInput.virtualMouse;
        if (mouse == null || !mouse.added)
            return;

        Vector2 virtualMousePosition = mouse.position.value;
        virtualMousePosition.x = Mathf.Clamp(virtualMousePosition.x, 0f, Screen.width);
        virtualMousePosition.y = Mathf.Clamp(virtualMousePosition.y, 0f, Screen.height);
        InputState.Change(mouse.position, virtualMousePosition);
    }
}
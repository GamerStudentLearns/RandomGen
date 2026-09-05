using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>
/// Self-contained virtual mouse: creates a Mouse device driven by gamepad-style stick/button
/// input, and drives a software cursor UI element to match. Avoids the anchoredPosition
/// initialization bug found in the built-in VirtualMouseInput component.
/// </summary>
[AddComponentMenu("Input/Custom Virtual Mouse")]
public class CustomVirtualMouse : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private Graphic cursorGraphic;

    [Header("Motion")]
    [SerializeField] private float cursorSpeed = 1200f;
    [SerializeField] private float scrollSpeed = 45f;

    [Header("Input")]
    [SerializeField] private InputActionReference stickAction;
    [SerializeField] private InputActionReference leftClickAction;
    [SerializeField] private InputActionReference rightClickAction;
    [SerializeField] private InputActionReference scrollAction;

    public Mouse VirtualMouse { get; private set; }

    private Vector2 lastStickValue;
    private bool positionInitialized;

    private void OnEnable()
    {
        // Reuse existing device if it still exists but was removed (e.g. after a disable), otherwise create fresh.
        if (VirtualMouse == null)
        {
            VirtualMouse = InputSystem.GetDevice<Mouse>("CustomVirtualMouse") as Mouse;
            if (VirtualMouse == null)
                VirtualMouse = InputSystem.AddDevice<Mouse>("CustomVirtualMouse");
        }
        else if (!VirtualMouse.added)
        {
            InputSystem.AddDevice(VirtualMouse);
        }

        positionInitialized = false;

        stickAction?.action?.Enable();
        leftClickAction?.action?.Enable();
        rightClickAction?.action?.Enable();
        scrollAction?.action?.Enable();
    }

    private void OnDisable()
    {
        if (VirtualMouse != null && VirtualMouse.added)
            InputSystem.RemoveDevice(VirtualMouse);

        stickAction?.action?.Disable();
        leftClickAction?.action?.Disable();
        rightClickAction?.action?.Disable();
        scrollAction?.action?.Disable();
    }

    private void Update()
    {
        // Keep cursor graphic crisp regardless of canvas scale, and always render on top.
        if (canvasRectTransform != null)
            transform.localScale = Vector3.one / canvasRectTransform.localScale.x;
        transform.SetAsLastSibling();
    }

    private void LateUpdate()
    {
        if (VirtualMouse == null || !VirtualMouse.added)
            return;

        // Initialize in real screen-pixel space exactly once, avoiding the anchoredPosition bug.
        if (!positionInitialized)
        {
            Vector2 startPos = new Vector2(Screen.width / 2f, Screen.height / 2f);
            InputState.Change(VirtualMouse.position, startPos);
            if (cursorTransform != null)
                cursorTransform.anchoredPosition = ScreenToCanvasPosition(startPos);
            positionInitialized = true;
            return;
        }

        UpdateMotion();
        UpdateButtons();
        UpdateScroll();

        // Sync visual cursor to device position.
        if (cursorTransform != null)
        {
            Vector2 pos = VirtualMouse.position.value;
            cursorTransform.anchoredPosition = ScreenToCanvasPosition(pos);
        }
    }

    private void UpdateMotion()
    {
        var action = stickAction != null ? stickAction.action : null;
        if (action == null)
            return;

        Vector2 stickValue = action.ReadValue<Vector2>();

        if (Mathf.Approximately(stickValue.x, 0f) && Mathf.Approximately(stickValue.y, 0f))
        {
            lastStickValue = Vector2.zero;
            return;
        }

        Vector2 currentPos = VirtualMouse.position.value;
        Vector2 delta = stickValue * cursorSpeed * Time.unscaledDeltaTime;
        Vector2 newPos = currentPos + delta;

        newPos.x = Mathf.Clamp(newPos.x, 0f, Screen.width);
        newPos.y = Mathf.Clamp(newPos.y, 0f, Screen.height);

        InputState.Change(VirtualMouse.position, newPos);
        InputState.Change(VirtualMouse.delta, delta);

        lastStickValue = stickValue;
    }

    private void UpdateButtons()
    {
        SetButton(leftClickAction, MouseButton.Left);
        SetButton(rightClickAction, MouseButton.Right);
    }

    private void SetButton(InputActionReference actionRef, MouseButton button)
    {
        var action = actionRef != null ? actionRef.action : null;
        if (action == null)
            return;

        bool isPressed = action.IsPressed();
        VirtualMouse.CopyState<MouseState>(out var state);
        state.WithButton(button, isPressed);
        InputState.Change(VirtualMouse, state);
    }

    private void UpdateScroll()
    {
        var action = scrollAction != null ? scrollAction.action : null;
        if (action == null)
            return;

        Vector2 scrollValue = action.ReadValue<Vector2>() * scrollSpeed;
        if (scrollValue != Vector2.zero)
            InputState.Change(VirtualMouse.scroll, scrollValue);
    }

    /// <summary>
    /// Converts real screen-pixel coordinates to the cursor RectTransform's parent-local
    /// anchored position, accounting for canvas scale factor (Screen Space - Overlay).
    /// </summary>
    private Vector2 ScreenToCanvasPosition(Vector2 screenPos)
    {
        if (canvasRectTransform == null)
            return screenPos;

        float scale = canvasRectTransform.localScale.x;
        // Canvas is assumed Screen Space - Overlay, anchored/pivoted such that (0,0) screen
        // pixel maps near the canvas rect's bottom-left. Adjust here if using Camera/World space.
        Vector2 canvasSize = canvasRectTransform.rect.size;
        Vector2 normalized = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        return new Vector2(
            (normalized.x - canvasRectTransform.pivot.x) * canvasSize.x,
            (normalized.y - canvasRectTransform.pivot.y) * canvasSize.y
        );
    }
}
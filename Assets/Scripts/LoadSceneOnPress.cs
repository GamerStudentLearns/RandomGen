using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LoadSceneOnPress : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "MyScene";

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.LoadScene.performed += OnLoadScene;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void OnLoadScene(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}

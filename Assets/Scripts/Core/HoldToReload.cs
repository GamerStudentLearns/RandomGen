using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldToReload : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "MyScene";
    [SerializeField] private float holdDuration = 4f;

    private float holdTimer = 0f;

    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            holdTimer += Time.deltaTime;

            if (holdTimer >= holdDuration)
            {
                SceneManager.LoadScene(sceneToLoad);
            }
        }
        else
        {
            holdTimer = 0f;
        }
    }
}

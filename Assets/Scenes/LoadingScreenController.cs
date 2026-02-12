using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    public GameObject loadingCanvas;   // Assign your canvas here
    public float duration = 5f;

    private MonoBehaviour PlayerMovement;

    private void Start()
    {
        // Automatically find the playerMovement script in the scene
        PlayerMovement = FindObjectOfType<PlayerMovement>();

        StartCoroutine(ShowLoadingScreen());
    }

    private System.Collections.IEnumerator ShowLoadingScreen()
    {
        loadingCanvas.SetActive(true);

        if (PlayerMovement != null)
            PlayerMovement.enabled = false;

        yield return new WaitForSeconds(duration);

        loadingCanvas.SetActive(false);

        if (PlayerMovement != null)
            PlayerMovement.enabled = true;
    }
}

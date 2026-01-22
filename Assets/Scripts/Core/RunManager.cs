using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager instance;

    public int currentHearts;
    public int maxHearts;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

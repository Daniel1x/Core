using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField, Min(0)] private int sceneToLoadID = 1;

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoadID);
    }
}

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private AssetReference startScene = new();

    private void Start()
    {
        LoadLevel(startScene);
    }

    public static void LoadLevel(AssetReference _sceneReference, LoadSceneMode _mode = LoadSceneMode.Single, SceneReleaseMode _release = SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
    {
        if (_sceneReference.RuntimeKeyIsValid())
        {
            Addressables.LoadSceneAsync(_sceneReference, _mode, _release);
        }
    }
}

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private AssetReference startScene = new();

    private void Start()
    {
        if (startScene.RuntimeKeyIsValid())
        {
            Addressables.LoadSceneAsync(startScene, LoadSceneMode.Single, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
        }
    }
}

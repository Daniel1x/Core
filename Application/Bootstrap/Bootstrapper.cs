using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private AssetReference startScene = new();

    private void Start()
    {
        startScene.LoadLevel();
    }
}

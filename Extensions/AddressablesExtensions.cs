using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AddressablesExtensions
{
    public static bool ReleaseAssetInstanceOrDestroy(this GameObject _obj)
    {
        if (_obj == null)
        {
            return false;
        }

        if (Addressables.ReleaseInstance(_obj))
        {
            return true;
        }

        Object.Destroy(_obj);
        return false;
    }
}

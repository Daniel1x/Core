namespace DL.AssetLoading
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    [DisallowMultipleComponent]
    public class AutoReleaseForInstantiateAsync : MonoBehaviour
    {
        private AsyncOperationHandle<GameObject>? instantiationHandle;
        private AssetReferenceGameObject assetReference;
        private bool released;

        private void OnDestroy()
        {
            if (released)
            {
                return;
            }

            // Priority: 1. Use the instantiation handle if available
            if (instantiationHandle.HasValue && instantiationHandle.Value.IsValid())
            {
                Addressables.Release(instantiationHandle.Value);
                released = true;
                return;
            }

            // Priority: 2. Use the asset reference if available
            if (assetReference != null && assetReference.RuntimeKeyIsValid())
            {
                assetReference.ReleaseInstance(gameObject);
                released = true;
                return;
            }

            // Priority: 3. Fallback to direct release
            Addressables.ReleaseInstance(gameObject);
            released = true;
        }

        /// <summary>
        /// Configures automatic release behavior for a spawned <see cref="GameObject"/> and associates it with optional
        /// asset management references.
        /// </summary>
        /// <param name="_spawnedAsset">The <see cref="GameObject"/> to which the automatic release behavior will be added. This parameter cannot be
        /// <see langword="null"/>.</param>
        /// <param name="_usedReference">An optional <see cref="AssetReferenceGameObject"/> representing the asset reference used to spawn the <see
        /// cref="GameObject"/>. This parameter can be <see langword="null"/>.</param>
        /// <param name="_handle">An optional <see cref="AsyncOperationHandle{T}"/> representing the handle for the asynchronous instantiation
        /// operation. This parameter can be <see langword="null"/>.</param>
        /// <returns>An instance of <see cref="AutoReleaseForInstantiateAsync"/> added to the <paramref name="_spawnedAsset"/>.
        /// Returns <see langword="null"/> if <paramref name="_spawnedAsset"/> is <see langword="null"/>.</returns>
        public static AutoReleaseForInstantiateAsync SetupAutoRelease(GameObject _spawnedAsset, AssetReferenceGameObject _usedReference = null, AsyncOperationHandle<GameObject>? _handle = null)
        {
            if (_spawnedAsset == null)
            {
                return null;
            }

            AutoReleaseForInstantiateAsync _autoRelease = _spawnedAsset.AddComponent<AutoReleaseForInstantiateAsync>();
            _autoRelease.instantiationHandle = _handle;
            _autoRelease.assetReference = _usedReference;
            _autoRelease.released = false;
            return _autoRelease;
        }
    }
}

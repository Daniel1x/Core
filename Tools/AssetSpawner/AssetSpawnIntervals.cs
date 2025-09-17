namespace DL.AssetLoading
{
    using DL.Structs;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class AssetSpawnIntervals : MonoBehaviour
    {
        private const float MAX_INTERVAL_RANGE = 120f;

        [Header("Timing")]
        [SerializeField, MinMaxSlider(0f, MAX_INTERVAL_RANGE)] private MinMax intervalRange = new(1f, 5f);
        [SerializeField, Min(0f)] private float initialDelay = 0f;
        [SerializeField] private bool useUnscaledTime = false;
        [SerializeField] private bool spawnOnStart = true;

        [Header("Asset")]
        [SerializeField] private AssetReferenceGameObject assetReference = new(string.Empty);

        [Header("Spawn Params")]
        [SerializeField] private Vector3 randomPositionSphereScale = Vector3.zero;
        [SerializeField] private bool parentToThisTransform = false;

        private float nextSpawnTime = 0f;

        private float TimeNow => useUnscaledTime ? Time.unscaledTime : Time.time;

        private void Start()
        {
            if (spawnOnStart && initialDelay <= 0f)
            {
                spawnAsset();
            }
            else
            {
                nextSpawnTime = TimeNow + (spawnOnStart ? initialDelay : intervalRange.RandomValue);
            }
        }

        private void Update()
        {
            if (TimeNow >= nextSpawnTime)
            {
                spawnAsset();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (randomPositionSphereScale != Vector3.zero)
            {
                var _defaultMatrix = Gizmos.matrix;
                var _defaultColor = Gizmos.color;

                Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, randomPositionSphereScale);
                Gizmos.DrawSphere(Vector3.zero, 1f);
                Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
                Gizmos.DrawWireSphere(Vector3.zero, 1f);

                Gizmos.matrix = _defaultMatrix;
                Gizmos.color = _defaultColor;
            }
        }

        private void spawnAsset()
        {
            if (assetReference.RuntimeKeyIsValid())
            {
                Vector3 _position = Random.insideUnitSphere;
                _position.Scale(randomPositionSphereScale);
                _position += transform.position;

                Transform _parent = parentToThisTransform ? transform : null;
                Addressables.InstantiateAsync(assetReference, _position, transform.rotation, _parent).Completed += onComplete;
            }

            nextSpawnTime = TimeNow + intervalRange.RandomValue;
        }

        private void onComplete(AsyncOperationHandle<GameObject> _handle)
        {
            if (_handle.Status is AsyncOperationStatus.Succeeded)
            {
                AutoReleaseForInstantiateAsync.SetupAutoRelease(_handle.Result, assetReference, _handle);
            }
            else if (_handle.IsValid())
            {
                Addressables.Release(_handle);
            }
        }
    }
}

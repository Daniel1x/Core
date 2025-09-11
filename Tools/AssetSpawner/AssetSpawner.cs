namespace DL.AssetLoading
{
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    [DisallowMultipleComponent, ExecuteAlways, SelectionBase]
    public class AssetSpawner : MonoBehaviour
    {
        [SerializeField] private AssetReferenceGameObject assetReference = new(string.Empty);

        private GameObject runtimeInstance;
        private bool isSpawning;

        public AssetReferenceGameObject AssetReference
        {
            get => assetReference;
            set
            {
#if UNITY_EDITOR
                if (assetReference != value)
                {
                    assetReference = value;
                    markPreviewDirty();
                }
#endif
            }
        }

#if UNITY_EDITOR
        // Editor preview (non-serialized)
        private GameObject previewInstance { get; set; }
        private AssetReferenceGameObject lastEditorReference;
        private Vector3 lastPos;
        private Quaternion lastRot;
        private Vector3 lastScale;
        private bool previewDirty;
#endif

        private void Awake()
        {
            if (Application.isPlaying)
            {
                gameObject.hideFlags = HideFlags.HideAndDontSave; // Hide in hierarchy during play mode
                spawnRuntime();
            }
#if UNITY_EDITOR
            else
            {
                markPreviewDirty();
            }
#endif
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            destroyPreviewInstance();
        }

        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                markPreviewDirty();
            }
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                markPreviewDirty();
            }
        }

        private void Update()
        {
            if (Application.isPlaying || previewInstance == null)
            {
                return;
            }

            if (transform.localPosition != lastPos
                || transform.localRotation != lastRot
                || transform.localScale != lastScale)
            {
                alignPreviewTransform();
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
            {
                ensureEditorPreview();
            }
        }
#endif

        private void spawnRuntime()
        {
            if (runtimeInstance != null
                || isSpawning
                || !assetReference.RuntimeKeyIsValid())
            {
                return;
            }

            isSpawning = true;

            assetReference.InstantiateAsync(transform.parent).Completed += _handle =>
            {
                isSpawning = false;

                if (_handle.Status is not AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(_handle);
                    return;
                }

                runtimeInstance = _handle.Result;
                AutoReleaseForInstantiateAsync.SetupAutoRelease(runtimeInstance, assetReference, _handle);
                copyTransformTo(runtimeInstance.transform);

                runtimeInstance.name = gameObject.name;
            };
        }

        private void copyTransformTo(Transform _target)
        {
            _target.SetParent(transform.parent);
            _target.localPosition = transform.localPosition;
            _target.localRotation = transform.localRotation;
            _target.localScale = transform.localScale;
        }

#if UNITY_EDITOR
        private void markPreviewDirty()
        {
            previewDirty = true;
        }

        private void ensureEditorPreview()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (!previewDirty
                && previewInstance != null
                && lastEditorReference == assetReference)
            {
                return;
            }

            previewDirty = false;

            if (assetReference == null || assetReference.editorAsset == null)
            {
                destroyPreviewInstance();
                lastEditorReference = null;
                return;
            }

            if (previewInstance == null || lastEditorReference != assetReference)
            {
                destroyPreviewInstance();

                if (assetReference.editorAsset is not GameObject _prefab)
                {
                    lastEditorReference = null;
                    return;
                }

                previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(_prefab);

                if (previewInstance == null)
                {
                    return;
                }

                previewInstance.name = $"{_prefab.name}_Preview";
                previewInstance.hideFlags = HideFlags.HideAndDontSave;
                previewInstance.transform.SetParent(transform.parent);
                previewInstance.GetOrAddComponent<AssetSelectionBase>(out _).Redirection = gameObject;
                previewInstance.GetOrAddComponent<NotSavableObject>(out _);
            }

            alignPreviewTransform();
            lastEditorReference = assetReference;
        }

        private void alignPreviewTransform()
        {
            if (previewInstance == null)
            {
                return;
            }

            lastPos = transform.localPosition;
            lastRot = transform.localRotation;
            lastScale = transform.localScale;

            copyTransformTo(previewInstance.transform);
        }

        private void destroyPreviewInstance()
        {
            if (previewInstance == null)
            {
                return;
            }

            DestroyImmediate(previewInstance);
            previewInstance = null;
        }
#endif
    }
}

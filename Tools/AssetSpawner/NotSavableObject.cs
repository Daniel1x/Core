namespace DL.AssetLoading
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif

    [ExecuteAlways]
    public class NotSavableObject : MonoBehaviour
    {
        [SerializeField] private bool destroyOnPrefabSave = true;
        [SerializeField] private bool destroyOnSceneSave = true;
        [SerializeField] private bool destroyOnPlayModeChange = true;

#if UNITY_EDITOR
        private void Awake() => register();
        private void OnEnable() => register();
        private void OnDestroy() => unregister();

        private void register()
        {
            unregister();

            EditorSceneManager.sceneSaving += onSceneSaving;
            PrefabStage.prefabSaving += onPrefabSaving;
            EditorApplication.playModeStateChanged += onPlayModeStateChanged;
        }

        private void unregister()
        {
            EditorSceneManager.sceneSaving -= onSceneSaving;
            PrefabStage.prefabSaving -= onPrefabSaving;
            EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
        }

        private void onPrefabSaving(GameObject _object) => destroyThisObject(true, destroyOnPrefabSave);
        private void onSceneSaving(Scene _scene, string _path) => destroyThisObject(false, destroyOnSceneSave);
        private void onPlayModeStateChanged(PlayModeStateChange _state) => destroyThisObject(false, destroyOnPlayModeChange);

        private void destroyThisObject(bool _fromPrefab, bool _flag)
        {
            if (this == null
                || _flag == false
                || gameObject == null
                || Application.isPlaying)
            {
                return; // Just in case
            }

            if (_fromPrefab
                && PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return; // Don't destroy if saving the prefab asset itself
            }

            if (_fromPrefab == false
                && gameObject.scene.IsValid() == false
                && PrefabUtility.IsPartOfPrefabInstance(gameObject) == false)
            {
                return; // Don't destroy if not in a scene and not part of a prefab instance
            }

            DestroyImmediate(gameObject);
        }
#endif

        public static NotSavableObject Mark(GameObject _object, bool _destroyOnPrefabSave = true, bool _destroyOnSceneSave = true, bool _destroyOnPlayModeChange = true)
        {
            if (_object == null)
            {
                return null;
            }

            NotSavableObject _notSavable = _object.GetOrAddComponent<NotSavableObject>(out _);
            _notSavable.destroyOnPrefabSave = _destroyOnPrefabSave;
            _notSavable.destroyOnSceneSave = _destroyOnSceneSave;
            _notSavable.destroyOnPlayModeChange = _destroyOnPlayModeChange;

            return _notSavable;
        }
    }
}

namespace DL.AssetLoading
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif

    [ExecuteAlways]
    public class PrefabOverrideBlocker : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake() => register();
        private void OnEnable() => register();
        private void OnDestroy() => unregister();

        private void register()
        {
            unregister();

            EditorSceneManager.sceneSaving += onSceneSaving;
            EditorSceneManager.sceneSaved += onSceneSaved;
        }

        private void unregister()
        {
            EditorSceneManager.sceneSaving -= onSceneSaving;
            EditorSceneManager.sceneSaved -= onSceneSaved;
        }

        private void onSceneSaved(Scene _scene) => revertPrefabOverrides();
        private void onSceneSaving(Scene _scene, string _path) => revertPrefabOverrides(true);

        protected void revertPrefabOverrides(bool _setDirty = false)
        {
            if (this == null
                || gameObject == null
                || Application.isPlaying)
            {
                return;
            }

            if (gameObject == null)
            {
                unregister();
                return;
            }

            if (gameObject.scene.IsValid()
                && PrefabUtility.IsPartOfAnyPrefab(gameObject)
                && PrefabUtility.HasPrefabInstanceAnyOverrides(gameObject, false))
            {
                PrefabUtility.RevertPrefabInstance(gameObject, InteractionMode.AutomatedAction);

                if (_setDirty)
                {
                    EditorUtilities.SetDirty(gameObject);
                }
            }
        }
#endif
    }
}

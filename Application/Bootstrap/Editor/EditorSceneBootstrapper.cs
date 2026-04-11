namespace DL.Application.Bootstrapper
{
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [InitializeOnLoad]
    public class EditorSceneBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticData()
        {
            s_RestartingToSwitchScene = false;
        }

        private const string k_MenuPath = "DL/Bootstrapper/";
        private const string k_PreviousSceneKey = "PreviousScene";
        private const string k_ShouldLoadBootstrapSceneKey = "LoadBootstrapScene";
        private const string k_LoadBootstrapSceneOnPlay = k_MenuPath + "Load Bootstrap Scene On Play";
        private const string k_DoNotLoadBootstrapSceneOnPlay = k_MenuPath + "Don't Load Bootstrap Scene On Play";
        private const string k_TestRunnerSceneName = "InitTestScene";

        private static bool s_RestartingToSwitchScene;

        private static string bootstrapScene => EditorBuildSettings.scenes[0].path;

        private static string previousScene
        {
            get => EditorPrefs.GetString(k_PreviousSceneKey);
            set => EditorPrefs.SetString(k_PreviousSceneKey, value);
        }

        private static bool shouldLoadBootstrapScene
        {
            get
            {
                if (!EditorPrefs.HasKey(k_ShouldLoadBootstrapSceneKey))
                {
                    EditorPrefs.SetBool(k_ShouldLoadBootstrapSceneKey, true);
                }

                return EditorPrefs.GetBool(k_ShouldLoadBootstrapSceneKey, true);
            }

            set => EditorPrefs.SetBool(k_ShouldLoadBootstrapSceneKey, value);
        }

        private static bool isTestRunnerActive => EditorSceneManager.GetActiveScene().name.StartsWith(k_TestRunnerSceneName);

        static EditorSceneBootstrapper()
        {
            EditorApplication.playModeStateChanged -= onPlayModeStateChanged;
            EditorApplication.playModeStateChanged += onPlayModeStateChanged;
        }

        [MenuItem(k_LoadBootstrapSceneOnPlay, true)] private static bool showLoadBootstrapSceneOnPlay() => !shouldLoadBootstrapScene;
        [MenuItem(k_LoadBootstrapSceneOnPlay)] private static void enableLoadBootstrapSceneOnPlay() => shouldLoadBootstrapScene = true;
        [MenuItem(k_DoNotLoadBootstrapSceneOnPlay, true)] private static bool showDoNotLoadBootstrapSceneOnPlay() => shouldLoadBootstrapScene;
        [MenuItem(k_DoNotLoadBootstrapSceneOnPlay)] private static void disableDoNotLoadBootstrapSceneOnPlay() => shouldLoadBootstrapScene = false;

        private static void onPlayModeStateChanged(PlayModeStateChange _playModeStateChange)
        {
            if (isTestRunnerActive)
            {
                return; // Do not interfere with test runner scenes
            }

            if (!shouldLoadBootstrapScene)
            {
                return; // feature is disabled
            }

            if (s_RestartingToSwitchScene)
            {
                if (_playModeStateChange == PlayModeStateChange.EnteredPlayMode)
                {
                    // for some reason there's multiple start and stops events happening while restarting the editor playmode. We're making sure to
                    // set stoppingAndStarting only when we're done and we've entered playmode. This way we won't corrupt "activeScene" with the multiple
                    // start and stop and will be able to return to the scene we were editing at first
                    s_RestartingToSwitchScene = false;
                }

                return; // we're already restarting to switch scene; ignore further playmode state changes
            }

            if (_playModeStateChange == PlayModeStateChange.ExitingEditMode)
            {
                // cache previous scene so we return to this scene after play session, if possible
                previousScene = EditorSceneManager.GetActiveScene().path;

                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    // user either hit "Save" or "Don't Save"; open bootstrap scene

                    if (!string.IsNullOrEmpty(bootstrapScene) && System.Array.Exists(EditorBuildSettings.scenes, scene => scene.path == bootstrapScene))
                    {
                        Scene _activeScene = EditorSceneManager.GetActiveScene();

                        s_RestartingToSwitchScene = _activeScene.path == string.Empty || !bootstrapScene.Contains(_activeScene.path);

                        // we only manually inject Bootstrap scene if we are in a blank empty scene,
                        // or if the active scene is not already BootstrapScene
                        if (s_RestartingToSwitchScene)
                        {
                            EditorApplication.isPlaying = false;

                            // scene is included in build settings; open it
                            EditorSceneManager.OpenScene(bootstrapScene);

                            EditorApplication.isPlaying = true;
                        }
                    }
                }
                else
                {
                    // user either hit "Cancel" or exited window; don't open bootstrap scene & return to editor
                    EditorApplication.isPlaying = false;
                }
            }
            else if (_playModeStateChange == PlayModeStateChange.EnteredEditMode)
            {
                if (!string.IsNullOrEmpty(previousScene))
                {
                    EditorSceneManager.OpenScene(previousScene);
                }
            }
        }
    }
}

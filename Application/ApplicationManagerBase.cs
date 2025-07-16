using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class ApplicationManagerBase : MonoBehaviour
{
    public enum InitializationState
    {
        Uninitialized = 0,
        Initializing = 1,
        Initialized = 2,
    }

    public static event UnityAction<ScreenResolution> OnScreenResolutionChanged = null;

    public static bool Exists => instance != null;
    public static InitializationState State => instance != null ? instance.initializationState : InitializationState.Uninitialized;

    private static ApplicationManagerBase instance = null;

    [Header("Initialization Modules")]
    [SerializeField] protected InitializationBehaviour[] initializationBehaviours = new InitializationBehaviour[] { };
    [SerializeField] protected InitializationObject[] initializationObjects = new InitializationObject[] { };

    [Header("FPS Display")]
    [SerializeField] protected bool showFPS = false;
    [SerializeField] protected int averageOverNFrames = 120;

    protected InitializationState initializationState = InitializationState.Uninitialized;
    protected float averageFPS = 0f;
    protected float[] fpsArray = null;
    protected int fpsIndex = -1;

    protected ScreenResolutionChecker screenResolutionChecker = new ScreenResolutionChecker();

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        instance = this;
        screenResolutionChecker.CheckForResolutionChange();
    }

    protected virtual IEnumerator Start()
    {
        initializationState = InitializationState.Initializing;

        yield return initializationBehaviours.HandleInitialization();
        yield return initializationObjects.HandleInitialization();

        IInitializationModule[] _additionalModules = GetComponentsInChildren<IInitializationModule>(true);

        if (_additionalModules != null && _additionalModules.Length > 0)
        {
            foreach (IInitializationModule _module in _additionalModules)
            {
                if (initializationBehaviours.Contains(_module) || initializationObjects.Contains(_module))
                {
                    continue; // Skip modules that are already handled
                }

                yield return _module.HandleModuleInitialization();
            }
        }

        initializationState = InitializationState.Initialized;
        MyLog.Log("All ApplicationModules initialized successfully.");
    }

    protected virtual void Update()
    {
        if (screenResolutionChecker.CheckForResolutionChange())
        {
            OnScreenResolutionChanged?.Invoke(screenResolutionChecker.Resolution);
        }

        float _currentFPS = 1f / Time.unscaledDeltaTime;

        if (fpsArray == null || fpsArray.Length != averageOverNFrames)
        {
            fpsIndex = -1;
            fpsArray = new float[averageOverNFrames];
        }

        fpsIndex++;

        if (fpsIndex >= averageOverNFrames)
        {
            fpsIndex = 0;
            averageFPS = 0f;

            for (int i = 0; i < fpsArray.Length; i++)
            {
                averageFPS += fpsArray[i];
            }

            averageFPS /= fpsArray.Length;
        }

        fpsArray[fpsIndex] = _currentFPS;
    }

    protected virtual void OnGUI()
    {
        if (showFPS == false)
        {
            return;
        }

        //Adjust font size based on screen resolution, default is 1920x1080, scale it accordingly
        float _scaleFactor = Mathf.Min(Screen.width / 1920f, Screen.height / 1080f);

        GUIStyle _style = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.RoundToInt(50 * _scaleFactor),
            normal = { textColor = Color.white }
        };

        GUILayout.BeginArea(new Rect(10, 10, 300 * _scaleFactor, 100 * _scaleFactor));
        GUILayout.Label($"FPS: {Mathf.RoundToInt(averageFPS)}", _style);
        GUILayout.EndArea();
    }
}
namespace DL.Editor.TimeScaler
{
    using UnityEditor;
    using UnityEngine;

    public class TimeScalerWindow : EditorWindow
    {
        public const float MIN_WIDTH = 300f;
        public const float MAX_WIDTH = 1000f;
        public const float HEIGHT = 65f;

        [SerializeField] private float max = 5f;
        [SerializeField] private float scale = 1f;
        [SerializeField] private float step = 0.1f;

        protected void Update()
        {
            Repaint();
        }

        protected void OnGUI()
        {
            using (new HorizontalScope())
            {
                EditorGUILayout.LabelField("Time Scale:", EditorStyles.boldLabel, EditorDrawing.GetElementWidth(70f));

                using (new DisabledScope())
                {
                    EditorGUILayout.FloatField(Time.timeScale);
                }
            }

            using (new LabelWidthScope(35f))
            {
                using (new HorizontalScope())
                {
                    scale = round(EditorGUILayout.Slider("Scale", scale, 0f, max), step);

                    if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
                    {
                        scale = 1f;
                    }
                }

                using (new HorizontalScope())
                {
                    step = Mathf.Clamp(EditorGUILayout.FloatField("Step", step), 0.0001f, 1);
                    max = Mathf.Clamp(EditorGUILayout.FloatField("Max", max), 0.001f, 1000);
                }
            }

            applyScale();
        }

        private void applyScale()
        {
            if (Application.isPlaying && Time.timeScale != scale)
            {
                Time.timeScale = scale;
            }
        }

        private float round(float _value, float _step)
        {
            if (_value >= max)
            {
                return max;
            }

            int _stepsCount = Mathf.RoundToInt(_value / _step);
            return (_step * _stepsCount).ClampMax(max);
        }

        [MenuItem("My Tools/Time Scaler", priority = 1)]
        public static void ShowWindow()
        {
            if (GetWindow(typeof(TimeScalerWindow)) is TimeScalerWindow _window)
            {
                _window.autoRepaintOnSceneChange = true;
                _window.titleContent = new GUIContent("Time Scaler");
                _window.minSize = new Vector2(MIN_WIDTH, HEIGHT);
                _window.maxSize = new Vector2(MAX_WIDTH, HEIGHT);
            }
        }
    }
}

namespace DL.Editor.FindAsset
{
    using UnityEditor;
    using UnityEngine;

    public class FindAssetUsingGUIDWindow : EditorWindow
    {
        public const int VALID_GUID_LENGTH = 32;
        public const float MIN_WIDTH = 300f;
        public const float MAX_WIDTH = 1000f;
        public const float HEIGHT = 45f;

        [SerializeField] private string assetGUID = string.Empty;
        [SerializeField] private Object foundObject = null;

        protected void OnGUI()
        {
            using (new LabelWidthScope(100f))
            {
                using (var _check = new ChangeCheckScope())
                {
                    using (new HorizontalScope())
                    {
                        assetGUID = EditorGUILayout.TextField("Asset GUID", assetGUID);

                        bool _clicked = GUILayout.Button("Find", GUILayout.Width(40f));

                        if ((_clicked || _check) && assetGUID.Length == VALID_GUID_LENGTH)
                        {
                            findAssetFromGUID();
                        }
                    }
                }

                using (ChangeCheckScope _check = new ChangeCheckScope())
                {
                    foundObject = EditorGUILayout.ObjectField("Found Asset", foundObject, typeof(Object), false);

                    if (_check)
                    {
                        assetGUID = foundObject != null ? foundObject.GetAssetGUID() : string.Empty;
                    }
                }
            }
        }

        private void findAssetFromGUID()
        {
            if (string.IsNullOrEmpty(assetGUID) == false && string.IsNullOrWhiteSpace(assetGUID) == false)
            {
                string _assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                foundObject = AssetDatabase.LoadAssetAtPath<Object>(_assetPath);
            }
        }

        [MenuItem("My Tools/Find Asset Using GUID", priority = 2)]
        public static void ShowWindow()
        {
            if (GetWindow(typeof(FindAssetUsingGUIDWindow)) is FindAssetUsingGUIDWindow _window)
            {
                _window.autoRepaintOnSceneChange = true;
                _window.titleContent = new GUIContent("Find Asset Using GUID");
                _window.minSize = new Vector2(MIN_WIDTH, HEIGHT);
                _window.maxSize = new Vector2(MAX_WIDTH, HEIGHT);
            }
        }
    }
}

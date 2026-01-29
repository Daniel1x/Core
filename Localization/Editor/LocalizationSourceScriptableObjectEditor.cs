namespace DL.Localization.Editor
{
    using DL.Localization;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(LocalizationSource))]
    public class LocalizationSourceScriptableObjectEditor : Editor
    {
        private static string previewKey = string.Empty;

        public override void OnInspectorGUI()
        {
            bool _valueChanged = DrawDefaultInspector();

            if (target is not LocalizationSource _source)
            {
                return;
            }

            if (_valueChanged || _source.Initialized == false)
            {
                _source.Initialize();
            }

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Localization Key Preview", EditorStyles.boldLabel);
            previewKey = EditorGUILayout.TextField("Key", previewKey);

            //Draw term preview with edition
            var _options = _source.Keys;
            int _currentIndex = System.Array.IndexOf(_options, previewKey);
            int _newIndex = EditorGUILayout.Popup("Key", _currentIndex, _options);

            if (_newIndex != _currentIndex && _newIndex >= 0 && _newIndex < _options.Length)
            {
                previewKey = _options[_newIndex];
            }

            if (_source.HasKey(previewKey, out LocalizationData _data))
            {
                string[] _languages = Languages.LanguageNames;

                for (int i = 0; i < _data.Localization.Length; i++)
                {
                    using (var _check = new EditorGUI.ChangeCheckScope())
                    {
                        string _newValue = EditorGUILayout.TextField(_languages[i], _data.Localization[i]);

                        if (_check.changed)
                        {
                            _data.SetLocalization(i, _newValue);
                            _source.SaveSource();
                        }
                    }
                }

                // Option to delete the key, with confirmation
                EditorGUILayout.Space(10f);

                if (GUILayout.Button("Delete Key") && EditorUtility.DisplayDialog("Confirm Deletion", $"Are you sure you want to delete the key '{previewKey}'?", "Delete", "Cancel"))
                {
                    _source.RemoveKey(previewKey);
                    _source.Initialize();
                    _source.SaveSource();
                    previewKey = string.Empty;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Key not found in localization source.", MessageType.Warning);

                if (string.IsNullOrEmpty(previewKey) == false && GUILayout.Button("Create New Key"))
                {
                    _source.CreateNewKey(previewKey);
                }
            }
        }
    }
}

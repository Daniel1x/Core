namespace DL.Localization.Editor
{
    using DL.Localization;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(LocTermAttribute))]
    public class LocTermAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Working only for string properties
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use LocTerm with string.");
                return;
            }

            //Draw default single line string field
            var _singleLineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(_singleLineRect, property, label);

            var _localizationSource = LocalizationEditorExtensions.GetSource();

            if (_localizationSource == null)
            {
                return;
            }

            //Draw popup for available keys
            var _spacing = EditorGUIUtility.standardVerticalSpacing;
            string[] _options = _localizationSource.Keys;

            if (attribute is LocTermAttribute _locTermAttribute
                && !string.IsNullOrEmpty(_locTermAttribute.Filter)
                && !string.IsNullOrWhiteSpace(_locTermAttribute.Filter))
            {
                _options = System.Array.FindAll(_options, k => k.Contains(_locTermAttribute.Filter, System.StringComparison.OrdinalIgnoreCase));
            }

            int _currentIndex = Mathf.Max(0, System.Array.IndexOf(_options, property.stringValue));

            using (var _changeCheckScope = new EditorGUI.ChangeCheckScope())
            {
                Rect _popupRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + _spacing, position.width, EditorGUIUtility.singleLineHeight);

                _currentIndex = EditorGUI.Popup(_popupRect, "Available Keys:", _currentIndex, _options);

                if (_changeCheckScope.changed)
                {
                    property.stringValue = _options[_currentIndex];
                    property.serializedObject.ApplyModifiedProperties();
                }
            }

            bool _hasKey = _localizationSource.HasKey(property.stringValue, out LocalizationData _data);

            //Draw foldout and then translations if expanded, allow editing them
            Rect _foldoutRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + _spacing) * 2, position.width, EditorGUIUtility.singleLineHeight);

            //Left half for foldout, right half for button
            var _foldoutHalfRect = new Rect(_foldoutRect.x, _foldoutRect.y, _foldoutRect.width / 2 - 2, _foldoutRect.height);
            var _buttonHalfRect = new Rect(_foldoutRect.x + _foldoutRect.width / 2 + 2, _foldoutRect.y, _foldoutRect.width / 2 - 2, _foldoutRect.height);

            //If key does not exist, show button to create it
            if (!_hasKey)
            {
                property.isExpanded = EditorGUI.Foldout(_foldoutHalfRect, property.isExpanded, "Translations", true);
                property.serializedObject.ApplyModifiedProperties();

                if (GUI.Button(_buttonHalfRect, "Create Key"))
                {
                    _localizationSource.CreateNewKey(property.stringValue);
                }
            }
            else //Key exists, show normal foldout with translations
            {
                property.isExpanded = EditorGUI.Foldout(_foldoutHalfRect, property.isExpanded, "Translations", true);

                if (GUI.Button(_buttonHalfRect, "Delete Key") && EditorUtility.DisplayDialog("Delete Key", "Are you sure you want to delete this key?", "Yes", "No"))
                {
                    _localizationSource.DeleteKey(property.stringValue);
                    return;
                }

                if (!property.isExpanded)
                {
                    return;
                }

                EditorGUI.indentLevel++;

                for (int i = 0; i < Languages.LanguageCount; i++)
                {
                    using (var _changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        Rect _langRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + _spacing) * (3 + i), position.width, EditorGUIUtility.singleLineHeight);

                        string _new = EditorGUI.TextField(_langRect, Languages.LanguageNames[i], _data.GetLocalization(i));

                        if (_changeCheckScope.changed)
                        {
                            _data.SetLocalization(i, _new);

                            _localizationSource.SetKey(property.stringValue, _data);
                            _localizationSource.SaveSource();
                        }
                    }
                }

                EditorGUI.indentLevel--;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            LocalizationSource _source = LocalizationEditorExtensions.GetSource();
            bool _isExpanded = property.isExpanded;
            float _spacing = EditorGUIUtility.standardVerticalSpacing;
            float _lineHeight = EditorGUIUtility.singleLineHeight;

            if (_isExpanded && _source != null && _source.HasKey(property.stringValue, out LocalizationData _data))
            {
                return (_lineHeight + _spacing) * (3 + Languages.LanguageCount) - _spacing;
            }
            else
            {
                return (_lineHeight + _spacing) * 3 - _spacing;
            }
        }
    }
}

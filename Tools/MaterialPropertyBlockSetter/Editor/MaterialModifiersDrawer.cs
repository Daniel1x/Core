namespace DL.MaterialPropertyBlockSetter
{
    using DL.Editor;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;

    public static class MaterialModifiersDrawer
    {
        public static void DrawArray<T1, T2>(MaterialPropertyBlockSetter _rendererOwner, SerializedProperty _cachedMaterial, SerializedProperty _overridedTypes, string _arrayPropertyName, List<T1> _list, ShaderPropertyType _type) where T1 : MaterialPropertyModifier<T2>
        {
            if (((ShaderPropertyMode)_overridedTypes.enumValueFlag).ContainsType(_type))
            {
                SerializedProperty _array = _cachedMaterial.FindPropertyRelative(_arrayPropertyName);
                _array.isExpanded = EditorGUILayout.Foldout(_array.isExpanded, _array.displayName, true);

                if (_array.isExpanded && _array.arraySize > 0)
                {
                    using (new IndentScope())
                    {
                        drawArray(_array, _rendererOwner, _type);
                    }
                }

                checkForDuplicatedProperties<T1, T2>(_list);
            }
        }

        private static void drawArray(SerializedProperty _array, MaterialPropertyBlockSetter _rendererOwner, ShaderPropertyType _type, bool _allowRemovingElement = false)
        {
            for (int i = 0; i < _array.arraySize; i++)
            {
                SerializedProperty _element = _array.GetArrayElementAtIndex(i);

                if (_element == null)
                {
                    continue;
                }

                EditorGUILayout.BeginHorizontal();

                _element.isExpanded = EditorGUILayout.Foldout(_element.isExpanded, _element.FindPropertyRelative("Property").stringValue);

                if (_allowRemovingElement && GUILayout.Button(new GUIContent("-", "Remove modifier!"), GUILayout.MaxWidth(20)))
                {
                    _array.DeleteArrayElementAtIndex(i);
                    i--;

                    EditorUtility.SetDirty(_rendererOwner);
                    EditorGUILayout.EndHorizontal();
                    continue;
                }

                EditorGUILayout.EndHorizontal();

                if (_element.isExpanded == false)
                {
                    continue;
                }

                using (new IndentScope())
                {
                    EditorGUILayout.PropertyField(_element.FindPropertyRelative("Value"));

                    if (_element.FindPropertyRelative("DefaultValueCached").boolValue)
                    {
                        using (new DisabledScope())
                        {
                            EditorGUILayout.PropertyField(_element.FindPropertyRelative("DefaultValue"));
                        }
                    }
                }
            }
        }

        private static void checkForDuplicatedProperties<T1, T2>(List<T1> _list) where T1 : MaterialPropertyModifier<T2>
        {
            if (_list.ModifiesTheSamePropertyMultipleTimes<T1, T2>(out List<string> _duplicatedTypes, out int _duplicatedTypesCount) == false)
            {
                return;
            }

            string _message = $"There are multiple modifiers that changes property of type: {_duplicatedTypes[0]}";

            for (int i = 1; i < _duplicatedTypesCount; i++)
            {
                _message += $", {_duplicatedTypes[i]}";
            }

            using (new GUIColorScope(Color.yellow))
            {
                EditorGUILayout.LabelField(_message + "!", EditorStyles.helpBox);
            }
        }
    }
}

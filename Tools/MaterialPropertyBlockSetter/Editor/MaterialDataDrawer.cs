namespace DL.MaterialPropertyBlockSetter
{
    using DL.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Rendering;

    public static class MaterialDataDrawer
    {
        private const float TOOLS_BUTTON_MIN_WIDTH = 50f;
        private const float TOOLS_BUTTON_MAX_WIDTH = 100f;
        private const float SETTINGS_LABEL_MAX_WIDTH = 150;

        private static ShaderPropertyType newType = ShaderPropertyType.Color;
        private static int newPropertyTypeID = 0;

        private static Color colorValue = Color.white;
        private static Vector4 vector4Value = Vector4.zero;
        private static float floatValue = 0f;
        private static float rangeValue = 0f;
        private static Texture textureValue = null;
        private static int intValue = 0;

        public static void Draw(SerializedProperty _cachedMaterialProperty, MaterialData _materialData, RendererData _materialOwner, MaterialPropertyBlockSetter _rendererOwner)
        {
            if (_rendererOwner == null || _materialOwner == null)
            {
                return;
            }

            EditorGUILayout.Space(2f);
            EditorDrawing.DrawEditorLine(Color.black);
            EditorGUILayout.Space(2f);

            SerializedProperty _materialIndexProperty = _cachedMaterialProperty.FindPropertyRelative("materialIndex");
            SerializedProperty _materialProperty = _cachedMaterialProperty.FindPropertyRelative("material");
            SerializedProperty _overridedTypesProperty = _cachedMaterialProperty.FindPropertyRelative("overridedTypes");

            markModifications(_cachedMaterialProperty, _materialData, _rendererOwner);

            using (new DisabledScope())
            {
                EditorGUILayout.PropertyField(_materialIndexProperty);
                EditorGUILayout.PropertyField(_materialProperty);
                EditorGUILayout.PropertyField(_overridedTypesProperty);
            }

            using (new DisabledScope(_materialData.EditMode))
            {
                MaterialModifiersDrawer.DrawArray<ColorProperty, Color>(_rendererOwner, _cachedMaterialProperty, _overridedTypesProperty, "colorProperties", _materialData.ColorProperties, ShaderPropertyType.Color);
                MaterialModifiersDrawer.DrawArray<Vector4Property, Vector4>(_rendererOwner, _cachedMaterialProperty, _overridedTypesProperty, "vector4Properties", _materialData.Vector4Properties, ShaderPropertyType.Vector);
                MaterialModifiersDrawer.DrawArray<FloatProperty, float>(_rendererOwner, _cachedMaterialProperty, _overridedTypesProperty, "floatProperties", _materialData.FloatProperties, ShaderPropertyType.Float);
                MaterialModifiersDrawer.DrawArray<RangeProperty, float>(_rendererOwner, _cachedMaterialProperty, _overridedTypesProperty, "rangeProperties", _materialData.RangeProperties, ShaderPropertyType.Range);
                MaterialModifiersDrawer.DrawArray<TextureProperty, Texture>(_rendererOwner, _cachedMaterialProperty, _overridedTypesProperty, "textureProperties", _materialData.TextureProperties, ShaderPropertyType.Texture);
                MaterialModifiersDrawer.DrawArray<IntProperty, int>(_rendererOwner, _cachedMaterialProperty, _overridedTypesProperty, "intProperties", _materialData.IntProperties, ShaderPropertyType.Int);
            }

            using (new IndentScope(-2))
            {
                drawEditModeButtonsAndUtilities(_cachedMaterialProperty, _materialData, _rendererOwner);
            }
        }

        private static void markModifications(SerializedProperty _cachedMaterialProperty, MaterialData _materialData, MaterialPropertyBlockSetter _rendererOwner, bool _forceSetDirty = false)
        {
            bool _updated = _materialData.UpdateOverridedTypes();
            _cachedMaterialProperty.ApplyModifiedProperties();

            if (_updated || _forceSetDirty)
            {
                EditorUtilities.SetDirty(_rendererOwner);
            }
        }

        private static void drawEditModeButtonsAndUtilities(SerializedProperty _cachedMaterialProperty, MaterialData _materialData, MaterialPropertyBlockSetter _rendererOwner)
        {
            if (_materialData.EditMode)
            {
                using (new GUIBackgroundColorScope(Color.green))
                {
                    drawEditModeButtons(_cachedMaterialProperty, _materialData, _rendererOwner);
                }
            }
            else
            {
                drawEditModeButtons(_cachedMaterialProperty, _materialData, _rendererOwner);
            }

            if (_materialData.EditMode == false
                || _rendererOwner.MaterialThatDisplaysEditorTools == null
                || _rendererOwner.MaterialThatDisplaysEditorTools != _materialData)
            {
                return;
            }

            EditorGUILayout.LabelField("Settings:", EditorStyles.boldLabel);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Shader Property Type", GUILayout.MaxWidth(SETTINGS_LABEL_MAX_WIDTH));
                newType = (ShaderPropertyType)EditorGUILayout.EnumPopup(newType);
            }

            if (_materialData.Material.GetPropertiesOfType(newType, out string[] _properties, out int _count))
            {
                if (newPropertyTypeID >= _count)
                {
                    newPropertyTypeID = 0;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Shader Property", GUILayout.MaxWidth(SETTINGS_LABEL_MAX_WIDTH));
                    newPropertyTypeID = EditorGUILayout.Popup(newPropertyTypeID, _properties);
                }
            }
            else
            {
                using (new GUIColorScope(Color.yellow))
                {
                    EditorGUILayout.LabelField("There is no property for the selected type!", EditorStyles.helpBox);
                }

                return;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Value", GUILayout.MaxWidth(SETTINGS_LABEL_MAX_WIDTH));
                drawVariableField();
            }

            drawUtilitiesButtons(_cachedMaterialProperty, _materialData, _rendererOwner);
        }

        private static void drawUtilitiesButtons(SerializedProperty _cachedMaterialProperty, MaterialData _materialData, MaterialPropertyBlockSetter _rendererOwner)
        {
            GUIStyle _buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            _buttonStyle.alignment = TextAnchor.MiddleCenter;

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Add To:", EditorDrawing.GetCenteredBoldLabelStyle());

                    if (GUILayout.Button(new GUIContent("Current Material",
                        "It will only add a modifier to one material above the buttons!"), _buttonStyle))
                    {
                        addModifier(_materialData, _cachedMaterialProperty);
                    }

                    if (GUILayout.Button(new GUIContent("All Materials",
                        "It will add a modifier to every material with the selected property!"), _buttonStyle))
                    {
                        _rendererOwner.PerformActionOnEveryCachedMaterial((_material) => addModifier(_material));
                    }
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    EditorGUILayout.LabelField("Remove From:", EditorDrawing.GetCenteredBoldLabelStyle());

                    if (GUILayout.Button(new GUIContent("Current Material",
                        "It will only remove a modifier from one material above the buttons! " +
                        "Only property names and their value type are used to remove modifiers!"), _buttonStyle))
                    {
                        removeModifier(_materialData, _cachedMaterialProperty);
                    }

                    if (GUILayout.Button(new GUIContent("All Materials",
                        "It will remove a modifier from every material with the selected property! " +
                        "Only property names and their value type are used to remove modifiers!"), _buttonStyle))
                    {
                        _rendererOwner.PerformActionOnEveryCachedMaterial((_material) => removeModifier(_material));
                    }
                }
            }
        }

        private static void drawEditModeButtons(SerializedProperty _cachedMaterialProperty, MaterialData _materialData, MaterialPropertyBlockSetter _rendererOwner)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Edit Mode"))
            {
                _materialData.EditMode = !_materialData.EditMode;

                if (_materialData.EditMode == false)
                {
                    if (_rendererOwner.MaterialThatDisplaysEditorTools == _materialData)
                    {
                        _rendererOwner.MaterialThatDisplaysEditorTools = null;
                    }

                    if (_materialData.RemoveUnassignedVariables())
                    {
                        markModifications(_cachedMaterialProperty, _materialData, _rendererOwner, true);
                    }
                }
            }

            if (GUILayout.Button("Tools", GUILayout.MinWidth(TOOLS_BUTTON_MIN_WIDTH), GUILayout.MaxWidth(TOOLS_BUTTON_MAX_WIDTH)))
            {
                if (_materialData.EditMode == false)
                {
                    _materialData.EditMode = true;
                }

                _rendererOwner.MaterialThatDisplaysEditorTools = _rendererOwner.MaterialThatDisplaysEditorTools == _materialData ? null : _materialData;
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void drawVariableField()
        {
            switch (newType)
            {
                case ShaderPropertyType.Color:
                    colorValue = EditorGUILayout.ColorField(colorValue);
                    break;
                case ShaderPropertyType.Vector:
                    vector4Value = EditorGUILayout.Vector4Field("Vector4Value", vector4Value);
                    break;
                case ShaderPropertyType.Float:
                    floatValue = EditorGUILayout.FloatField(floatValue);
                    break;
                case ShaderPropertyType.Range:
                    rangeValue = EditorGUILayout.FloatField(rangeValue);
                    break;
                case ShaderPropertyType.Texture:
                    Object _obj = EditorGUILayout.ObjectField(textureValue, typeof(Texture), false);
                    textureValue = _obj != null && _obj is Texture _texture ? _texture : null;
                    break;
                case ShaderPropertyType.Int:
                    intValue = EditorGUILayout.IntField(intValue);
                    break;
            }
        }

        private static void addModifier(MaterialData _materialData, SerializedProperty _cachedMaterialProperty = null)
        {
            if (_materialData.Material.GetNameOfPropertyOfType(newType, newPropertyTypeID, out string _newPropertyName) == false)
            {
                return;
            }

            switch (newType)
            {
                case ShaderPropertyType.Color:
                    _materialData.AddModifier<ColorProperty, Color>(new ColorProperty(_newPropertyName, colorValue));
                    break;
                case ShaderPropertyType.Vector:
                    _materialData.AddModifier<Vector4Property, Vector4>(new Vector4Property(_newPropertyName, vector4Value));
                    break;
                case ShaderPropertyType.Float:
                    _materialData.AddModifier<FloatProperty, float>(new FloatProperty(_newPropertyName, floatValue));
                    break;
                case ShaderPropertyType.Range:
                    _materialData.AddModifier<RangeProperty, float>(new RangeProperty(_newPropertyName, rangeValue));
                    break;
                case ShaderPropertyType.Texture:
                    _materialData.AddModifier<TextureProperty, Texture>(new TextureProperty(_newPropertyName, textureValue));
                    break;
                case ShaderPropertyType.Int:
                    _materialData.AddModifier<IntProperty, int>(new IntProperty(_newPropertyName, intValue));
                    break;
            }

            _materialData.UpdateOverridedTypes();

            if (_cachedMaterialProperty != null)
            {
                _cachedMaterialProperty.ApplyModifiedProperties();
            }
        }

        private static void removeModifier(MaterialData _materialData, SerializedProperty _cachedMaterialProperty = null)
        {
            if (_materialData.Material.GetNameOfPropertyOfType(newType, newPropertyTypeID, out string _newPropertyName) == false)
            {
                return;
            }

            switch (newType)
            {
                case ShaderPropertyType.Color:
                    _materialData.RemoveModifier<ColorProperty, Color>(new ColorProperty(_newPropertyName, colorValue));
                    break;
                case ShaderPropertyType.Vector:
                    _materialData.RemoveModifier<Vector4Property, Vector4>(new Vector4Property(_newPropertyName, vector4Value));
                    break;
                case ShaderPropertyType.Float:
                    _materialData.RemoveModifier<FloatProperty, float>(new FloatProperty(_newPropertyName, floatValue));
                    break;
                case ShaderPropertyType.Range:
                    _materialData.RemoveModifier<RangeProperty, float>(new RangeProperty(_newPropertyName, rangeValue));
                    break;
                case ShaderPropertyType.Texture:
                    _materialData.RemoveModifier<TextureProperty, Texture>(new TextureProperty(_newPropertyName, textureValue));
                    break;
                case ShaderPropertyType.Int:
                    _materialData.RemoveModifier<IntProperty, int>(new IntProperty(_newPropertyName, intValue));
                    break;
            }

            _materialData.UpdateOverridedTypes();

            if (_cachedMaterialProperty != null)
            {
                _cachedMaterialProperty.ApplyModifiedProperties();
            }
        }
    }
}

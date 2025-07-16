namespace DL.MaterialPropertyBlockSetter
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Serializable]
    public class MaterialData
    {
        [SerializeField] private bool editMode = false;

        /// <summary> Editor only! </summary>
        public bool EditMode
        {
            get => editMode;
            set
            {
                if (editMode != value)
                {
                    editMode = value;

                    if (value == false)
                    {
                        RemoveUnassignedVariables();
                    }
                }
            }
        }

        [SerializeField] private int materialIndex = -1;
        [SerializeField] private Material material = null;
        [SerializeField] private ShaderPropertyMode overridedTypes = 0;

        [SerializeField] private List<ColorProperty> colorProperties = new List<ColorProperty>();
        [SerializeField] private List<Vector4Property> vector4Properties = new List<Vector4Property>();
        [SerializeField] private List<FloatProperty> floatProperties = new List<FloatProperty>();
        [SerializeField] private List<RangeProperty> rangeProperties = new List<RangeProperty>();
        [SerializeField] private List<TextureProperty> textureProperties = new List<TextureProperty>();
        [SerializeField] private List<IntProperty> intProperties = new List<IntProperty>();

        private bool overrideModeInitialized = false;
        private MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();

        public ShaderPropertyMode OverridedTypes => overridedTypes;
        public Material Material => material;

        public List<ColorProperty> ColorProperties => colorProperties;
        public List<Vector4Property> Vector4Properties => vector4Properties;
        public List<FloatProperty> FloatProperties => floatProperties;
        public List<RangeProperty> RangeProperties => rangeProperties;
        public List<TextureProperty> TextureProperties => textureProperties;
        public List<IntProperty> IntProperties => intProperties;

        public MaterialData(Renderer _renderer, int _index, Material _material)
        {
            materialIndex = _index;
            material = _material;

            GetMaterialPropertyBlock(_renderer);
        }

        public void ApplyModifiersAndSetBlock(Renderer _renderer, bool _createNewPropertyBlock = false)
        {
            if (_renderer == null)
            {
                return;
            }

            GetMaterialPropertyBlock(_renderer, _createNewPropertyBlock);
            ApplyModifiers(materialPropertyBlock);
            SetMaterialPropertyBlock(_renderer);
        }

        public void ApplyModifiers(MaterialPropertyBlock _targetBlock)
        {
            if (overrideModeInitialized == false)
            {
                overrideModeInitialized = true;
                UpdateOverridedTypes();
            }

            _applyModifiersIfModeIsEnabled<ColorProperty, Color>(colorProperties, ShaderPropertyType.Color);
            _applyModifiersIfModeIsEnabled<Vector4Property, Vector4>(vector4Properties, ShaderPropertyType.Vector);
            _applyModifiersIfModeIsEnabled<FloatProperty, float>(floatProperties, ShaderPropertyType.Float);
            _applyModifiersIfModeIsEnabled<RangeProperty, float>(rangeProperties, ShaderPropertyType.Range);
            _applyModifiersIfModeIsEnabled<TextureProperty, Texture>(textureProperties, ShaderPropertyType.Texture);
            _applyModifiersIfModeIsEnabled<IntProperty, int>(intProperties, ShaderPropertyType.Int);

            void _applyModifiersIfModeIsEnabled<T1, T2>(List<T1> _list, ShaderPropertyType _type) where T1 : MaterialPropertyModifier<T2>
            {
                if (overridedTypes.ContainsType(_type))
                {
                    _list.ApplyModifiersToBlock<T1, T2>(material, _targetBlock);
                }
            }
        }

        public void ClearAllModifiers()
        {
            colorProperties.Clear();
            vector4Properties.Clear();
            floatProperties.Clear();
            rangeProperties.Clear();
            textureProperties.Clear();
            intProperties.Clear();
        }

        public void GetMaterialPropertyBlock(Renderer _renderer, bool _createNewPropertyBlock = false)
        {
            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();

                if (_createNewPropertyBlock)
                {
                    return;
                }
            }

            if (_createNewPropertyBlock)
            {
                materialPropertyBlock.Clear();
            }
            else
            {
                _renderer.GetPropertyBlock(materialPropertyBlock, materialIndex);
            }
        }

        public void SetMaterialPropertyBlock(Renderer _renderer)
        {
            _renderer.SetPropertyBlock(materialPropertyBlock, materialIndex);
        }

        public void AddModifierAndApplyChanges<T1, T2>(T1 _modifier, Renderer _renderer, bool _createNewPropertyBlock = false) where T1 : MaterialPropertyModifier<T2>
        {
            AddModifier<T1, T2>(_modifier);
            ApplyModifiersAndSetBlock(_renderer, _createNewPropertyBlock);
        }

        public bool AddModifier<T1, T2>(T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (_modifier == null)
            {
                return false;
            }

            return _tryToAddModifier<ColorProperty, Color>(ref colorProperties)
                || _tryToAddModifier<Vector4Property, Vector4>(ref vector4Properties)
                || _tryToAddModifier<FloatProperty, float>(ref floatProperties)
                || _tryToAddModifier<RangeProperty, float>(ref rangeProperties)
                || _tryToAddModifier<TextureProperty, Texture>(ref textureProperties)
                || _tryToAddModifier<IntProperty, int>(ref intProperties);

            bool _tryToAddModifier<T3, T4>(ref List<T3> _list) where T3 : MaterialPropertyModifier<T4>
            {
                if (_modifier is T3 _modifierOfType)
                {
                    _modifierOfType.CacheDefaultValue(material, true);
                    _list.InsertModifier<T3, T4>(_modifierOfType);
                    updateSingleMode<T3, T4>(_list, _modifierOfType.GetShaderPropertyType());

                    return true;
                }

                return false;
            }
        }

        public void RemoveModifierAndApplyChanges<T1, T2>(T1 _modifier, Renderer _renderer, bool _createNewPropertyBlock = false) where T1 : MaterialPropertyModifier<T2>
        {
            RemoveModifier<T1, T2>(_modifier);
            ApplyModifiersAndSetBlock(_renderer, _createNewPropertyBlock);
        }

        public bool RemoveModifier<T1, T2>(T1 _modifier) where T1 : MaterialPropertyModifier<T2>
        {
            if (_modifier == null)
            {
                return false;
            }

            return _tryToRemoveModifier<ColorProperty, Color>(ref colorProperties)
                || _tryToRemoveModifier<Vector4Property, Vector4>(ref vector4Properties)
                || _tryToRemoveModifier<FloatProperty, float>(ref floatProperties)
                || _tryToRemoveModifier<RangeProperty, float>(ref rangeProperties)
                || _tryToRemoveModifier<TextureProperty, Texture>(ref textureProperties)
                || _tryToRemoveModifier<IntProperty, int>(ref intProperties);

            bool _tryToRemoveModifier<T3, T4>(ref List<T3> _list) where T3 : MaterialPropertyModifier<T4>
            {
                if (_modifier is T3 _modifierOfType)
                {
                    bool _removed = _list.TryToRemoveModifier<T3, T4>(_modifierOfType, out T3 _removedModifier);

                    if (_removed)
                    {
                        if (_removedModifier.ResetToDefault())
                        {
                            _list.InsertModifier<T3, T4>(_removedModifier);
                        }

                        updateSingleMode<T3, T4>(_list, _modifierOfType.GetShaderPropertyType());
                    }

                    return _removed;
                }

                return false;
            }
        }

        public void EnableKeyword(ShaderKeyword _keyword)
        {
            EnableKeyword(_keyword.Value);
        }

        public void EnableKeyword(string _keyword)
        {
            if (material.HasProperty(_keyword))
            {
                material.EnableKeyword(_keyword);
            }
        }

        public void DisableKeyword(ShaderKeyword _keyword)
        {
            DisableKeyword(_keyword.Value);
        }

        public void DisableKeyword(string _keyword)
        {
            if (material.HasProperty(_keyword))
            {
                material.DisableKeyword(_keyword);
            }
        }

        public bool UpdateOverridedTypes()
        {
            ShaderPropertyMode _currentMode = overridedTypes;

            updateSingleMode<ColorProperty, Color>(colorProperties, ShaderPropertyType.Color);
            updateSingleMode<Vector4Property, Vector4>(vector4Properties, ShaderPropertyType.Vector);
            updateSingleMode<FloatProperty, float>(floatProperties, ShaderPropertyType.Float);
            updateSingleMode<RangeProperty, float>(rangeProperties, ShaderPropertyType.Range);
            updateSingleMode<TextureProperty, Texture>(textureProperties, ShaderPropertyType.Texture);
            updateSingleMode<IntProperty, int>(intProperties, ShaderPropertyType.Int);

            return overridedTypes != _currentMode;
        }

        public bool RemoveUnassignedVariables()
        {
            bool _anyFieldModified = false;

            _removeUnassignedVariables<ColorProperty, Color>(colorProperties);
            _removeUnassignedVariables<Vector4Property, Vector4>(vector4Properties);
            _removeUnassignedVariables<FloatProperty, float>(floatProperties);
            _removeUnassignedVariables<RangeProperty, float>(rangeProperties);
            _removeUnassignedVariables<TextureProperty, Texture>(textureProperties);
            _removeUnassignedVariables<IntProperty, int>(intProperties);

            void _removeUnassignedVariables<T1, T2>(List<T1> _list) where T1 : MaterialPropertyModifier<T2>
            {
                if (_list == null)
                {
                    return;
                }

                int _count = _list.Count;

                if (_count <= 0)
                {
                    return;
                }

                bool _received = material.GetPropertiesOfType(_list[0].GetShaderPropertyType(), out string[] _properties, out int _);

                for (int i = _count - 1; i >= 0; i--)
                {
                    T1 _modifier = _list[i];

                    if (_modifier == null || (_received && _properties.Contains(_modifier.Property) == false))
                    {
                        _list.RemoveAt(i);
                        _anyFieldModified = true;
                    }
                }
            }

            if (_anyFieldModified)
            {
                UpdateOverridedTypes();
            }

            return _anyFieldModified;
        }

        private void updateSingleMode<T1, T2>(List<T1> _list, ShaderPropertyType _type) where T1 : MaterialPropertyModifier<T2>
        {
            overridedTypes = _list.Count > 0
                ? overridedTypes.EnableType(_type)
                : overridedTypes.DisableType(_type);
        }
    }
}

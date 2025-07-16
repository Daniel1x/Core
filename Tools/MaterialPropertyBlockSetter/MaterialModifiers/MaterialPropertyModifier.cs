namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Serializable]
    public abstract class MaterialPropertyModifier<T>
    {
        public string Property = default;
        public T Value = default;

        public bool DefaultValueCached = false;
        public T DefaultValue = default;

        [System.NonSerialized] protected bool shaderPropertyIDCached = false;
        [System.NonSerialized] protected int shaderPropertyID = -1;

        public MaterialPropertyModifier(string _type, T _value)
        {
            Property = _type;
            Value = _value;
        }

        public void ApplyModifier(Material _material, MaterialPropertyBlock _targetBlock)
        {
            CacheShaderPropertyID();
            CacheDefaultValue(_material);
            applyModifier(_targetBlock, shaderPropertyID);
        }

        public void CacheShaderPropertyID(bool _forceUpdate = false)
        {
            if (shaderPropertyIDCached == false || _forceUpdate)
            {
                shaderPropertyID = Shader.PropertyToID(Property);
                shaderPropertyIDCached = true;
            }
        }

        public void CacheDefaultValue(Material _material, bool _forceUpdate = false)
        {
            if (DefaultValueCached && _forceUpdate == false)
            {
                return;
            }

            if (getDefaultValue(_material, out T _default))
            {
                DefaultValue = _default;
                DefaultValueCached = true;
            }
        }

        public void OverrideDefaultValue<T1>(T1 _modifier) where T1 : MaterialPropertyModifier<T>
        {
            if (_modifier != null && _modifier.DefaultValueCached)
            {
                DefaultValue = _modifier.DefaultValue;
                DefaultValueCached = true;
            }
        }

        public bool ResetToDefault()
        {
            if (DefaultValueCached)
            {
                Value = DefaultValue;
                return true;
            }

            return false;
        }

        public abstract ShaderPropertyType GetShaderPropertyType();

        protected abstract void applyModifier(MaterialPropertyBlock _targetBlock, int _shaderPropertyID);
        protected abstract bool getDefaultValue(Material _material, out T _defaultValue);
    }
}

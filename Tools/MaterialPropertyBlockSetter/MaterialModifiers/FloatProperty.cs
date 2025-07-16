namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Serializable]
    public class FloatProperty : MaterialPropertyModifier<float>
    {
        public FloatProperty(string _type, float _value) : base(_type, _value) { }

        public override ShaderPropertyType GetShaderPropertyType()
        {
            return ShaderPropertyType.Float;
        }

        protected override void applyModifier(MaterialPropertyBlock _targetBlock, int _shaderPropertyID)
        {
            _targetBlock.SetFloat(_shaderPropertyID, Value);
        }

        protected override bool getDefaultValue(Material _material, out float _defaultValue)
        {
            if (_material != null && _material.HasFloat(Property))
            {
                _defaultValue = _material.GetFloat(Property);
                return true;
            }

            _defaultValue = default;
            return false;
        }
    }
}

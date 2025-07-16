namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Serializable]
    public class IntProperty : MaterialPropertyModifier<int>
    {
        public IntProperty(string _type, int _value) : base(_type, _value) { }

        public override ShaderPropertyType GetShaderPropertyType()
        {
            return ShaderPropertyType.Int;
        }

        protected override void applyModifier(MaterialPropertyBlock _targetBlock, int _shaderPropertyID)
        {
            _targetBlock.SetInt(_shaderPropertyID, Value);
        }

        protected override bool getDefaultValue(Material _material, out int _defaultValue)
        {
            if (_material != null && _material.HasInteger(Property))
            {
                _defaultValue = _material.GetInt(Property);
                return true;
            }

            _defaultValue = default;
            return false;
        }
    }
}

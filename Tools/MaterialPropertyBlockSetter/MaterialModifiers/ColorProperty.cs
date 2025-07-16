namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Serializable]
    public class ColorProperty : MaterialPropertyModifier<Color>
    {
        public ColorProperty(string _type, Color _value) : base(_type, _value) { }

        public override ShaderPropertyType GetShaderPropertyType()
        {
            return ShaderPropertyType.Color;
        }

        protected override void applyModifier(MaterialPropertyBlock _targetBlock, int _shaderPropertyID)
        {
            _targetBlock.SetColor(_shaderPropertyID, Value);
        }

        protected override bool getDefaultValue(Material _material, out Color _defaultValue)
        {
            if (_material != null && _material.HasColor(Property))
            {
                _defaultValue = _material.GetColor(Property);
                return true;
            }

            _defaultValue = default;
            return false;
        }
    }
}

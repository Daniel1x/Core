namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Serializable]
    public class TextureProperty : MaterialPropertyModifier<Texture>
    {
        public TextureProperty(string _type, Texture _value) : base(_type, _value) { }

        public override ShaderPropertyType GetShaderPropertyType()
        {
            return ShaderPropertyType.Texture;
        }

        protected override void applyModifier(MaterialPropertyBlock _targetBlock, int _shaderPropertyID)
        {
            _targetBlock.SetTexture(_shaderPropertyID, Value);
        }

        protected override bool getDefaultValue(Material _material, out Texture _defaultValue)
        {
            if (_material != null && _material.HasTexture(Property))
            {
                _defaultValue = _material.GetTexture(Property);
                return true;
            }

            _defaultValue = default;
            return false;
        }
    }
}

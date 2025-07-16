namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;
    using UnityEngine.Rendering;

    [System.Serializable]
    public class Vector4Property : MaterialPropertyModifier<Vector4>
    {
        public Vector4Property(string _type, Vector4 _value) : base(_type, _value) { }

        public override ShaderPropertyType GetShaderPropertyType()
        {
            return ShaderPropertyType.Vector;
        }

        protected override void applyModifier(MaterialPropertyBlock _targetBlock, int _shaderPropertyID)
        {
            _targetBlock.SetVector(_shaderPropertyID, Value);
        }

        protected override bool getDefaultValue(Material _material, out Vector4 _defaultValue)
        {
            if (_material != null && _material.HasVector(Property))
            {
                _defaultValue = _material.GetVector(Property);
                return true;
            }

            _defaultValue = default;
            return false;
        }
    }
}

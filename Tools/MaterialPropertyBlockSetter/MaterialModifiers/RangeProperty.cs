namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine.Rendering;

    [System.Serializable]
    public class RangeProperty : FloatProperty
    {
        public RangeProperty(string _type, float _value) : base(_type, _value) { }

        public override ShaderPropertyType GetShaderPropertyType()
        {
            return ShaderPropertyType.Range;
        }
    }
}

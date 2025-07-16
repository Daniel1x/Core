namespace DL.MaterialPropertyBlockSetter
{
    public interface ICustomShaderVariable
    {
        public UnityEngine.Rendering.ShaderPropertyType PropertyType { get; }
        public string ShaderPropertyName { get; }
        public ShaderProperty ShaderProperty { get; }
    }
}

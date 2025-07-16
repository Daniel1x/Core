namespace DL.MaterialPropertyBlockSetter
{
    using DL.Scriptables;
    using UnityEngine;
    using UnityEngine.Rendering;

    [CreateAssetMenu(fileName = "shaderProperty_New", menuName = CoreData.CREATION_TAB_NAME + "/Material Property Block Setter/Shader Property")]
    public class ShaderPropertyTypeVariable : ScriptableData<ShaderProperty>, ICustomShaderVariable
    {
        public ShaderPropertyType PropertyType => Value.Type;
        public string ShaderPropertyName => Value.Name;
        public ShaderProperty ShaderProperty => Value;
    }
}

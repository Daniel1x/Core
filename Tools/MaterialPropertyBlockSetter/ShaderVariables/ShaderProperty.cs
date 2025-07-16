namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine.Rendering;

    [System.Serializable]
    public struct ShaderProperty
    {
        public ShaderPropertyType Type;
        public string Name;

        public ShaderProperty(ShaderPropertyType _type, string _name)
        {
            Type = _type;
            Name = _name;
        }
    }
}

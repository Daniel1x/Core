using UnityEngine;

public class Vector4ShaderGlobalPropertySetter : ShaderGlobalPropertySetter<Vector4>
{
    protected override void setShaderGlobalProperty(string _name, Vector4 _value) => Shader.SetGlobalVector(_name, _value);
}

using UnityEngine;

public class Vector3ShaderGlobalPropertySetter : ShaderGlobalPropertySetter<Vector3>
{
    protected override void setShaderGlobalProperty(string _name, Vector3 _value) => Shader.SetGlobalVector(_name, _value);
}

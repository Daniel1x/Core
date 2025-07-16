using UnityEngine;

public class Vector2ShaderGlobalPropertySetter : ShaderGlobalPropertySetter<Vector2>
{
    protected override void setShaderGlobalProperty(string _name, Vector2 _value) => Shader.SetGlobalVector(_name, _value);
}

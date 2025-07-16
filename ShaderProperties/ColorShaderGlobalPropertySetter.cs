using UnityEngine;

public class ColorShaderGlobalPropertySetter : ShaderGlobalPropertySetter<Color>
{
    protected override void setShaderGlobalProperty(string _name, Color _value) => Shader.SetGlobalColor(_name, _value);
}

using UnityEngine;

public class FloatShaderGlobalPropertySetter : ShaderGlobalPropertySetter<float>
{
    protected override void setShaderGlobalProperty(string _name, float _value) => Shader.SetGlobalFloat(_name, _value);
}

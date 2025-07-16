using UnityEngine;

public class BoolShaderGlobalPropertySetter : ShaderGlobalPropertySetter<bool>
{
    protected override void setShaderGlobalProperty(string _name, bool _value) => Shader.SetGlobalFloat(_name, _value ? 1.0f : 0.0f);
}

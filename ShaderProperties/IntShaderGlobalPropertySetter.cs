using UnityEngine;

public class IntShaderGlobalPropertySetter : ShaderGlobalPropertySetter<int>
{
    protected override void setShaderGlobalProperty(string _name, int _value) => Shader.SetGlobalInteger(_name, _value);
}

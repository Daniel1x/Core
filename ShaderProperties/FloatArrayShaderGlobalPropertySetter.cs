using UnityEngine;

public class FloatArrayShaderGlobalPropertySetter : ShaderGlobalPropertySetter<float[]>
{
    protected override void setShaderGlobalProperty(string _name, float[] _value) => Shader.SetGlobalFloatArray(_name, _value);
}

using UnityEngine;

public class RenderTextureShaderGlobalPropertySetter : ShaderGlobalPropertySetter<RenderTexture>
{
    protected override void setShaderGlobalProperty(string _name, RenderTexture _value) => Shader.SetGlobalTexture(_name, _value);
}

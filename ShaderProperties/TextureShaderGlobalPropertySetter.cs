using UnityEngine;

public class TextureShaderGlobalPropertySetter : ShaderGlobalPropertySetter<Texture>
{
    protected override void setShaderGlobalProperty(string _name, Texture _value) => Shader.SetGlobalTexture(_name, _value);
}

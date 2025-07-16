using UnityEngine;

public class Texture2DShaderGlobalPropertySetter : ShaderGlobalPropertySetter<Texture2D>
{
    protected override void setShaderGlobalProperty(string _name, Texture2D _value) => Shader.SetGlobalTexture(_name, _value);
}

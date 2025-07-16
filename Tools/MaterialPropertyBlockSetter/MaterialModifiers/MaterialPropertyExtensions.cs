namespace DL.MaterialPropertyBlockSetter
{
    using UnityEngine;

    public static class MaterialPropertyExtensions
    {
        public static ColorProperty CreateColorProperty(this Color _color, string _propertyName) => new ColorProperty(_propertyName, _color);
        public static FloatProperty CreateFloatProperty(this float _value, string _propertyName) => new FloatProperty(_propertyName, _value);
        public static RangeProperty CreateRangeProperty(this float _value, string _propertyName) => new RangeProperty(_propertyName, _value);
        public static IntProperty CreateIntProperty(this int _value, string _propertyName) => new IntProperty(_propertyName, _value);
        public static TextureProperty CreateTextureProperty(this Texture _texture, string _propertyName) => new TextureProperty(_propertyName, _texture);
        public static Vector4Property CreateVector4Property(this Vector4 _vector, string _propertyName) => new Vector4Property(_propertyName, _vector);
    }
}

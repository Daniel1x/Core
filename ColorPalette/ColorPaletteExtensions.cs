namespace DL.ColorPalette
{
    using DL.ColorPalette.Structs;
    using UnityEngine;

    public static class ColorPaletteExtensions
    {
        public static ColorPair CreateComplementaryColors(this Color _color, float _saturation01, float _brightness01)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;
            Color _color1 = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 0.5f) % 1f, _saturation, _value);
            return new(_color1, _color2);
        }

        public static ColorTriplet CreateAnalogousColors(this Color _color, float _saturation01, float _brightness01, float _angleDegrees = 30f)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);

            _saturation = _saturation01;
            _value = _brightness01;

            float _angle01 = _angleDegrees / 360f;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + _angle01) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue - _angle01 + 1f) % 1f, _saturation, _value);

            return new(_color1, _color2, _baseColor);
        }

        public static ColorTriplet CreateTriadicColors(this Color _color, float _saturation01, float _brightness01)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;

            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 1f / 3f) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 2f / 3f) % 1f, _saturation, _value);

            return new(_color1, _color2, _baseColor);
        }

        public static ColorTriplet CreateSplitComplementaryColors(this Color _color, float _saturation01, float _brightness01, float _angleDegrees = 30f)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;

            float _angle01 = _angleDegrees / 360f;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 0.5f + _angle01) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 0.5f - _angle01 + 1f) % 1f, _saturation, _value);

            return new(_color1, _color2, _baseColor);
        }

        public static ColorQuartet CreateTetradicColors(this Color _color, float _saturation01, float _brightness01, float _angleDegrees = 60f)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;

            float _angle01 = _angleDegrees / 360f;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 0.5f) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + _angle01) % 1f, _saturation, _value);
            Color _color3 = Color.HSVToRGB((_hue + 0.5f + _angle01) % 1f, _saturation, _value);

            return new(_color1, _color2, _color3, _baseColor);
        }

        public static ColorQuartet CreateSquareColors(this Color _color, float _saturation01, float _brightness01)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;

            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 0.25f) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 0.5f) % 1f, _saturation, _value);
            Color _color3 = Color.HSVToRGB((_hue + 0.75f) % 1f, _saturation, _value);

            return new(_color1, _color2, _color3, _baseColor);
        }

        public static ColorQuartet CreateAbstractColors(this Color _color, float _saturation01, float _brightness01, float _angleDegrees = 20f)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;

            float _angle01 = _angleDegrees / 360f;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + _angle01) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 0.5f) % 1f, _saturation, _value);
            Color _color3 = Color.HSVToRGB((_hue + 0.5f + _angle01) % 1f, _saturation, _value);

            return new(_color1, _color2, _color3, _baseColor);
        }

        public static ColorQuintet CreatePentadicColors(this Color _color, float _saturation01, float _brightness01)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 0.2f) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 0.4f) % 1f, _saturation, _value);
            Color _color3 = Color.HSVToRGB((_hue + 0.6f) % 1f, _saturation, _value);
            Color _color4 = Color.HSVToRGB((_hue + 0.8f) % 1f, _saturation, _value);
            return new(_color1, _color2, _color3, _color4, _baseColor);
        }

        public static ColorQuintet CreatePentadicColors(this Color _colorPair, float _saturation01, float _brightness01, float _angleDegrees)
        {
            Color.RGBToHSV(_colorPair, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;
            float _angle01 = _angleDegrees / 360f;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 0.2f + _angle01) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 0.4f + _angle01) % 1f, _saturation, _value);
            Color _color3 = Color.HSVToRGB((_hue + 0.6f + _angle01) % 1f, _saturation, _value);
            Color _color4 = Color.HSVToRGB((_hue + 0.8f + _angle01) % 1f, _saturation, _value);
            return new(_color1, _color2, _color3, _color4, _baseColor);
        }

        public static ColorSextet CreateHexadicColors(this Color _color, float _saturation01, float _brightness01)
        {
            Color.RGBToHSV(_color, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 1f / 6f) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 2f / 6f) % 1f, _saturation, _value);
            Color _color3 = Color.HSVToRGB((_hue + 3f / 6f) % 1f, _saturation, _value);
            Color _color4 = Color.HSVToRGB((_hue + 4f / 6f) % 1f, _saturation, _value);
            Color _color5 = Color.HSVToRGB((_hue + 5f / 6f) % 1f, _saturation, _value);
            return new(_color1, _color2, _color3, _color4, _color5, _baseColor);
        }

        public static ColorSextet CreateHexadicColors(this Color _colorPair, float _saturation01, float _brightness01, float _angleDegrees)
        {
            Color.RGBToHSV(_colorPair, out float _hue, out float _saturation, out float _value);
            _saturation = _saturation01;
            _value = _brightness01;
            float _angle01 = _angleDegrees / 360f;
            Color _baseColor = Color.HSVToRGB(_hue, _saturation, _value);
            Color _color1 = Color.HSVToRGB((_hue + 1f / 6f + _angle01) % 1f, _saturation, _value);
            Color _color2 = Color.HSVToRGB((_hue + 2f / 6f + _angle01) % 1f, _saturation, _value);
            Color _color3 = Color.HSVToRGB((_hue + 3f / 6f + _angle01) % 1f, _saturation, _value);
            Color _color4 = Color.HSVToRGB((_hue + 4f / 6f + _angle01) % 1f, _saturation, _value);
            Color _color5 = Color.HSVToRGB((_hue + 5f / 6f + _angle01) % 1f, _saturation, _value);
            return new(_color1, _color2, _color3, _color4, _color5, _baseColor);
        }
    }
}

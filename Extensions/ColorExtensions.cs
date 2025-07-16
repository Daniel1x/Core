using DL.Structs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class ColorExtensions
{
    public static Color WithRed(this Color _color, float _red)
    {
        _color.r = _red;
        return _color;
    }

    public static Color WithGreen(this Color _color, float _green)
    {
        _color.g = _green;
        return _color;
    }

    public static Color WithBlue(this Color _color, float _blue)
    {
        _color.b = _blue;
        return _color;
    }

    public static Color WithAlpha(this Color _color, float _alpha)
    {
        _color.a = _alpha;
        return _color;
    }

    public static Color GetNormalizedGray(float _normalizedValue, float _alpha = 1f)
    {
        return new Color(_normalizedValue, _normalizedValue, _normalizedValue, _alpha);
    }

    public static Color Transparent(this Color _color)
    {
        _color.a = 0f;
        return _color;
    }

    public static Color Opaque(this Color _color)
    {
        _color.a = 1f;
        return _color;
    }

    public static Color LerpAlpha(this Color _color, float _from, float _to, float _progress)
    {
        return _progress <= 0f
                ? _color.WithAlpha(_from)
                : _progress >= 1f
                   ? _color.WithAlpha(_to)
                   : _color.WithAlpha(_from + ((_to - _from) * _progress));
    }

    public static ColorBlock WithNormalColor(this ColorBlock _colorBlock, Color _newNormalColor)
    {
        _colorBlock.normalColor = _newNormalColor;
        return _colorBlock;
    }

    public static ColorBlock WithHighlightedColor(this ColorBlock _colorBlock, Color _newHighlightedColor)
    {
        _colorBlock.highlightedColor = _newHighlightedColor;
        return _colorBlock;
    }

    public static ColorBlock WithPressedColor(this ColorBlock _colorBlock, Color _newPressedColor)
    {
        _colorBlock.pressedColor = _newPressedColor;
        return _colorBlock;
    }

    public static ColorBlock WithSelectedColor(this ColorBlock _colorBlock, Color _newSelectedColor)
    {
        _colorBlock.selectedColor = _newSelectedColor;
        return _colorBlock;
    }

    public static ColorBlock WithDisabledColor(this ColorBlock _colorBlock, Color _newDisabledColor)
    {
        _colorBlock.disabledColor = _newDisabledColor;
        return _colorBlock;
    }

    /// <param name="_newColorMultiplier">Value clamped to <1, 5>.</param>
    public static ColorBlock WithColorMultiplier(this ColorBlock _colorBlock, float _newColorMultiplier)
    {
        _colorBlock.colorMultiplier = Mathf.Clamp(_newColorMultiplier, 1f, 5f);
        return _colorBlock;
    }

    /// <param name="_newFadeDuration">Value should not be negative!</param>
    public static ColorBlock WithFadeDuration(this ColorBlock _colorBlock, float _newFadeDuration)
    {
        _colorBlock.fadeDuration = _newFadeDuration < 0f ? 0f : _newFadeDuration;
        return _colorBlock;
    }

    public static Color Blend(this IEnumerable<Color> _collection)
    {
        int _count = _collection.Count();

        if (_count <= 0)
        {
            return new Color();
        }

        if (_count == 1)
        {
            return _collection.ElementAt(0);
        }

        Color _newColor = new Color();

        for (int i = 0; i < _count; i++)
        {
            _newColor += _collection.ElementAt(i);
        }

        return _newColor / _count;
    }

    public static Color Blend(this IEnumerable<Color> _collection, float _newAlpha)
    {
        return _collection.Blend().WithAlpha(_newAlpha);
    }

    public static Color BlendWithExceptions(this IEnumerable<Color> _collection, params Color[] _excludedColors)
    {
        int _count = _collection.Count();

        if (_count <= 0)
        {
            return new Color();
        }

        if (_count == 1)
        {
            return _collection.ElementAt(0);
        }

        Color _newColor = new Color();
        int _colors = 0;

        for (int i = 0; i < _count; i++)
        {
            Color _element = _collection.ElementAt(i);

            if (_excludedColors.Contains(_element) == false)
            {
                _newColor += _element;
                _colors++;
            }
        }

        if (_colors > 1)
        {
            return _newColor / _colors;
        }

        return _newColor;
    }

    public static Color BlendWithExceptions(this IEnumerable<Color> _collection, Color _default, float _newAlpha, params Color[] _excludedColors)
    {
        Color _color = _collection.BlendWithExceptions(_excludedColors);

        if (_color == default)
        {
            return _default.WithAlpha(_newAlpha);
        }

        return _color.WithAlpha(_newAlpha);
    }

    public static Color RandomColor(float _alpha = 1f)
    {
        return new Color
        {
            r = UnityEngine.Random.Range(0f, 1f),
            g = UnityEngine.Random.Range(0f, 1f),
            b = UnityEngine.Random.Range(0f, 1f),
            a = _alpha
        };
    }

    public static Color RandomColorAndAlpha()
    {
        return new Color
        {
            r = UnityEngine.Random.Range(0f, 1f),
            g = UnityEngine.Random.Range(0f, 1f),
            b = UnityEngine.Random.Range(0f, 1f),
            a = UnityEngine.Random.Range(0f, 1f)
        };
    }

    public static Color RandomColor(MinMax _hueRange)
    {
        return Random.ColorHSV(_hueRange.Min, _hueRange.Max);
    }

    public static Color RandomColor(MinMax _hueRange, MinMax _saturationRange)
    {
        return Random.ColorHSV(_hueRange.Min, _hueRange.Max, _saturationRange.Min, _saturationRange.Max);
    }

    public static Color RandomColor(MinMax _hueRange, MinMax _saturationRange, MinMax _valueRange)
    {
        return Random.ColorHSV(_hueRange.Min, _hueRange.Max, _saturationRange.Min, _saturationRange.Max, _valueRange.Min, _valueRange.Max);
    }

    public static Color RandomColor(MinMax _hueRange, MinMax _saturationRange, MinMax _valueRange, MinMax _alphaRange)
    {
        return Random.ColorHSV(_hueRange.Min, _hueRange.Max, _saturationRange.Min, _saturationRange.Max, _valueRange.Min, _valueRange.Max, _alphaRange.Min, _alphaRange.Max);
    }

    public static void SetColorRed(this Graphic _graphic, float _red)
    {
        _graphic.color = _graphic.color.WithRed(_red);
    }

    public static void SetColorGreen(this Graphic _graphic, float _green)
    {
        _graphic.color = _graphic.color.WithGreen(_green);
    }

    public static void SetColorBlue(this Graphic _graphic, float _blue)
    {
        _graphic.color = _graphic.color.WithBlue(_blue);
    }

    public static void SetColorAlpha(this Graphic _graphic, float _alpha)
    {
        _graphic.color = _graphic.color.WithAlpha(_alpha);
    }

    public static float GetLuminance(this Color _color)
    {
        return (0.2126f * _color.r) + (0.7152f * _color.g) + (0.0722f * _color.b);
    }

    public static float GetColorAbsDelta(this Color _color, Color _otherColor)
    {
        return Mathf.Abs(_color.r - _otherColor.r) + Mathf.Abs(_color.g - _otherColor.g) + Mathf.Abs(_color.b - _otherColor.b);
    }

    public static float GetHSVHue(this Color _color)
    {
        Color.RGBToHSV(_color, out float _hue, out _, out _);
        return _hue;
    }

    public static float GetHSVSaturation(this Color _color)
    {
        Color.RGBToHSV(_color, out _, out float _saturation, out _);
        return _saturation;
    }

    public static float GetHSVValue(this Color _color)
    {
        Color.RGBToHSV(_color, out _, out _, out float _value);
        return _value;
    }

    public static bool IsSimilar(this Color _first, Color _toMatch, float _threshold)
    {
        return _first.IsSimilar(_toMatch, _threshold, out _);
    }

    public static bool IsSimilar(this Color _first, Color _toMatch, float _threshold, out float _difference)
    {
        float _rDist = Mathf.Abs(_first.r - _toMatch.r);
        float _gDist = Mathf.Abs(_first.g - _toMatch.g);
        float _bDist = Mathf.Abs(_first.b - _toMatch.b);

        _difference = _rDist + _gDist + _bDist;
        return _difference <= _threshold;
    }

    public static string ToHTML(this Color _color, bool _withAlpha = true)
    {
        return _withAlpha ? ColorUtility.ToHtmlStringRGBA(_color) : ColorUtility.ToHtmlStringRGB(_color);
    }
}

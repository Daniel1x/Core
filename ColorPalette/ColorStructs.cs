namespace DL.ColorPalette.Structs
{
    using DL.ColorPalette.Interfaces;
    using UnityEngine;
    using static UnityEngine.ColorUtility;

    [System.Serializable]
    public struct SingleColor : IFirstColor
    {
        public Color Color1;
        public Color FirstColor => Color1;
        public SingleColor(Color _color) => Color1 = _color;
        public SingleColor(IFirstColor _color) => Color1 = _color.FirstColor;
        public string GetColorHexCodes() => ToHtmlStringRGBA(Color1);
        public static implicit operator Color(SingleColor _singleColor) => _singleColor.Color1;
        public static implicit operator SingleColor(Color _color) => new(_color);
        public static bool operator ==(SingleColor _c1, SingleColor _c2) => _c1.Color1 == _c2.Color1;
        public static bool operator !=(SingleColor _c1, SingleColor _c2) => _c1.Color1 != _c2.Color1;
        public override bool Equals(object _obj) => _obj is SingleColor _other && this == _other;
        public override int GetHashCode() => Color1.GetHashCode();
    }

    [System.Serializable]
    public struct SingleColorHDR : IFirstColor
    {
        [ColorUsage(true, true)] public Color Color1;
        public Color FirstColor => Color1;
        public SingleColorHDR(Color _color) => Color1 = _color;
        public SingleColorHDR(IFirstColor _color) => Color1 = _color.FirstColor;
        public string GetColorHexCodes() => ToHtmlStringRGBA(Color1);
        public static implicit operator Color(SingleColorHDR _singleColor) => _singleColor.Color1;
        public static implicit operator SingleColorHDR(Color _color) => new(_color);
        public static bool operator ==(SingleColorHDR _c1, SingleColorHDR _c2) => _c1.Color1 == _c2.Color1;
        public static bool operator !=(SingleColorHDR _c1, SingleColorHDR _c2) => _c1.Color1 != _c2.Color1;
        public override bool Equals(object _obj) => _obj is SingleColorHDR _other && this == _other;
        public override int GetHashCode() => Color1.GetHashCode();
    }

    [System.Serializable]
    public struct ColorPair : ISecondColor
    {
        public Color Color1;
        public Color Color2;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public ColorPair(Color _color1, Color _color2)
        {
            Color1 = _color1;
            Color2 = _color2;
        }
        public ColorPair(ISecondColor _colorPair)
        {
            Color1 = _colorPair.FirstColor;
            Color2 = _colorPair.SecondColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}";
        public static implicit operator (Color, Color)(ColorPair _colorPair) => (_colorPair.Color1, _colorPair.Color2);
        public static implicit operator ColorPair((Color, Color) _colors) => new(_colors.Item1, _colors.Item2);
        public static bool operator ==(ColorPair _c1, ColorPair _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2;
        public static bool operator !=(ColorPair _c1, ColorPair _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2;
        public override bool Equals(object _obj) => _obj is ColorPair _other && this == _other;
        public override int GetHashCode() => (Color1, Color2).GetHashCode();
    }

    [System.Serializable]
    public struct ColorPairHDR : ISecondColor
    {
        [ColorUsage(true, true)] public Color Color1;
        [ColorUsage(true, true)] public Color Color2;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public ColorPairHDR(Color _color1, Color _color2)
        {
            Color1 = _color1;
            Color2 = _color2;
        }
        public ColorPairHDR(ISecondColor _colorPair)
        {
            Color1 = _colorPair.FirstColor;
            Color2 = _colorPair.SecondColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}";
        public static implicit operator (Color, Color)(ColorPairHDR _colorPair) => (_colorPair.Color1, _colorPair.Color2);
        public static implicit operator ColorPairHDR((Color, Color) _colors) => new(_colors.Item1, _colors.Item2);
        public static bool operator ==(ColorPairHDR _c1, ColorPairHDR _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2;
        public static bool operator !=(ColorPairHDR _c1, ColorPairHDR _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2;
        public override bool Equals(object _obj) => _obj is ColorPairHDR _other && this == _other;
        public override int GetHashCode() => (Color1, Color2).GetHashCode();
    }

    [System.Serializable]
    public struct ColorTriplet : IThirdColor
    {
        public Color Color1;
        public Color Color2;
        public Color Color3;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public ColorTriplet(Color _color1, Color _color2, Color _color3)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
        }
        public ColorTriplet(IThirdColor _colorTriplet)
        {
            Color1 = _colorTriplet.FirstColor;
            Color2 = _colorTriplet.SecondColor;
            Color3 = _colorTriplet.ThirdColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}";
        public static implicit operator (Color, Color, Color)(ColorTriplet _colorTriple) => (_colorTriple.Color1, _colorTriple.Color2, _colorTriple.Color3);
        public static implicit operator ColorTriplet((Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3);
        public static bool operator ==(ColorTriplet _c1, ColorTriplet _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3;
        public static bool operator !=(ColorTriplet _c1, ColorTriplet _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3;
        public override bool Equals(object _obj) => _obj is ColorTriplet _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3).GetHashCode();
    }

    [System.Serializable]
    public struct ColorTripletHDR : IThirdColor
    {
        [ColorUsage(true, true)] public Color Color1;
        [ColorUsage(true, true)] public Color Color2;
        [ColorUsage(true, true)] public Color Color3;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public ColorTripletHDR(Color _color1, Color _color2, Color _color3)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
        }
        public ColorTripletHDR(IThirdColor _colorTriplet)
        {
            Color1 = _colorTriplet.FirstColor;
            Color2 = _colorTriplet.SecondColor;
            Color3 = _colorTriplet.ThirdColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}";
        public static implicit operator (Color, Color, Color)(ColorTripletHDR _colorTriple) => (_colorTriple.Color1, _colorTriple.Color2, _colorTriple.Color3);
        public static implicit operator ColorTripletHDR((Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3);
        public static bool operator ==(ColorTripletHDR _c1, ColorTripletHDR _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3;
        public static bool operator !=(ColorTripletHDR _c1, ColorTripletHDR _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3;
        public override bool Equals(object _obj) => _obj is ColorTripletHDR _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3).GetHashCode();
    }

    [System.Serializable]
    public struct ColorQuartet : IFourthColor
    {
        public Color Color1;
        public Color Color2;
        public Color Color3;
        public Color Color4;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public Color FourthColor => Color4;
        public ColorQuartet(Color _color1, Color _color2, Color _color3, Color _color4)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
            Color4 = _color4;
        }
        public ColorQuartet(IFourthColor _colorQuartet)
        {
            Color1 = _colorQuartet.FirstColor;
            Color2 = _colorQuartet.SecondColor;
            Color3 = _colorQuartet.ThirdColor;
            Color4 = _colorQuartet.FourthColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}, {ToHtmlStringRGBA(Color4)}";
        public static implicit operator (Color, Color, Color, Color)(ColorQuartet _colorQuadruple) => (_colorQuadruple.Color1, _colorQuadruple.Color2, _colorQuadruple.Color3, _colorQuadruple.Color4);
        public static implicit operator ColorQuartet((Color, Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3, _colors.Item4);
        public static bool operator ==(ColorQuartet _c1, ColorQuartet _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3 && _c1.Color4 == _c2.Color4;
        public static bool operator !=(ColorQuartet _c1, ColorQuartet _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3 || _c1.Color4 != _c2.Color4;
        public override bool Equals(object _obj) => _obj is ColorQuartet _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3, Color4).GetHashCode();
    }

    [System.Serializable]
    public struct ColorQuartetHDR : IFourthColor
    {
        [ColorUsage(true, true)] public Color Color1;
        [ColorUsage(true, true)] public Color Color2;
        [ColorUsage(true, true)] public Color Color3;
        [ColorUsage(true, true)] public Color Color4;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public Color FourthColor => Color4;
        public ColorQuartetHDR(Color _color1, Color _color2, Color _color3, Color _color4)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
            Color4 = _color4;
        }
        public ColorQuartetHDR(IFourthColor _colorQuartet)
        {
            Color1 = _colorQuartet.FirstColor;
            Color2 = _colorQuartet.SecondColor;
            Color3 = _colorQuartet.ThirdColor;
            Color4 = _colorQuartet.FourthColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}, {ToHtmlStringRGBA(Color4)}";
        public static implicit operator (Color, Color, Color, Color)(ColorQuartetHDR _colorQuadruple) => (_colorQuadruple.Color1, _colorQuadruple.Color2, _colorQuadruple.Color3, _colorQuadruple.Color4);
        public static implicit operator ColorQuartetHDR((Color, Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3, _colors.Item4);
        public static bool operator ==(ColorQuartetHDR _c1, ColorQuartetHDR _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3 && _c1.Color4 == _c2.Color4;
        public static bool operator !=(ColorQuartetHDR _c1, ColorQuartetHDR _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3 || _c1.Color4 != _c2.Color4;
        public override bool Equals(object _obj) => _obj is ColorQuartetHDR _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3, Color4).GetHashCode();
    }

    [System.Serializable]
    public struct ColorQuintet : IFifthColor
    {
        public Color Color1;
        public Color Color2;
        public Color Color3;
        public Color Color4;
        public Color Color5;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public Color FourthColor => Color4;
        public Color FifthColor => Color5;
        public ColorQuintet(Color _color1, Color _color2, Color _color3, Color _color4, Color _color5)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
            Color4 = _color4;
            Color5 = _color5;
        }
        public ColorQuintet(IFifthColor _colorQuintet)
        {
            Color1 = _colorQuintet.FirstColor;
            Color2 = _colorQuintet.SecondColor;
            Color3 = _colorQuintet.ThirdColor;
            Color4 = _colorQuintet.FourthColor;
            Color5 = _colorQuintet.FifthColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}, {ToHtmlStringRGBA(Color4)}, {ToHtmlStringRGBA(Color5)}";
        public static implicit operator (Color, Color, Color, Color, Color)(ColorQuintet _colorQuintuple) => (_colorQuintuple.Color1, _colorQuintuple.Color2, _colorQuintuple.Color3, _colorQuintuple.Color4, _colorQuintuple.Color5);
        public static implicit operator ColorQuintet((Color, Color, Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3, _colors.Item4, _colors.Item5);
        public static bool operator ==(ColorQuintet _c1, ColorQuintet _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3 && _c1.Color4 == _c2.Color4 && _c1.Color5 == _c2.Color5;
        public static bool operator !=(ColorQuintet _c1, ColorQuintet _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3 || _c1.Color4 != _c2.Color4 || _c1.Color5 != _c2.Color5;
        public override bool Equals(object _obj) => _obj is ColorQuintet _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3, Color4, Color5).GetHashCode();
    }

    [System.Serializable]
    public struct ColorQuintetHDR : IFifthColor
    {
        [ColorUsage(true, true)] public Color Color1;
        [ColorUsage(true, true)] public Color Color2;
        [ColorUsage(true, true)] public Color Color3;
        [ColorUsage(true, true)] public Color Color4;
        [ColorUsage(true, true)] public Color Color5;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public Color FourthColor => Color4;
        public Color FifthColor => Color5;
        public ColorQuintetHDR(Color _color1, Color _color2, Color _color3, Color _color4, Color _color5)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
            Color4 = _color4;
            Color5 = _color5;
        }
        public ColorQuintetHDR(IFifthColor _colorQuintet)
        {
            Color1 = _colorQuintet.FirstColor;
            Color2 = _colorQuintet.SecondColor;
            Color3 = _colorQuintet.ThirdColor;
            Color4 = _colorQuintet.FourthColor;
            Color5 = _colorQuintet.FifthColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}, {ToHtmlStringRGBA(Color4)}, {ToHtmlStringRGBA(Color5)}";
        public static implicit operator (Color, Color, Color, Color, Color)(ColorQuintetHDR _colorQuintuple) => (_colorQuintuple.Color1, _colorQuintuple.Color2, _colorQuintuple.Color3, _colorQuintuple.Color4, _colorQuintuple.Color5);
        public static implicit operator ColorQuintetHDR((Color, Color, Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3, _colors.Item4, _colors.Item5);
        public static bool operator ==(ColorQuintetHDR _c1, ColorQuintetHDR _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3 && _c1.Color4 == _c2.Color4 && _c1.Color5 == _c2.Color5;
        public static bool operator !=(ColorQuintetHDR _c1, ColorQuintetHDR _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3 || _c1.Color4 != _c2.Color4 || _c1.Color5 != _c2.Color5;
        public override bool Equals(object _obj) => _obj is ColorQuintetHDR _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3, Color4, Color5).GetHashCode();
    }

    [System.Serializable]
    public struct ColorSextet : ISixthColor
    {
        public Color Color1;
        public Color Color2;
        public Color Color3;
        public Color Color4;
        public Color Color5;
        public Color Color6;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public Color FourthColor => Color4;
        public Color FifthColor => Color5;
        public Color SixthColor => Color6;
        public ColorSextet(Color _color1, Color _color2, Color _color3, Color _color4, Color _color5, Color _color6)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
            Color4 = _color4;
            Color5 = _color5;
            Color6 = _color6;
        }
        public ColorSextet(ISixthColor _colorSextet)
        {
            Color1 = _colorSextet.FirstColor;
            Color2 = _colorSextet.SecondColor;
            Color3 = _colorSextet.ThirdColor;
            Color4 = _colorSextet.FourthColor;
            Color5 = _colorSextet.FifthColor;
            Color6 = _colorSextet.SixthColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}, {ToHtmlStringRGBA(Color4)}, {ToHtmlStringRGBA(Color5)}, {ToHtmlStringRGBA(Color6)}";
        public static implicit operator (Color, Color, Color, Color, Color, Color)(ColorSextet _colorSextuple) => (_colorSextuple.Color1, _colorSextuple.Color2, _colorSextuple.Color3, _colorSextuple.Color4, _colorSextuple.Color5, _colorSextuple.Color6);
        public static implicit operator ColorSextet((Color, Color, Color, Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3, _colors.Item4, _colors.Item5, _colors.Item6);
        public static bool operator ==(ColorSextet _c1, ColorSextet _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3 && _c1.Color4 == _c2.Color4 && _c1.Color5 == _c2.Color5 && _c1.Color6 == _c2.Color6;
        public static bool operator !=(ColorSextet _c1, ColorSextet _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3 || _c1.Color4 != _c2.Color4 || _c1.Color5 != _c2.Color5 || _c1.Color6 != _c2.Color6;
        public override bool Equals(object _obj) => _obj is ColorSextet _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3, Color4, Color5, Color6).GetHashCode();
    }

    [System.Serializable]
    public struct ColorSextetHDR : ISixthColor
    {
        [ColorUsage(true, true)] public Color Color1;
        [ColorUsage(true, true)] public Color Color2;
        [ColorUsage(true, true)] public Color Color3;
        [ColorUsage(true, true)] public Color Color4;
        [ColorUsage(true, true)] public Color Color5;
        [ColorUsage(true, true)] public Color Color6;
        public Color FirstColor => Color1;
        public Color SecondColor => Color2;
        public Color ThirdColor => Color3;
        public Color FourthColor => Color4;
        public Color FifthColor => Color5;
        public Color SixthColor => Color6;
        public ColorSextetHDR(Color _color1, Color _color2, Color _color3, Color _color4, Color _color5, Color _color6)
        {
            Color1 = _color1;
            Color2 = _color2;
            Color3 = _color3;
            Color4 = _color4;
            Color5 = _color5;
            Color6 = _color6;
        }
        public ColorSextetHDR(ISixthColor _colorSextet)
        {
            Color1 = _colorSextet.FirstColor;
            Color2 = _colorSextet.SecondColor;
            Color3 = _colorSextet.ThirdColor;
            Color4 = _colorSextet.FourthColor;
            Color5 = _colorSextet.FifthColor;
            Color6 = _colorSextet.SixthColor;
        }
        public string GetColorHexCodes() => $"{ToHtmlStringRGBA(Color1)}, {ToHtmlStringRGBA(Color2)}, {ToHtmlStringRGBA(Color3)}, {ToHtmlStringRGBA(Color4)}, {ToHtmlStringRGBA(Color5)}, {ToHtmlStringRGBA(Color6)}";
        public static implicit operator (Color, Color, Color, Color, Color, Color)(ColorSextetHDR _colorSextuple) => (_colorSextuple.Color1, _colorSextuple.Color2, _colorSextuple.Color3, _colorSextuple.Color4, _colorSextuple.Color5, _colorSextuple.Color6);
        public static implicit operator ColorSextetHDR((Color, Color, Color, Color, Color, Color) _colors) => new(_colors.Item1, _colors.Item2, _colors.Item3, _colors.Item4, _colors.Item5, _colors.Item6);
        public static bool operator ==(ColorSextetHDR _c1, ColorSextetHDR _c2) => _c1.Color1 == _c2.Color1 && _c1.Color2 == _c2.Color2 && _c1.Color3 == _c2.Color3 && _c1.Color4 == _c2.Color4 && _c1.Color5 == _c2.Color5 && _c1.Color6 == _c2.Color6;
        public static bool operator !=(ColorSextetHDR _c1, ColorSextetHDR _c2) => _c1.Color1 != _c2.Color1 || _c1.Color2 != _c2.Color2 || _c1.Color3 != _c2.Color3 || _c1.Color4 != _c2.Color4 || _c1.Color5 != _c2.Color5 || _c1.Color6 != _c2.Color6;
        public override bool Equals(object _obj) => _obj is ColorSextetHDR _other && this == _other;
        public override int GetHashCode() => (Color1, Color2, Color3, Color4, Color5, Color6).GetHashCode();
    }
}

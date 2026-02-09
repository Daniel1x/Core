namespace DL.ColorPalette
{
    using DL.ColorPalette.Interfaces;
    using DL.ColorPalette.Structs;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ColorPalettePreview", menuName = "DL/Colors/Color Palette Preview", order = 1)]
    public class ColorPalettePreviewObject : ScriptableObject
    {
        [SerializeField] private ColorPalettePreview preview = new();

        private void OnValidate()
        {
            preview.OnValidate();
        }
    }

    [System.Serializable]
    public class ColorPalettePreview
    {
        [System.Serializable]
        public struct ColorEntry<T> where T : ColorHexCode
        {
            [SerializeField] private T color;
            [SerializeField] private string hex;

            public T Color => color;
            public string Hex => hex;

            public ColorEntry(T _color)
            {
                color = _color;
                hex = _color.GetColorHexCodes();
            }

            public static implicit operator T(ColorEntry<T> _entry) => _entry.Color;
            public static implicit operator ColorEntry<T>(T _color) => new(_color);
        }

        [Header("Base Color Settings")]
        [SerializeField, Range(0f, 1f)] protected float saturation01 = 1f;
        [SerializeField, Range(0f, 1f)] protected float brightness01 = 1f;
        [SerializeField, Range(0f, 90f)] protected float angleDegrees = 30f;
        [SerializeField] protected Color baseColor = Color.white;

        [Header("Generated Colors")]
        [SerializeField] protected ColorEntry<ColorPair> complementaryColor = default;
        [SerializeField] protected ColorEntry<ColorTriplet> analogousColor = default;
        [SerializeField] protected ColorEntry<ColorTriplet> triadicColor = default;
        [SerializeField] protected ColorEntry<ColorTriplet> splitComplementaryColor = default;
        [SerializeField] protected ColorEntry<ColorQuartet> tetradicColor = default;
        [SerializeField] protected ColorEntry<ColorQuartet> squareColor = default;
        [SerializeField] protected ColorEntry<ColorQuartet> abstractColor = default;
        [SerializeField] protected ColorEntry<ColorQuintet> pentadicColor = default;
        [SerializeField] protected ColorEntry<ColorQuintet> pentadicAngledColor = default;
        [SerializeField] protected ColorEntry<ColorSextet> hexadicColor = default;
        [SerializeField] protected ColorEntry<ColorSextet> hexadicAngledColor = default;

        public float Saturation
        {
            get => saturation01;
            set
            {
                float _clampedValue = Mathf.Clamp01(value);

                if (_clampedValue != saturation01)
                {
                    saturation01 = _clampedValue;
                    OnValidate();
                }
            }
        }

        public float Brightness
        {
            get => brightness01;
            set
            {
                float _clampedValue = Mathf.Clamp01(value);

                if (_clampedValue != brightness01)
                {
                    brightness01 = _clampedValue;
                    OnValidate();
                }
            }
        }

        public float AngleDegrees
        {
            get => angleDegrees;
            set
            {
                float _clampedValue = Mathf.Clamp(value, 0f, 90f);

                if (_clampedValue != angleDegrees)
                {
                    angleDegrees = _clampedValue;
                    OnValidate();
                }
            }
        }

        public Color BaseColor
        {
            get => baseColor;
            set
            {
                if (value != baseColor)
                {
                    baseColor = value;
                    OnValidate();
                }
            }
        }

        public ColorPair ComplementaryColor => complementaryColor;
        public ColorTriplet AnalogousColor => analogousColor;
        public ColorTriplet TriadicColor => triadicColor;
        public ColorTriplet SplitComplementaryColor => splitComplementaryColor;
        public ColorQuartet TetradicColor => tetradicColor;
        public ColorQuartet SquareColor => squareColor;
        public ColorQuartet AbstractColor => abstractColor;
        public ColorQuintet PentadicColor => pentadicColor;
        public ColorQuintet PentadicAngledColor => pentadicAngledColor;
        public ColorSextet HexadicColor => hexadicColor;
        public ColorSextet HexadicAngledColor => hexadicAngledColor;

        public void OnValidate()
        {
            Color _color = baseColor;

            complementaryColor = _color.CreateComplementaryColors(saturation01, brightness01);
            analogousColor = _color.CreateAnalogousColors(saturation01, brightness01, angleDegrees);
            triadicColor = _color.CreateTriadicColors(saturation01, brightness01);
            splitComplementaryColor = _color.CreateSplitComplementaryColors(saturation01, brightness01, angleDegrees);
            tetradicColor = _color.CreateTetradicColors(saturation01, brightness01, angleDegrees);
            squareColor = _color.CreateSquareColors(saturation01, brightness01);
            abstractColor = _color.CreateAbstractColors(saturation01, brightness01, angleDegrees);
            pentadicColor = _color.CreatePentadicColors(saturation01, brightness01);
            pentadicAngledColor = _color.CreatePentadicColors(saturation01, brightness01, angleDegrees);
            hexadicColor = _color.CreateHexadicColors(saturation01, brightness01);
            hexadicAngledColor = _color.CreateHexadicColors(saturation01, brightness01, angleDegrees);
        }
    }
}

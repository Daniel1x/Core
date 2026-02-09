namespace DL.ColorPalette.Interfaces
{
    using UnityEngine;

    public interface IPaletteProvider<T> where T : IColorPalette
    {
        public event System.Action<IPaletteProvider<T>> OnPaletteChanged;

        public T GetPalette();
    }

    public interface IColorPalette { }

    public interface ColorHexCode
    {
        public string GetColorHexCodes();
    }

    public interface IFirstColor : ColorHexCode, IColorPalette
    {
        public Color FirstColor { get; }
    }

    public interface ISecondColor : IFirstColor
    {
        public Color SecondColor { get; }
    }

    public interface IThirdColor : ISecondColor
    {
        public Color ThirdColor { get; }
    }

    public interface IFourthColor : IThirdColor
    {
        public Color FourthColor { get; }
    }

    public interface IFifthColor : IFourthColor
    {
        public Color FifthColor { get; }
    }

    public interface ISixthColor : IFifthColor
    {
        public Color SixthColor { get; }
    }
}

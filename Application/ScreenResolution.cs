using System;
using UnityEngine;

[System.Serializable]
public struct ScreenResolution
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    public Vector2Int Size => new Vector2Int(Width, Height);

    public ScreenResolution(Resolution _resolution)
    {
        Width = _resolution.width;
        Height = _resolution.height;
    }

    public ScreenResolution(Vector2Int _size)
    {
        Width = _size.x;
        Height = _size.y;
    }

    public override string ToString()
    {
        int _divisor = Width.GreatestCommonDivisor(Height).ClampMin(1);
        return $"({Width}x{Height} [{Width / _divisor}:{Height / _divisor}])";
    }

    public override bool Equals(object _obj)
    {
        return _obj is ScreenResolution _resolution &&
               Width == _resolution.Width &&
               Height == _resolution.Height;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static bool operator ==(ScreenResolution _this, ScreenResolution _other)
    {
        return _this.Width == _other.Width
            && _this.Height == _other.Height;
    }

    public static bool operator !=(ScreenResolution _this, ScreenResolution _other)
    {
        return _this.Width != _other.Width
            || _this.Height != _other.Height;
    }

    public static bool operator ==(ScreenResolution _this, Resolution _other)
    {
        return _this.Width == _other.width
            && _this.Height == _other.height;
    }

    public static bool operator !=(ScreenResolution _this, Resolution _other)
    {
        return _this.Width != _other.width
            || _this.Height != _other.height;
    }
}

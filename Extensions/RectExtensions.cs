using DL.Structs;
using UnityEngine;

public static class RectExtensions
{
    public static float GetAspectRatio(this Rect _rect)
    {
        return _rect.y == 0f
            ? 0f
            : _rect.x / _rect.y;
    }

    public static Rect HorizontalRange(this Rect _rect, MinMax _range)
    {
        Vector2 _position = _rect.position;
        _position.x += _rect.width * _range.Min;

        Vector2 _size = _rect.size;
        _size.x *= _range.Range;

        return new Rect(_position, _size);
    }

    public static Rect HorizontalRange(this Rect _rect, float _min, float _max)
    {
        return _rect.HorizontalRange(new(_min, _max));
    }

    public static Rect VerticalRange(this Rect _rect, MinMax _range)
    {
        Vector2 _position = _rect.position;
        _position.y += _rect.height * (1f - _range.Max);

        Vector2 _size = _rect.size;
        _size.y *= _range.Range;

        return new Rect(_position, _size);
    }

    public static Rect VerticalRange(this Rect _rect, float _min, float _max)
    {
        return _rect.VerticalRange(new(_min, _max));
    }

    public static Rect WithRange(this Rect _rect, MinMax _xRange, MinMax _yRange)
    {
        Vector2 _position = _rect.position;
        _position.x += _rect.width * _xRange.Min;
        _position.y += _rect.height * (1f - _yRange.Max);

        Vector2 _size = _rect.size;
        _size.x *= _xRange.Range;
        _size.y *= _yRange.Range;

        return new Rect(_position, _size);
    }

    public static Rect WithRange(this Rect _rect, float _minX, float _maxX, float _minY, float _maxY)
    {
        return _rect.WithRange(new(_minX, _maxX), new(_minY, _maxY));
    }

    public static Rect ExpandEvenly(this Rect _rect, float _pixels)
    {
        return _rect.ExpandEvenly(_pixels, _pixels);
    }

    public static Rect ExpandEvenly(this Rect _rect, float _xPixels, float _yPixels)
    {
        _rect.height += _yPixels;
        _rect.y -= _yPixels * 0.5f;

        _rect.width += _xPixels;
        _rect.x -= _xPixels * 0.5f;

        return _rect;
    }

    public static Rect ExpandEvenlyHorizontal(this Rect _rect, float _xPixels)
    {
        _rect.width += _xPixels;
        _rect.x -= _xPixels * 0.5f;

        return _rect;
    }

    public static Rect ExpandEvenlyVertical(this Rect _rect, float _yPixels)
    {
        _rect.height += _yPixels;
        _rect.y -= _yPixels * 0.5f;

        return _rect;
    }

    public static float GetWidthRange(this Rect _rect, float _xPixels)
    {
        return _rect.width == 0f
            ? 0f
            : _xPixels / _rect.width;
    }

    public static float GetHeightRange(this Rect _rect, float _yPixels)
    {
        return _rect.height == 0f
            ? 0f
            : _yPixels / _rect.height;
    }

    public static Vector2 GetRange(this Rect _rect, float _xPixels, float _yPixels)
    {
        return new Vector2()
        {
            x = _rect.width == 0f
                ? 0f
                : _xPixels / _rect.width,
            y = _rect.height == 0f
                ? 0f
                : _yPixels / _rect.height,
        };
    }

    public static void SplitHorizontal(this Rect _rect, float _xSplit01, out Rect _left, out Rect _right)
    {
        _xSplit01 = Mathf.Clamp01(_xSplit01);
        _left = _rect.HorizontalRange(0f, _xSplit01);
        _right = _rect.HorizontalRange(_xSplit01, 1f);
    }

    public static void SplitVertical(this Rect _rect, float _ySplit01, out Rect _top, out Rect _bottom)
    {
        _ySplit01 = Mathf.Clamp01(_ySplit01);
        _top = _rect.VerticalRange(_ySplit01, 1f);
        _bottom = _rect.VerticalRange(0f, _ySplit01);
    }

    public static void Split(this Rect _rect, Vector2 _split01, out Rect _topLeft, out Rect _topRight, out Rect _bottomLeft, out Rect _bottomRight)
    {
        _rect.SplitVertical(_split01.y, out var _topLine, out var _bottomLine);
        _topLine.SplitHorizontal(_split01.x, out _topLeft, out _topRight);
        _bottomLine.SplitHorizontal(_split01.x, out _bottomLeft, out _bottomRight);
    }

    public static void SplitHorizontalPixels(this Rect _rect, float _xPixels, bool _fromLeft, out Rect _left, out Rect _right, float _spacing = 0f)
    {
        if (_xPixels < 0f)
        {
            _xPixels = 0f;
        }

        float _range = _rect.GetWidthRange(_xPixels);

        if (_fromLeft)
        {
            _rect.SplitHorizontal(_range, out _left, out _right);

            if (_spacing > 0f)
            {
                _right.SplitHorizontal(_right.GetWidthRange(_spacing), out _, out _right);
            }
        }
        else
        {
            _rect.SplitHorizontal(1f - _range, out _left, out _right);

            if (_spacing > 0f)
            {
                _left.SplitHorizontal(1f - _left.GetWidthRange(_spacing), out _left, out _);
            }
        }
    }

    public static void SplitVerticalPixels(this Rect _rect, float _yPixels, bool _fromBottom, out Rect _top, out Rect _bottom, float _spacing = 0f)
    {
        if (_yPixels < 0f)
        {
            _yPixels = 0f;
        }

        float _range = _rect.GetHeightRange(_yPixels);

        if (_fromBottom)
        {
            _rect.SplitVertical(_range, out _top, out _bottom);

            if (_spacing > 0f)
            {
                _top.SplitVertical(_top.GetHeightRange(_spacing), out _top, out _);
            }
        }
        else
        {
            _rect.SplitVertical(1f - _range, out _top, out _bottom);

            if (_spacing > 0f)
            {
                _bottom.SplitVertical(1f - _bottom.GetHeightRange(_spacing), out _, out _bottom);
            }
        }
    }

    public static void SplitPixels(this Rect _rect, Vector2 _pixels, bool _fromLeft, bool _fromBottom, out Rect _topLeft, out Rect _topRight, out Rect _bottomLeft, out Rect _bottomRight)
    {
        _rect.SplitVerticalPixels(_pixels.y, _fromBottom, out Rect _topLine, out Rect _bottonLine);
        _topLine.SplitHorizontalPixels(_pixels.x, _fromLeft, out _topLeft, out _topRight);
        _bottonLine.SplitVerticalPixels(_pixels.x, _fromLeft, out _bottomLeft, out _bottomRight);
    }

    public static void SplitHorizontalPixels(this Rect _rect, float _xPixelsFromLeft, float _xPixelsFromRight, out Rect _left, out Rect _center, out Rect _right, float _spacing = 0f)
    {
        if (_xPixelsFromLeft < 0f)
        {
            _xPixelsFromLeft = 0f;
        }

        if (_xPixelsFromRight < 0f)
        {
            _xPixelsFromRight = 0f;
        }

        float _spacingRange = _spacing > 0f ? _rect.GetWidthRange(_spacing) : 0f;
        float _leftRange = _rect.GetWidthRange(_xPixelsFromLeft);
        float _rightRange = _rect.GetWidthRange(_xPixelsFromRight);
        float _centerRange = 1f - _leftRange - _rightRange - (2f * _spacingRange);

        if (_centerRange < 0f)
        {
            _leftRange /= _leftRange + _rightRange; //Normalize
            _centerRange = 0f;
        }

        _left = _rect.HorizontalRange(0f, _leftRange);
        _center = _rect.HorizontalRange(_leftRange + _spacingRange, _leftRange + _spacingRange + _centerRange);
        _right = _rect.HorizontalRange(_leftRange + (2f * _spacingRange) + _centerRange, 1f);
    }

    public static void SplitVerticalPixels(this Rect _rect, float _yPixelsFromBottom, float _yPixelsFromTop, out Rect _top, out Rect _center, out Rect _bottom, float _spacing = 0f)
    {
        if (_yPixelsFromBottom < 0f)
        {
            _yPixelsFromBottom = 0f;
        }

        if (_yPixelsFromTop < 0f)
        {
            _yPixelsFromTop = 0f;
        }

        float _spacingRange = _spacing > 0f ? _rect.GetHeightRange(_spacing) : 0f;
        float _bottomRange = _rect.GetHeightRange(_yPixelsFromBottom);
        float _topRange = _rect.GetHeightRange(_yPixelsFromTop);
        float _centerRange = 1f - _topRange - _bottomRange - (2f * _spacingRange);

        if (_centerRange < 0f)
        {
            _bottomRange /= _bottomRange + _topRange; //Normalize
            _centerRange = 0f;
        }

        _bottom = _rect.VerticalRange(0f, _bottomRange);
        _center = _rect.VerticalRange(_bottomRange + _spacingRange, _bottomRange + _spacingRange + _centerRange);
        _top = _rect.VerticalRange(_bottomRange + (2f * _spacingRange) + _centerRange, 1f);
    }

    public static Rect FlipHorizontal(this Rect _rect)
    {
        _rect.x += _rect.width;
        _rect.width *= -1f;
        return _rect;
    }

    public static Rect FlipVertical(this Rect _rect)
    {
        _rect.y += _rect.height;
        _rect.height *= -1f;
        return _rect;
    }

    public static bool Overlaps(this Rect _rect, Rect _other, bool _allowInverse = true)
    {
        return _rect.Overlaps(_other, _allowInverse);
    }

    public static Rect CreateRect(this Vector2 _from, Vector2 _to)
    {
        Vector2 _upperLeftCorner = new Vector2()
        {
            x = Mathf.Min(_from.x, _to.x),
            y = Mathf.Min(_from.y, _to.y)
        };

        Vector2 _lowerRightCorner = new Vector2()
        {
            x = Mathf.Max(_from.x, _to.x),
            y = Mathf.Max(_from.y, _to.y)
        };

        return new Rect(_upperLeftCorner, _lowerRightCorner - _upperLeftCorner);
    }

    ///<summary>It changes rect only in editor!</summary>
    public static Rect Indented(this Rect _rect)
    {
#if UNITY_EDITOR
        return UnityEditor.EditorGUI.IndentedRect(_rect);
#else
        return _rect;
#endif
    }
}

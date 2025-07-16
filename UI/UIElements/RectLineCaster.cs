using UnityEngine;

public static class RectLineCaster
{
    public struct LineCastEntry
    {
        public float Entry;
        public bool Intersects => Entry != float.PositiveInfinity;

        public LineCastEntry(float _entry)
        {
            Entry = _entry;
        }
    }

    public enum RectPart
    {
        None,
        UpperLeft,
        UpperCenter,
        UpperRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        LowerLeft,
        LowerCenter,
        LowerRight
    }

    private static readonly RectPart[,] raycastLookup =
    {
        {
            RectPart.None,  RectPart.None,      RectPart.None,
            RectPart.None,  RectPart.UpperLeft, RectPart.UpperLeft,
            RectPart.None,  RectPart.UpperLeft, RectPart.UpperLeft,
        },
        {
            RectPart.None,          RectPart.None,          RectPart.None,
            RectPart.UpperCenter,   RectPart.UpperCenter,   RectPart.UpperCenter,
            RectPart.UpperCenter,   RectPart.UpperCenter,   RectPart.UpperCenter,
        },
        {
            RectPart.None,          RectPart.None,          RectPart.None,
            RectPart.UpperRight,    RectPart.UpperRight,    RectPart.None,
            RectPart.UpperRight,    RectPart.UpperRight,    RectPart.None,
        },
        {
            RectPart.None,  RectPart.MiddleLeft,    RectPart.MiddleLeft,
            RectPart.None,  RectPart.MiddleLeft,    RectPart.MiddleLeft,
            RectPart.None,  RectPart.MiddleLeft,    RectPart.MiddleLeft,
        },
        {
            RectPart.MiddleCenter,  RectPart.MiddleCenter,  RectPart.MiddleCenter,
            RectPart.MiddleCenter,  RectPart.MiddleCenter,  RectPart.MiddleCenter,
            RectPart.MiddleCenter,  RectPart.MiddleCenter,  RectPart.MiddleCenter,
        },
        {
            RectPart.MiddleRight,   RectPart.MiddleRight,   RectPart.None,
            RectPart.MiddleRight,   RectPart.MiddleRight,   RectPart.None,
            RectPart.MiddleRight,   RectPart.MiddleRight,   RectPart.None,
        },
        {
            RectPart.None,  RectPart.LowerLeft, RectPart.LowerLeft,
            RectPart.None,  RectPart.LowerLeft, RectPart.LowerLeft,
            RectPart.None,  RectPart.None,      RectPart.None,
        },
        {
            RectPart.LowerCenter,   RectPart.LowerCenter,   RectPart.LowerCenter,
            RectPart.LowerCenter,   RectPart.LowerCenter,   RectPart.LowerCenter,
            RectPart.None,          RectPart.None,          RectPart.None,
        },
        {
            RectPart.LowerRight,    RectPart.LowerRight,    RectPart.None,
            RectPart.LowerRight,    RectPart.LowerRight,    RectPart.None,
            RectPart.None,          RectPart.None,          RectPart.None,
        },
    };

    /// <summary> Find entry intersection of a Line segment and an axis-aligned Rect. </summary>
    public static LineCastEntry CastLineEntryIntersection(Vector2 _start, Vector2 _end, Rect _rect)
    {
        Vector3 _dir = _end - _start;
        int _sectorBegin = GetRectPart(_rect, _start);
        int _sectorEnd = GetRectPart(_rect, _end);
        float _entry = GetRayToRectSide(_start, _dir, raycastLookup[_sectorBegin, _sectorEnd], _rect, 0f);

        return new LineCastEntry(_entry);
    }

    public static int GetRectPart(Rect _rect, Vector2 _point)
    {
        if (_point.y > _rect.yMax)
        {
            return _point.x < _rect.xMin
                ? 0
                : _point.x <= _rect.xMax
                ? 1
                : 2;
        }
        else if (_point.y >= _rect.yMin)
        {
            return _point.x < _rect.xMin
                ? 3
                : _point.x <= _rect.xMax
                ? 4
                : 5;
        }
        else
        {
            return _point.x < _rect.xMin
                ? 6
                : _point.x <= _rect.xMax
                ? 7
                : 8;
        }
    }

    public static float GetRayToRectSide(Vector2 _start, Vector2 _direction, RectPart _side, Rect _rect, float _inside)
    {
        return _side switch
        {
            RectPart.UpperLeft => CheckFractions(RectPart.UpperCenter, RectPart.MiddleLeft, _start, _direction, _rect, _inside),
            RectPart.UpperCenter => RayToHorizontal(_start, _direction, _rect.xMin, _rect.yMax, _rect.width),
            RectPart.UpperRight => CheckFractions(RectPart.UpperCenter, RectPart.MiddleRight, _start, _direction, _rect, _inside),
            RectPart.MiddleLeft => RayToVertical(_start, _direction, _rect.xMin, _rect.yMin, _rect.height),
            RectPart.MiddleCenter => _inside,
            RectPart.MiddleRight => RayToVertical(_start, _direction, _rect.xMax, _rect.yMin, _rect.height),
            RectPart.LowerLeft => CheckFractions(RectPart.MiddleLeft, RectPart.LowerCenter, _start, _direction, _rect, _inside),
            RectPart.LowerCenter => RayToHorizontal(_start, _direction, _rect.xMin, _rect.yMin, _rect.width),
            RectPart.LowerRight => CheckFractions(RectPart.MiddleRight, RectPart.LowerCenter, _start, _direction, _rect, _inside),
            _ => float.PositiveInfinity,
        };
    }

    public static float CheckFractions(RectPart _firstPart, RectPart _secondPart, Vector2 _start, Vector2 _direction, Rect _rect, float _inside)
    {
        float _fraction = GetRayToRectSide(_start, _direction, _firstPart, _rect, _inside);

        if (_fraction == float.PositiveInfinity)
        {
            _fraction = GetRayToRectSide(_start, _direction, _secondPart, _rect, _inside);
        }

        return _fraction;
    }

    public static float RayToHorizontal(Vector2 _start, Vector2 _dir, float _x, float _y, float _width)
    {
        float _from = (_y - _start.y) / _dir.y;

        if (_from < 0f || _from > 1f)
        {
            return float.PositiveInfinity;
        }

        float _line = (_start.x + _dir.x * _from - _x) / _width;

        return _line < 0f || _line > 1f
            ? float.PositiveInfinity
            : _from;
    }

    public static float RayToVertical(Vector2 _start, Vector2 _dir, float _x, float _y, float _height)
    {
        float _from = (_x - _start.x) / _dir.x;

        if (_from < 0f || _from > 1f)
        {
            return float.PositiveInfinity;
        }

        float _line = (_start.y + _dir.y * _from - _y) / _height;

        return _line < 0f || _line > 1f
            ? float.PositiveInfinity
            : _from;
    }
}

using System.Collections.Generic;
using UnityEngine;

public static class MathExtensions
{
    public enum RoundingStyle
    {
        ToCeil = 0,
        ToFloor = 1
    }

    public static float Normalize(this float _value, float _min, float _max)
    {
        if (_min == _max)
        {
            return 0f;
        }
        else if (_min > _max)
        {
            return Mathf.Clamp01((_value - _max) / (_min - _max));
        }
        else
        {
            return Mathf.Clamp01((_value - _min) / (_max - _min));
        }
    }

    public static float Remap(this float _value, float _from1, float _to1, float _from2, float _to2)
    {
        return (_value - _from1) / (_to1 - _from1) * (_to2 - _from2) + _from2;
    }

    public static float SineWave(float _time, float _min, float _max, float _frequency)
    {
        float _range = (_max - _min) * 0.5f;
        return _min + _range + _range * Mathf.Sin(2f * Mathf.PI * _frequency * _time);
    }

    public static float SawtoothWave(float _time, float _min, float _max, float _cycleDuration, bool _ascending)
    {
        float _normalizedTime = (_time % _cycleDuration / _cycleDuration) + (_time < 0f ? 1f : 0f);

        return _ascending
            ? Mathf.Lerp(_min, _max, _normalizedTime)
            : Mathf.Lerp(_max, _min, _normalizedTime);
    }

    public static float SquareWave(float _time, float _min, float _max, float _cycleDuration, float _normalizedDutyCycle)
    {
        return (((_time < 0f ? 1f : 0f) + (_time % _cycleDuration / _cycleDuration)) <= _normalizedDutyCycle) ? _max : _min;
    }

    public static float TriangleWave(float _time, float _min, float _max, float _cycleDuration, float _normalizedPeek)
    {
        _normalizedPeek = Mathf.Clamp01(_normalizedPeek);
        float _normalizedTime = _time % _cycleDuration / _cycleDuration + (_time < 0f ? 1f : 0f);

        return _normalizedTime <= _normalizedPeek
            ? Mathf.Lerp(_min, _max, _normalizedTime / _normalizedPeek)
            : Mathf.Lerp(_max, _min, (_normalizedTime - _normalizedPeek) / (1f - _normalizedPeek));
    }

    public static float ClampOneNegOne(this float _value)
    {
        return Mathf.Clamp(_value, -1f, 1f);
    }

    public static float Clamp01(this float _value)
    {
        return Mathf.Clamp01(_value);
    }

    public static bool FitsInRange(this float _value, float _min, float _max)
    {
        return _value >= _min && _value <= _max;
    }

    public static bool FitsInRange(this int _value, int _min, int _max)
    {
        return _value >= _min && _value <= _max;
    }

    public static bool IsInRange<T>(this T _value, T _from, T _to, bool _inclusiveLimitValues = true) where T : System.IComparable<T>
    {
        int _rangeComparison = _to.CompareTo(_from);

        if (_inclusiveLimitValues)
        {
            if (_rangeComparison > 0)
            {
                return _value.CompareTo(_from) >= 0 && _value.CompareTo(_to) <= 0;
            }
            else if (_rangeComparison < 0)
            {
                return _value.CompareTo(_to) >= 0 && _value.CompareTo(_from) <= 0;
            }
            else
            {
                return _value.CompareTo(_from) == 0;
            }
        }
        else
        {
            if (_rangeComparison > 0)
            {
                return _value.CompareTo(_from) > 0 && _value.CompareTo(_to) < 0;
            }
            else if (_rangeComparison < 0)
            {
                return _value.CompareTo(_to) > 0 && _value.CompareTo(_from) < 0;
            }
            else
            {
                return false;
            }
        }
    }

    public static bool IsOutOfRange<T>(this T _value, T _from, T _to, bool _inclusiveLimitValues = false) where T : System.IComparable<T>
    {
        return _inclusiveLimitValues
                ? !_value.IsInRange(_from, _to, false)
                : !_value.IsInRange(_from, _to, true);
    }

    public static float RoundToCeil(this float _value, float _valueDelta)
    {
        return Mathf.Ceil(_value / _valueDelta) * _valueDelta;
    }

    public static float RoundToFloor(this float _value, float _valueDelta)
    {
        return Mathf.Floor(_value / _valueDelta) * _valueDelta;
    }

    public static float Round(this float _value, RoundingStyle _style, float _valueDelta)
    {
        if (_style == RoundingStyle.ToCeil)
        {
            return _value.RoundToCeil(_valueDelta);
        }
        else
        {
            return _value.RoundToFloor(_valueDelta);
        }
    }

    public static int FloorToInt(this float _value)
    {
        return Mathf.FloorToInt(_value);
    }

    public static int RoundToInt(this float _value)
    {
        return Mathf.RoundToInt(_value);
    }

    public static int RoundToCeil(this int _value, int _valueDelta)
    {
        return Mathf.CeilToInt(_value / (float)_valueDelta) * _valueDelta;
    }

    public static int RoundToFloor(this int _value, int _valueDelta)
    {
        return Mathf.FloorToInt(_value / (float)_valueDelta) * _valueDelta;
    }

    public static int Round(this int _value, RoundingStyle _style, int _valueDelta)
    {
        if (_style == RoundingStyle.ToCeil)
        {
            return _value.RoundToCeil(_valueDelta);
        }
        else
        {
            return _value.RoundToFloor(_valueDelta);
        }
    }

    public static bool AbsGreaterThan(this float _thisValue, float _otherPositiveValue)
    {
        return Mathf.Abs(_thisValue) > _otherPositiveValue;
    }

    public static bool LastDigitIsEqualTo(this int _thisValue, int _lastDigit)
    {
        return (_thisValue % 10) == _lastDigit;
    }

    public static float ClampAngle(float _angle, float _min, float _max)
    {
        _angle = NormalizeAngle(_angle);

        if (_angle > 180f)
        {
            _angle -= 360f;
        }
        else if (_angle < -180f)
        {
            _angle += 360f;
        }

        _min = NormalizeAngle(_min);

        if (_min > 180f)
        {
            _min -= 360f;
        }
        else if (_min < -180f)
        {
            _min += 360f;
        }

        _max = NormalizeAngle(_max);

        if (_max > 180f)
        {
            _max -= 360f;
        }
        else if (_max < -180f)
        {
            _max += 360f;
        }

        return Mathf.Clamp(_angle, _min, _max);
    }

    public static float NormalizeAngle(this float _angle)
    {
        _angle %= 360f;

        while (_angle < 0)
        {
            _angle += 360f;
        }

        return _angle;
    }

    public static float ToClosestAngle(this float _angle)
    {
        if (_angle > 180f)
        {
            _angle -= 360f;
        }
        else if (_angle < -180f)
        {
            _angle += 180f;
        }

        return _angle;
    }

    public static float Sign(this float _value)
    {
        return _value < 0 ? -1f : 1f;
    }

    public static int Sign(this int _value)
    {
        return _value < 0 ? -1 : 1;
    }

    public static float ClampMin(this float _value, float _min)
    {
        return _value < _min ? _min : _value;
    }

    public static int ClampMin(this int _value, int _min)
    {
        return _value < _min ? _min : _value;
    }

    public static float ClampMax(this float _value, float _max)
    {
        return _value > _max ? _max : _value;
    }

    public static int ClampMax(this int _value, int _max)
    {
        return _value > _max ? _max : _value;
    }

    public static int ClampIndex(this int _value, int _itemsCount, bool _loop = true)
    {
        if (_value >= 0 && _value < _itemsCount)
        {
            return _value;
        }

        return _loop
            ? _value < 0 ? _itemsCount - 1 : 0
            : _value < 0 ? 0 : _itemsCount - 1;
    }

    public static int Clamp(this int _value, int _min, int _max)
    {
        return Mathf.Clamp(_value, _min, _max);
    }

    public static float Clamp360(this float _value)
    {
        _value %= 360f;

        if (_value < 0f)
        {
            _value += 360f;
        }

        return _value;
    }

    public static int GreatestCommonDivisor(this int _this, int _other)
    {
        while (_other != 0)
        {
            int _val = _other;
            _other = _this % _other;
            _this = _val;
        }

        return _this;
    }

    public static int LeastCommonMultiple(this int _this, int _other)
    {
        return (_this / LeastCommonMultiple(_this, _other)) * _other;
    }

    public static Vector3 Average(this Vector3[] _collection)
    {
        int _size = _collection.Length;
        Vector3 _sum = Vector3.zero;

        if (_size <= 0)
        {
            return _sum;
        }

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return _sum / _size;
    }

    public static Vector3 Average(this List<Vector3> _collection)
    {
        int _size = _collection.Count;
        Vector3 _sum = Vector3.zero;

        if (_size <= 0)
        {
            return _sum;
        }

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return _sum / _size;
    }

    public static Vector3Int Average(this Vector3Int[] _collection)
    {
        int _size = _collection.Length;

        if (_size <= 0)
        {
            return Vector3Int.zero;
        }

        Vector3 _sum = Vector3.zero;

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return (_sum / _size).ToNearestInt();
    }

    public static Vector3Int Average(this List<Vector3Int> _collection)
    {
        int _size = _collection.Count;

        if (_size <= 0)
        {
            return Vector3Int.zero;
        }

        Vector3 _sum = Vector3.zero;

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return (_sum / _size).ToNearestInt();
    }

    public static Vector2 Average(this Vector2[] _collection)
    {
        int _size = _collection.Length;

        if (_size <= 0)
        {
            return Vector2.zero;
        }

        Vector2 _sum = Vector2.zero;

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return _sum / _size;
    }

    public static int Average(this int[] _collection)
    {
        int _size = _collection.Length;

        if (_size <= 0)
        {
            return 0;
        }

        float _sum = 0;

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return (_sum / _size).RoundToInt();
    }

    public static int Average(this List<int> _collection)
    {
        int _size = _collection.Count;

        if (_size <= 0)
        {
            return 0;
        }

        float _sum = 0;

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return (_sum / _size).RoundToInt();
    }

    public static float Average(this float[] _collection)
    {
        int _size = _collection.Length;

        if (_size <= 0)
        {
            return 0f;
        }

        float _sum = 0f;

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return _sum / _size;
    }

    public static float Average(this List<float> _collection)
    {
        int _size = _collection.Count;

        if (_size <= 0)
        {
            return 0f;
        }

        float _sum = 0f;

        for (int i = 0; i < _size; i++)
        {
            _sum += _collection[i];
        }

        return _sum / _size;
    }
}

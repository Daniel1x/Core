using DL.Structs;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 _vector3, float _value)
    {
        _vector3.x = _value;
        return _vector3;
    }

    public static Vector3 WithY(this Vector3 _vector3, float _value)
    {
        _vector3.y = _value;
        return _vector3;
    }

    public static Vector3 WithZ(this Vector3 _vector3, float _value)
    {
        _vector3.z = _value;
        return _vector3;
    }

    public static Vector2 WithX(this Vector2 _vector2, float _value)
    {
        _vector2.x = _value;
        return _vector2;
    }

    public static Vector2 WithY(this Vector2 _vector2, float _value)
    {
        _vector2.y = _value;
        return _vector2;
    }

    public static Vector3Int WithX(this Vector3Int _vector3, int _value)
    {
        _vector3.x = _value;
        return _vector3;
    }

    public static Vector3Int WithY(this Vector3Int _vector3, int _value)
    {
        _vector3.y = _value;
        return _vector3;
    }

    public static Vector3Int WithZ(this Vector3Int _vector3, int _value)
    {
        _vector3.z = _value;
        return _vector3;
    }

    public static Vector2Int WithX(this Vector2Int _vector2, int _value)
    {
        _vector2.x = _value;
        return _vector2;
    }

    public static Vector2Int WithY(this Vector2Int _vector2, int _value)
    {
        _vector2.y = _value;
        return _vector2;
    }

    public static Vector3 XZ(this Vector2 _vector)
    {
        return new Vector3(_vector.x, 0f, _vector.y);
    }

    public static float Area(this Vector2 _vector)
    {
        return Mathf.Abs(_vector.x * _vector.y);
    }

    public static float Volume(this Vector3 _vector)
    {
        return Mathf.Abs(_vector.x * _vector.y * _vector.z);
    }

    public static int Area(this Vector2Int _vector)
    {
        return Mathf.Abs(_vector.x * _vector.y);
    }

    public static int Volume(this Vector3Int _vector)
    {
        return Mathf.Abs(_vector.x * _vector.y * _vector.z);
    }

    public static Vector2 Clamp(this Vector2 _value, Vector2 _min, Vector2 _max)
    {
        return new Vector2(Mathf.Clamp(_value.x, _min.x, _max.x),
                           Mathf.Clamp(_value.y, _min.y, _max.y));
    }

    public static Vector3 Clamp(this Vector3 _value, Vector3 _min, Vector3 _max)
    {
        return new Vector3(Mathf.Clamp(_value.x, _min.x, _max.x),
                           Mathf.Clamp(_value.y, _min.y, _max.y),
                           Mathf.Clamp(_value.z, _min.z, _max.z));
    }

    public static Vector4 Clamp(this Vector4 _value, Vector4 _min, Vector4 _max)
    {
        return new Vector4(Mathf.Clamp(_value.x, _min.x, _max.x),
                           Mathf.Clamp(_value.y, _min.y, _max.y),
                           Mathf.Clamp(_value.z, _min.z, _max.z),
                           Mathf.Clamp(_value.w, _min.w, _max.w));
    }

    public static Vector3 Clamp360(this Vector3 _value)
    {
        return new Vector3(_value.x.Clamp360(),
                           _value.y.Clamp360(),
                           _value.z.Clamp360());
    }

    public static Vector2Int Clamp(this Vector2Int _value, Vector2Int _min, Vector2Int _max)
    {
        return new Vector2Int(Mathf.Clamp(_value.x, _min.x, _max.x),
                              Mathf.Clamp(_value.y, _min.y, _max.y));
    }

    public static Vector3Int Clamp(this Vector3Int _value, Vector3Int _min, Vector3Int _max)
    {
        return new Vector3Int(Mathf.Clamp(_value.x, _min.x, _max.x),
                              Mathf.Clamp(_value.y, _min.y, _max.y),
                              Mathf.Clamp(_value.z, _min.z, _max.z));
    }

    public static Vector2 ClampMin(this Vector2 _value, Vector2 _min)
    {
        return new Vector2
        {
            x = _value.x.ClampMin(_min.x),
            y = _value.y.ClampMin(_min.y),
        };
    }

    public static Vector3 ClampMin(this Vector3 _value, Vector3 _min)
    {
        return new Vector3
        {
            x = _value.x.ClampMin(_min.x),
            y = _value.y.ClampMin(_min.y),
            z = _value.z.ClampMin(_min.z)
        };
    }

    public static Vector2Int ClampMin(this Vector2Int _value, Vector2Int _min)
    {
        return new Vector2Int
        {
            x = _value.x.ClampMin(_min.x),
            y = _value.y.ClampMin(_min.y),
        };
    }

    public static Vector3Int ClampMin(this Vector3Int _value, Vector3Int _min)
    {
        return new Vector3Int
        {
            x = _value.x.ClampMin(_min.x),
            y = _value.y.ClampMin(_min.y),
            z = _value.z.ClampMin(_min.z)
        };
    }

    public static Vector2 ClampMax(this Vector2 _value, Vector2 _max)
    {
        return new Vector2
        {
            x = _value.x.ClampMax(_max.x),
            y = _value.y.ClampMax(_max.y),
        };
    }

    public static Vector3 ClampMax(this Vector3 _value, Vector3 _max)
    {
        return new Vector3
        {
            x = _value.x.ClampMax(_max.x),
            y = _value.y.ClampMax(_max.y),
            z = _value.z.ClampMax(_max.z)
        };
    }

    public static Vector2Int ClampMax(this Vector2Int _value, Vector2Int _max)
    {
        return new Vector2Int
        {
            x = _value.x.ClampMax(_max.x),
            y = _value.y.ClampMax(_max.y)
        };
    }

    public static Vector3Int ClampMax(this Vector3Int _value, Vector3Int _max)
    {
        return new Vector3Int
        {
            x = _value.x.ClampMax(_max.x),
            y = _value.y.ClampMax(_max.y),
            z = _value.z.ClampMax(_max.z)
        };
    }

    public static Vector2 Rotate(this Vector2 _value, float _angle, bool _isAngleInRadians = false)
    {
        if (_isAngleInRadians == false)
        {
            _angle *= Mathf.Deg2Rad;
        }

        float _sinAngle = Mathf.Sin(_angle);
        float _cosAngle = Mathf.Cos(_angle);

        return new Vector2(_value.x * _cosAngle - _value.y * _sinAngle,
                           _value.x * _sinAngle + _value.y * _cosAngle);
    }

    public static Vector3 RotateArountPivot(this Vector3 _value, in Quaternion _rotation, in Vector3 _pivot)
    {
        return (_rotation * (_value - _pivot)) + _pivot;
    }

    public static Vector2 RotateUsingQuaternion(this Vector2 _vector, float _degrees)
    {
        return Quaternion.Euler(0, 0, _degrees) * _vector;
    }

    public static float DistanceTo(this Vector3 _value, Vector3 _target)
    {
        return Vector3.Distance(_value, _target);
    }

    public static float DistanceTo(this Vector3 _value, Transform _target)
    {
        return Vector3.Distance(_value, _target.position);
    }

    public static float DistanceTo(this Vector3 _value, GameObject _target)
    {
        return Vector3.Distance(_value, _target.transform.position);
    }

    public static float DistanceTo<T>(this Vector3 _value, T _target) where T : Component
    {
        return Vector3.Distance(_value, _target.transform.position);
    }

    public static float DistanceTo(this Transform _transform, Vector3 _target)
    {
        return Vector3.Distance(_transform.position, _target);
    }

    public static float DistanceTo(this Transform _transform, Transform _target)
    {
        return Vector3.Distance(_transform.position, _target.position);
    }

    public static float DistanceTo(this Transform _transform, GameObject _target)
    {
        return Vector3.Distance(_transform.position, _target.transform.position);
    }

    public static float DistanceTo<T>(this Transform _transform, T _target) where T : Component
    {
        return Vector3.Distance(_transform.position, _target.transform.position);
    }

    public static float DistanceTo(this GameObject _gameObject, Vector3 _target)
    {
        return Vector3.Distance(_gameObject.transform.position, _target);
    }

    public static float DistanceTo(this GameObject _gameObject, Transform _target)
    {
        return Vector3.Distance(_gameObject.transform.position, _target.position);
    }

    public static float DistanceTo(this GameObject _gameObject, GameObject _target)
    {
        return Vector3.Distance(_gameObject.transform.position, _target.transform.position);
    }

    public static float DistanceTo<T>(this GameObject _gameObject, T _target) where T : Component
    {
        return Vector3.Distance(_gameObject.transform.position, _target.transform.position);
    }

    public static float DistanceTo<T>(this T _component, Vector3 _target) where T : Component
    {
        return Vector3.Distance(_component.transform.position, _target);
    }

    public static float DistanceTo<T>(this T _component, Transform _target) where T : Component
    {
        return Vector3.Distance(_component.transform.position, _target.position);
    }

    public static float DistanceTo<T>(this T _component, GameObject _target) where T : Component
    {
        return Vector3.Distance(_component.transform.position, _target.transform.position);
    }

    public static float DistanceTo<T1, T2>(this T1 _value, T2 _target) where T1 : Component where T2 : Component
    {
        return Vector3.Distance(_value.transform.position, _target.transform.position);
    }

    public static float SqrDistanceTo(this Vector3 _value, Vector3 _target)
    {
        return (_target - _value).sqrMagnitude;
    }

    public static float SqrDistanceTo(this Vector3 _value, Transform _target)
    {
        return (_target.position - _value).sqrMagnitude;
    }

    public static float SqrDistanceTo(this Vector3 _value, GameObject _target)
    {
        return (_target.transform.position - _value).sqrMagnitude;
    }

    public static float SqrDistanceTo<T>(this Vector3 _value, T _target) where T : Component
    {
        return (_target.transform.position - _value).sqrMagnitude;
    }

    public static float SqrDistanceTo(this Transform _transform, Vector3 _target)
    {
        return (_target - _transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo(this Transform _transform, Transform _target)
    {
        return (_target.position - _transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo(this Transform _transform, GameObject _target)
    {
        return (_target.transform.position - _transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo<T>(this Transform _transform, T _target) where T : Component
    {
        return (_target.transform.position - _transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo(this GameObject _gameObject, Vector3 _target)
    {
        return (_target - _gameObject.transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo(this GameObject _gameObject, Transform _target)
    {
        return (_target.position - _gameObject.transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo(this GameObject _gameObject, GameObject _target)
    {
        return (_target.transform.position - _gameObject.transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo<T>(this GameObject _gameObject, T _target) where T : Component
    {
        return (_target.transform.position - _gameObject.transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo<T>(this T _component, Vector3 _target) where T : Component
    {
        return (_target - _component.transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo<T>(this T _component, Transform _target) where T : Component
    {
        return (_target.position - _component.transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo<T>(this T _component, GameObject _target) where T : Component
    {
        return (_target.transform.position - _component.transform.position).sqrMagnitude;
    }

    public static float SqrDistanceTo<T1, T2>(this T1 _component, T2 _target) where T1 : Component where T2 : Component
    {
        return (_target.transform.position - _component.transform.position).sqrMagnitude;
    }

    public static Vector2Int ToNearestInt(this Vector2 _value)
    {
        return new Vector2Int(Mathf.RoundToInt(_value.x), Mathf.RoundToInt(_value.y));
    }

    public static Vector3Int ToNearestInt(this Vector3 _value)
    {
        return new Vector3Int(Mathf.RoundToInt(_value.x), Mathf.RoundToInt(_value.y), Mathf.RoundToInt(_value.z));
    }

    public static Vector2Int ToInt(this Vector2 _value)
    {
        return new Vector2Int((int)_value.x, (int)_value.y);
    }

    public static Vector3Int ToInt(this Vector3 _value)
    {
        return new Vector3Int((int)_value.x, (int)_value.y, (int)_value.z);
    }

    public static Vector2 ToFloat(this Vector2Int _value)
    {
        return new Vector2(_value.x, _value.y);
    }

    public static Vector3 ToFloat(this Vector3Int _value)
    {
        return new Vector3(_value.x, _value.y, _value.z);
    }

    public static Vector3 DirectionTo(this Vector3 _value, Vector3 _target)
    {
        return _target - _value;
    }

    public static Vector3 DirectionTo(this Vector3 _value, Transform _target)
    {
        return DirectionTo(_value, _target.position);
    }

    public static Vector3 DirectionTo(this Vector3 _value, GameObject _target)
    {
        return DirectionTo(_value, _target.transform.position);
    }

    public static Vector3 DirectionTo<T>(this Vector3 _value, T _target) where T : Component
    {
        return DirectionTo(_value, _target.transform.position);
    }

    public static Vector3 DirectionTo(this Transform _transform, Vector3 _target)
    {
        return DirectionTo(_transform.position, _target);
    }

    public static Vector3 DirectionTo(this Transform _transform, Transform _target)
    {
        return DirectionTo(_transform.position, _target.position);
    }

    public static Vector3 DirectionTo(this Transform _transform, GameObject _target)
    {
        return DirectionTo(_transform.position, _target.transform.position);
    }

    public static Vector3 DirectionTo<T>(this Transform _transform, T _target) where T : Component
    {
        return DirectionTo(_transform.position, _target.transform.position);
    }

    public static Vector3 DirectionTo(this GameObject _gameObject, Vector3 _target)
    {
        return DirectionTo(_gameObject.transform.position, _target);
    }

    public static Vector3 DirectionTo(this GameObject _gameObject, Transform _target)
    {
        return DirectionTo(_gameObject.transform.position, _target.position);
    }

    public static Vector3 DirectionTo(this GameObject _gameObject, GameObject _target)
    {
        return DirectionTo(_gameObject.transform.position, _target.transform.position);
    }

    public static Vector3 DirectionTo<T>(this GameObject _gameObject, T _target) where T : Component
    {
        return DirectionTo(_gameObject.transform.position, _target.transform.position);
    }

    public static Vector3 DirectionTo<T>(this T _component, Vector3 _target) where T : Component
    {
        return DirectionTo(_component.transform.position, _target);
    }

    public static Vector3 DirectionTo<T>(this T _component, Transform _target) where T : Component
    {
        return DirectionTo(_component.transform.position, _target.position);
    }

    public static Vector3 DirectionTo<T>(this T _component, GameObject _target) where T : Component
    {
        return DirectionTo(_component.transform.position, _target.transform.position);
    }

    public static Vector3 DirectionTo<T1, T2>(this T1 _value, T2 _target) where T1 : Component where T2 : Component
    {
        return DirectionTo(_value.transform.position, _target.transform.position);
    }

    public static float GetMaxValue(this Vector2 _vector)
    {
        return Mathf.Max(_vector.x, _vector.y);
    }

    public static int GetMaxValue(this Vector2Int _vector)
    {
        return Mathf.Max(_vector.x, _vector.y);
    }

    public static float GetMaxValue(this Vector3 _vector)
    {
        return Mathf.Max(_vector.x, _vector.y, _vector.z);
    }

    public static int GetMaxValue(this Vector3Int _vector)
    {
        return Mathf.Max(_vector.x, _vector.y, _vector.z);
    }

    public static float GetMaxValue(this Vector4 _vector)
    {
        return Mathf.Max(_vector.x, _vector.y, _vector.z, _vector.w);
    }

    public static float GetMinValue(this Vector2 _vector)
    {
        return Mathf.Min(_vector.x, _vector.y);
    }

    public static int GetMinValue(this Vector2Int _vector)
    {
        return Mathf.Min(_vector.x, _vector.y);
    }

    public static float GetMinValue(this Vector3 _vector)
    {
        return Mathf.Min(_vector.x, _vector.y, _vector.z);
    }

    public static int GetMinValue(this Vector3Int _vector)
    {
        return Mathf.Min(_vector.x, _vector.y, _vector.z);
    }

    public static float GetMinValue(this Vector4 _vector)
    {
        return Mathf.Min(_vector.x, _vector.y, _vector.z, _vector.w);
    }

    public static float GetMaxAbsValue(this Vector2 _vector)
    {
        return _vector.Abs().GetMaxValue();
    }

    public static int GetMaxAbsValue(this Vector2Int _vector)
    {
        return _vector.Abs().GetMaxValue();
    }

    public static float GetMaxAbsValue(this Vector3 _vector)
    {
        return _vector.Abs().GetMaxValue();
    }

    public static int GetMaxAbsValue(this Vector3Int _vector)
    {
        return _vector.Abs().GetMaxValue();
    }

    public static float GetMaxAbsValue(this Vector4 _vector)
    {
        return _vector.Abs().GetMaxValue();
    }

    public static float GetMinAbsValue(this Vector2 _vector)
    {
        return _vector.Abs().GetMinValue();
    }

    public static int GetMinAbsValue(this Vector2Int _vector)
    {
        return _vector.Abs().GetMinValue();
    }

    public static float GetMinAbsValue(this Vector3 _vector)
    {
        return _vector.Abs().GetMinValue();
    }

    public static int GetMinAbsValue(this Vector3Int _vector)
    {
        return _vector.Abs().GetMinValue();
    }

    public static float GetMinAbsValue(this Vector4 _vector)
    {
        return _vector.Abs().GetMinValue();
    }

    public static float GetSumOfValues(this Vector3 _vector)
    {
        return _vector.x + _vector.y + _vector.z;
    }

    public static Vector2 MultiplyComponentwise(this Vector2 _vector, Vector2 _multiplier)
    {
        return new Vector2(_vector.x * _multiplier.x, _vector.y * _multiplier.y);
    }

    public static Vector2Int MultiplyComponentwise(this Vector2Int _vector, Vector2Int _multiplier)
    {
        return new Vector2Int(_vector.x * _multiplier.x, _vector.y * _multiplier.y);
    }
    public static Vector3 MultiplyComponentwise(this Vector3 _vector, Vector3 _multiplier)
    {
        return new Vector3(_vector.x * _multiplier.x, _vector.y * _multiplier.y, _vector.z * _multiplier.z);
    }

    public static Vector3Int MultiplyComponentwise(this Vector3Int _vector, Vector3Int _multiplier)
    {
        return new Vector3Int(_vector.x * _multiplier.x, _vector.y * _multiplier.y, _vector.z * _multiplier.z);
    }

    public static Vector4 MultiplyComponentWise(this Vector4 _vector, Vector4 _multiplier)
    {
        return new Vector4(_vector.x * _multiplier.x, _vector.y * _multiplier.y, _vector.z * _multiplier.z, _vector.w * _multiplier.w);
    }

    public static Vector2 Abs(this Vector2 _v)
    {
        return new Vector2(Mathf.Abs(_v.x), Mathf.Abs(_v.y));
    }

    public static Vector2Int Abs(this Vector2Int _v)
    {
        return new Vector2Int(Mathf.Abs(_v.x), Mathf.Abs(_v.y));
    }

    public static Vector3 Abs(this Vector3 _v)
    {
        return new Vector3(Mathf.Abs(_v.x), Mathf.Abs(_v.y), Mathf.Abs(_v.z));
    }

    public static Vector3Int Abs(this Vector3Int _v)
    {
        return new Vector3Int(Mathf.Abs(_v.x), Mathf.Abs(_v.y), Mathf.Abs(_v.z));
    }

    public static Vector4 Abs(this Vector4 _v)
    {
        return new Vector4(Mathf.Abs(_v.x), Mathf.Abs(_v.y), Mathf.Abs(_v.z), Mathf.Abs(_v.w));
    }

    public static Vector2 Expand(this Vector2 _v, MinMax _xOccupiedRange, MinMax _yOccupiedRange)
    {
        return new Vector2(_v.x.expandValue(_xOccupiedRange), _v.y.expandValue(_yOccupiedRange));
    }

    public static Vector3 Expand(this Vector3 _v, MinMax _xOccupiedRange, MinMax _yOccupiedRange, MinMax _zOccupiedRange)
    {
        return new Vector3(_v.x.expandValue(_xOccupiedRange), _v.y.expandValue(_yOccupiedRange), _v.z.expandValue(_zOccupiedRange));
    }

    private static float expandValue(this float _value, MinMax _occupiedRange) => _value.expandValue(_occupiedRange.Range);
    private static float expandValue(this float _value, float _occupiedRange) => _occupiedRange == 0f ? 0f : _value / _occupiedRange;

    public static Vector2 NormalizedByComponents(this Vector2 _v)
    {
        float _max = _v.GetMaxAbsValue();
        return _max == 0f ? default : _v / _max;
    }

    public static Vector3 NormalizedByComponents(this Vector3 _v)
    {
        float _max = _v.GetMaxAbsValue();
        return _max == 0f ? default : _v / _max;
    }

    public static Vector4 NormalizedByComponents(this Vector4 _v)
    {
        float _max = _v.GetMaxAbsValue();
        return _max == 0f ? default : _v / _max;
    }

    public static int GetGridDistance(this Vector2Int _from, Vector2Int _to)
    {
        return Mathf.Abs(_from.x - _to.x)
            + Mathf.Abs(_from.y - _to.y);
    }

    public static int GetGridDistance(this Vector3Int _from, Vector3Int _to)
    {
        return Mathf.Abs(_from.x - _to.x)
            + Mathf.Abs(_from.y - _to.y)
            + Mathf.Abs(_from.z - _to.z);
    }
}

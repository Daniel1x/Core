namespace DL.Structs
{
    using UnityEngine;

    [System.Serializable]
    public struct MinMax
    {
        public float Min;
        public float Max;

        public float RandomValue => UnityEngine.Random.Range(Min, Max);
        public float Range => Max - Min;
        public float Middle => (Min + Max) * 0.5f;

        public MinMax(float _min, float _max)
        {
            Min = _min;
            Max = _max;
        }

        public bool FitsInRange(float _value)
        {
            return _value >= Min
                && _value <= Max;
        }

        public float GetValueFromRange(float _normalizedPercentage01)
        {
            return Min + (Range * Mathf.Clamp01(_normalizedPercentage01));
        }

        public float Clamp(float _value)
        {
            return Mathf.Clamp(_value, Min, Max);
        }

        public float Normalize(float _value)
        {
            return Range == 0f
                ? 0f
                : (Clamp(_value) - Min) / Range;
        }

        public void TryExpandMinRange(float _min)
        {
            if (_min < Min)
            {
                Min = _min;
            }
        }

        public void TryExpandMaxRange(float _max)
        {
            if (_max > Max)
            {
                Max = _max;
            }
        }

        public void TryExpandMinOrMaxRange(float _value)
        {
            TryExpandMinRange(_value);
            TryExpandMaxRange(_value);
        }

        public static MinMax Lerp(MinMax _form, MinMax _to, float _t)
        {
            return new MinMax()
            {
                Min = Mathf.Lerp(_form.Min, _to.Min, _t),
                Max = Mathf.Lerp(_form.Max, _to.Max, _t),
            };
        }

        public static bool operator ==(MinMax _this, MinMax _other)
        {
            return _this.Min == _other.Min
                && _this.Max == _other.Max;
        }

        public static bool operator !=(MinMax _this, MinMax _other)
        {
            return _this.Min != _other.Min
                || _this.Max != _other.Max;
        }

        public override bool Equals(object _obj)
        {
            return _obj is MinMax _minMax
                && Min == _minMax.Min
                && Max == _minMax.Max;
        }

        public override int GetHashCode() => System.HashCode.Combine(Min, Max);
        public override string ToString() => $"({Min}, {Max})";
    }
}

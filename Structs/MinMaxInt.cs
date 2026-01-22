namespace DL.Structs
{
    using UnityEngine;

    [System.Serializable]
    public struct MinMaxInt
    {
        public int Min;
        public int Max;

        public int RandomValue => UnityEngine.Random.Range(Min, Max + 1);
        public int Range => Max - Min;
        public int Middle => (Min + Max) / 2;

        public MinMaxInt(int _min, int _max)
        {
            Min = _min;
            Max = _max;
        }

        public bool FitsInRange(int _value)
        {
            return _value >= Min
                && _value <= Max;
        }

        public int GetValueFromRange(float _normalizedPercentage01)
        {
            return Mathf.RoundToInt(Min + (Range * Mathf.Clamp01(_normalizedPercentage01)));
        }

        public int Clamp(int _value)
        {
            return Mathf.Clamp(_value, Min, Max);
        }

        public float Normalize(int _value)
        {
            return Range == 0f
                ? 0f
                : (Clamp(_value) - Min) / (float)Range;
        }

        public void TryExpandMinRange(int _min)
        {
            if (_min < Min)
            {
                Min = _min;
            }
        }

        public void TryExpandMaxRange(int _max)
        {
            if (_max > Max)
            {
                Max = _max;
            }
        }

        public void TryExpandMinOrMaxRange(int _value)
        {
            TryExpandMinRange(_value);
            TryExpandMaxRange(_value);
        }

        public static MinMaxInt Lerp(MinMaxInt _form, MinMaxInt _to, float _t)
        {
            return new MinMaxInt
            {
                Min = Mathf.Lerp(_form.Min, _to.Min, _t).RoundToInt(),
                Max = Mathf.Lerp(_form.Max, _to.Max, _t).RoundToInt()
            };
        }

        public static bool operator ==(MinMaxInt _this, MinMaxInt _other)
        {
            return _this.Min == _other.Min
                && _this.Max == _other.Max;
        }

        public static bool operator !=(MinMaxInt _this, MinMaxInt _other)
        {
            return _this.Min != _other.Min
                || _this.Max != _other.Max;
        }

        public override bool Equals(object _obj)
        {
            return _obj is MinMaxInt _minMax
                && Min == _minMax.Min
                && Max == _minMax.Max;
        }

        public override int GetHashCode() => System.HashCode.Combine(Min, Max);
        public override string ToString() => $"({Min}, {Max})";
    }
}

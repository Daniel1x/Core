namespace DL.Core.Maze
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class MazeExtensions
    {
        public static T GetRandom<T>(this List<T> _list)
        {
            if (_list == null || _list.Count == 0)
            {
                throw new System.ArgumentException("List cannot be null or empty.", nameof(_list));
            }

            int _randomIndex = Random.Range(0, _list.Count);
            return _list[_randomIndex];
        }

        public static T GetRandom<T>(this T[] _array)
        {
            if (_array == null || _array.Length == 0)
            {
                throw new System.ArgumentException("Array cannot be null or empty.", nameof(_array));
            }

            int _randomIndex = Random.Range(0, _array.Length);
            return _array[_randomIndex];
        }

        public static Color WithAlpha(this Color _color, float _alpha)
        {
            _color.a = _alpha;
            return _color;
        }

        public static int GetRandomID<T>(this List<T> _list)
        {
            if (_list == null || _list.Count == 0)
            {
                throw new System.ArgumentException("List cannot be null or empty.", nameof(_list));
            }

            return Random.Range(0, _list.Count);
        }

        public static int GetRandomID<T>(this T[] _array)
        {
            if (_array == null || _array.Length == 0)
            {
                throw new System.ArgumentException("Array cannot be null or empty.", nameof(_array));
            }

            return Random.Range(0, _array.Length);
        }

        public static float RoundToClosestDecimalPlace(this float _value, int _decimalPlaces)
        {
            float _multiplier = Mathf.Pow(10f, _decimalPlaces);
            return Mathf.Round(_value * _multiplier) / _multiplier;
        }
    }
}

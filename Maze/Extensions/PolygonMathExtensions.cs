namespace DL.Core.Maze
{
    using UnityEngine;

    public static class PolygonMathExtensions
    {
        public static float CalculateSideLengthFromOuterRadius(int _vertexCount, float _outerRadius)
        {
            return _outerRadius * 2f * Mathf.Sin(Mathf.PI / _vertexCount);
        }

        public static float CalculateInnerRadiusFromSideLength(int _vertexCount, float _sideLength)
        {
            return _sideLength / (2f * Mathf.Tan(Mathf.PI / _vertexCount));
        }

        public static float CalculateOuterRadiusFromSideLength(int _vertexCount, float _sideLength)
        {
            return _sideLength / (2f * Mathf.Sin(Mathf.PI / _vertexCount));
        }

        public static Vector2 CalculateDistanceToNeighborCenter(int _vertexCount, float _innerRadius, float _angleOffset = 0f)
        {
            float _distance = 2f * _innerRadius;
            float _angleStep = 360f / _vertexCount;
            _angleStep *= 0.5f * Mathf.Deg2Rad;

            if (_angleOffset != 0f)
            {
                _angleStep += _angleOffset * Mathf.Deg2Rad;
            }

            return new Vector2(_distance * Mathf.Cos(_angleStep), _distance * Mathf.Sin(_angleStep));
        }

        public static Vector2 CalculatePointOnOuterRadius(float _outerRadius, float _angleDeg)
        {
            float _angleRad = _angleDeg * Mathf.Deg2Rad;
            return new Vector2(_outerRadius * Mathf.Cos(_angleRad), _outerRadius * Mathf.Sin(_angleRad));
        }

        public static float CalculateDistanceBetweenVertices(float _outerRadius, float _angleDegrees)
        {
            return Mathf.Sqrt(2f * _outerRadius * _outerRadius * (1 - Mathf.Cos(_angleDegrees * Mathf.Deg2Rad)));
        }

        public static Vector2 CalculateVertexOffset(float _outerRadius, float _angleDegrees)
        {
            float _radian = _angleDegrees * Mathf.Deg2Rad;
            float _x = _outerRadius * Mathf.Cos(_radian);
            float _y = _outerRadius * Mathf.Sin(_radian);
            return new Vector2(_x, _y);
        }
    }
}

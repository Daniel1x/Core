namespace DL.Core.Maze
{
    using UnityEngine;

    public static class GridExtensions
    {
        public static int GetGridDistance(this Vector2Int _from, Vector2Int _to)
        {
            return Mathf.Abs(_from.x - _to.x)
                + Mathf.Abs(_from.y - _to.y);
        }

        public static bool IsValidGridPosition(this Vector2Int _position, Vector2Int _gridSize)
        {
            return _position.x >= 0 && _position.x < _gridSize.x
                && _position.y >= 0 && _position.y < _gridSize.y;
        }

        public static int GetGridIndex(this Vector2Int _position, int _gridWidth)
        {
            return _position.y * _gridWidth + _position.x;
        }

        public static Vector2Int GetRandomPointOnGrid(this Vector2Int _gridSize)
        {
            int _x = Random.Range(0, _gridSize.x);
            int _y = Random.Range(0, _gridSize.y);
            return new Vector2Int(_x, _y);
        }

        public static int Area(this Vector2Int _size)
        {
            return _size.x * _size.y;
        }
    }
}

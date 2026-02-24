namespace DL.Core.Maze
{
    using UnityEngine;

    [System.Serializable]
    public struct ShapeOffset
    {
        public Vector2 Position;
        public float Rotation;

        public ShapeOffset(float _posX, float _posY, float _rotation = 0f)
        {
            Position = new Vector2(_posX, _posY);
            Rotation = _rotation;
        }

        public ShapeOffset(Vector2 _position = default, float _rotation = 0f)
        {
            Position = _position;
            Rotation = _rotation;
        }

        public static implicit operator ShapeOffset(Vector2 _offset) => new ShapeOffset(_offset);
    }
}

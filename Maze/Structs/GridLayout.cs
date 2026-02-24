namespace DL.Core.Maze
{
    using UnityEngine;

    public readonly struct GridLayout
    {
        public readonly Vector2 Position;
        public readonly float Rotation;

        public GridLayout(Vector2 position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}

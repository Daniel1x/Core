namespace DL.Core.Maze
{
    using UnityEngine;

    public interface IGrid
    {
        public Vector2Int Size { get; }
        public int NodeCount { get; }
        public IGridType GridType { get; }

        public GridNode GetNode(Vector2Int _gridPosition);
    }

    public interface IMazeGrid
    {
        public event System.Action<IGrid> OnMazeGenerationComplete;
        public void ResetGridWalls();
        public void NotifyMazeGenerationComplete();
    }
}

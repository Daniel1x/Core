namespace DL.Core.Maze
{
    using System.Collections.Generic;
    using UnityEngine;

    public interface IGridContainer
    {
        public MazeGrid Grid { get; }
    }

    public interface IGridCreator : IGridContainer
    {
        public event System.Action<MazeGrid> OnGridCreated;
        public void SpawnNewGrid();
    }

    [System.Serializable]
    public sealed class MazeGrid : IGrid, IMazeGrid
    {
        public event System.Action<IGrid> OnMazeGenerationComplete;

        public const int k_MaxSupportedNeighbors = 10;
        private static readonly Vector2Int[] neighborPositionBuffer = new Vector2Int[k_MaxSupportedNeighbors];

        [SerializeField] private Vector2Int size;
        [SerializeField] private int height;
        [SerializeField] private int width;
        [SerializeField] private int nodeCount;
        [SerializeField] private GridNode[] nodes;

        public Vector2Int Size => size;
        public int NodeCount => nodeCount;

        public IGridType GridType { get; private set; }

        public MazeGrid(Vector2Int _size, IGridType _gridType)
        {
            if (_gridType == null)
            {
                throw new System.ArgumentNullException(nameof(_gridType), "IGridType cannot be null.");
            }

            if (_size.x <= 0 || _size.y <= 0)
            {
                throw new System.ArgumentException("Grid size dimensions must be greater than zero.", nameof(_size));
            }

            size = _size;
            height = size.y;
            width = size.x;
            nodeCount = size.Area();
            nodes = new GridNode[nodeCount];
            GridType = _gridType;

            // Populate the grid with items
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    GridNode _node = new GridNode(x, y, width);
                    nodes[_node.ID] = _node;
                }
            }

            // Calculate neighbors for each grid item
            for (int i = 0; i < nodes.Length; i++)
            {
                GridNode _currentItem = nodes[i];
                int _validNeighborCount = _gridType.Topology.GetNeighborsPositionsNonAlloc(_currentItem.Position, neighborPositionBuffer);

                if (_validNeighborCount <= 0)
                {
                    continue;
                }

                List<GridNode> _neighbors = new List<GridNode>(_validNeighborCount);

                for (int n = 0; n < _validNeighborCount; n++)
                {
                    Vector2Int _neighborPosition = neighborPositionBuffer[n];

                    if (!_neighborPosition.IsValidGridPosition(size))
                    {
                        continue;
                    }

                    _neighbors.Add(nodes[_neighborPosition.GetGridIndex(width)]);
                }

                _currentItem.SetNeighbors(_neighbors);
            }
        }

        public GridNode GetNode(int _nodeID)
        {
            return (_nodeID >= 0 && _nodeID < nodeCount)
                ? nodes[_nodeID]
                : null;
        }

        public GridNode GetNode(Vector2Int _gridPosition)
        {
            return _gridPosition.IsValidGridPosition(size)
                ? nodes[_gridPosition.GetGridIndex(width)]
                : null;
        }

        public void ResetGridWalls()
        {
            if (nodes == null)
            {
                throw new System.InvalidOperationException("Grid items have not been initialized.");
            }

            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i].ResetAllWalls();
            }
        }

        public void NotifyMazeGenerationComplete()
        {
            if (nodes != null)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i] != null)
                    {
                        nodes[i].UpdateAvailableNeighbors();
                    }
                }
            }

            OnMazeGenerationComplete?.Invoke(this);
        }
    }
}

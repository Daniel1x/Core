namespace DL.Core.Maze
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public sealed class GridNode 
    {
        public delegate void WallChangedHandler(GridNode _owner, GridNode _neighbor, bool _hasWall);

        /// <summary> (Owner, Neighbor, HasWall) </summary>
        public event WallChangedHandler OnWallChanged;

        [SerializeField] private int id;
        [SerializeField] private Vector2Int position;

        private List<GridNode> neighbors = null;
        private List<GridNode> availableNeighbors = null;
        private Dictionary<GridNode, bool> walls = null;

        public int ID => id;
        public Vector2Int Position => position;
        public IReadOnlyList<GridNode> Neighbors => neighbors;
        public IReadOnlyList<GridNode> AvailableNeighbors => availableNeighbors;

        public GridNode(int _x, int _y, int _gridWidth)
        {
            position = new Vector2Int(_x, _y);
            id = Position.GetGridIndex(_gridWidth);
        }

        public void SetNeighbors(List<GridNode> _neighbors)
        {
            neighbors = _neighbors;
            walls = new Dictionary<GridNode, bool>(neighbors.Count);

            foreach (var _neighbor in neighbors)
            {
                walls[_neighbor] = true; // Initialize all walls as present
            }
        }

        public void UpdateAvailableNeighbors()
        {
            if (neighbors == null || walls == null)
            {
                availableNeighbors = new List<GridNode>();
                return;
            }

            if (availableNeighbors == null)
            {
                availableNeighbors = new List<GridNode>(walls.Count);
            }
            else
            {
                availableNeighbors.Clear();
            }

            foreach (GridNode _neighbor in neighbors)
            {
                if (!walls[_neighbor])
                {
                    availableNeighbors.Add(_neighbor);
                }
            }
        }

        public bool IsNeighbor(GridNode _neighbor)
        {
            return walls.ContainsKey(_neighbor);
        }

        public bool HasWall(GridNode _neighbor)
        {
            if (walls.ContainsKey(_neighbor))
            {
                return walls[_neighbor];
            }

            return false;
        }

        public bool SetWall(GridNode _neighbor, bool _hasWall)
        {
            bool _changedA = setWall(_neighbor, _hasWall);
            bool _changedB = _neighbor.setWall(this, _hasWall);

            return _changedA || _changedB;
        }

        public bool ResetAllWalls(bool _hasWalls = true)
        {
            bool _anyChanged = false;

            foreach (var _neighbor in neighbors)
            {
                if (walls[_neighbor] == _hasWalls)
                {
                    continue;
                }

                walls[_neighbor] = _hasWalls;
                OnWallChanged?.Invoke(this, _neighbor, _hasWalls);
                _anyChanged = true;
            }

            return _anyChanged;
        }

        private bool setWall(GridNode _neighbor, bool _hasWall)
        {
            if (!walls.ContainsKey(_neighbor))
            {
                return false; // Neighbor not found
            }

            if (walls[_neighbor] == _hasWall)
            {
                return false; // No change needed
            }

            walls[_neighbor] = _hasWall;
            OnWallChanged?.Invoke(this, _neighbor, _hasWall);
            return true;
        }
    }
}

namespace DL.Core.Maze
{
    using System;
    using UnityEngine;

    public abstract class GridNodeVisualizationCreator : INodeVisualizationCreator
    {
        protected readonly IGridType gridType;

        protected GridNodeVisualizationCreator(IGridType _gridType)
        {
            gridType = _gridType ?? throw new ArgumentNullException(nameof(_gridType), "Grid type cannot be null.");
        }

        public IPolygonVisualization CreateNewInstance(Transform _parent, GridNode _node, IPolygonPermutationDataProvider _permutationProvider)
        {
            ShapeOffset _offset = gridType.Layout.GetPositionAndRotation(_node.Position);
            IPolygonVisualization _polygon = createNewInstance(_node.Position);
            _polygon.Initialize(_node, gridType, _parent, _offset, _permutationProvider);
            return _polygon;
        }

        public abstract void ReturnVisualizationToPool();

        protected abstract IPolygonVisualization createNewInstance(Vector2Int _position);
    }
}

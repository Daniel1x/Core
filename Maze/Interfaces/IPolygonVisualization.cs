namespace DL.Core.Maze
{
    using UnityEngine;

    public interface IPolygonVisualization : IPolygonShape, IPolygonWallMaskProvider, IPolygonStateSetter, IPolygonRadius
    {
        public GameObject GameObject { get; }
        public GridNode Node { get; }

        void Initialize(GridNode _node, IGridType _gridType, Transform _parent, ShapeOffset _offset, IPolygonPermutationDataProvider _permutationProvider);
        void AdjustToFit(IPolygonRadius _radius);
    }
}

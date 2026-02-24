namespace DL.Core.Maze
{
    using UnityEngine;

    public interface IPolygonVisualization : IPolygonShape, IPolygonWallMaskProvider, IPolygonStateSetter
    {
        public GameObject GameObject { get; }

        void Initialize(GridNode _node, IGridType _gridType, Transform _parent, ShapeOffset _offset, IPolygonPermutationDataProvider _permutationProvider);
        void AdjustToFit(float _outerRadius);
    }
}

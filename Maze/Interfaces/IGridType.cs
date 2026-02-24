namespace DL.Core.Maze
{
    using UnityEngine;

    public interface IGridType : ISelectionKeyContainer<string>,
        IPolygonSizeSetter,
        IGridLayoutProvider,
        IGridTopologyProvider,
        IGridGeometryProvider,
        IGridBoundsProvider,
        INodeVisualizationCreatorProvider,
        IGridNeighborWallVisualizationResolverProvider
    {

    }

    public interface INodeVisualizationCreatorProvider
    {
        INodeVisualizationCreator VisualizationCreator { get; }
    }

    public interface INodeVisualizationCreator
    {
        IPolygonVisualization CreateNewInstance(Transform _parent, GridNode _node, IPolygonPermutationDataProvider _permutationProvider);
        void ReturnVisualizationToPool();
    }

    public interface IGridNeighborWallVisualizationResolverProvider
    {
        IGridNeighborWallVisualizationResolver WallVisualizationResolver { get; }
    }

    public interface IGridNeighborWallVisualizationResolver
    {
        void SetWallFlags(IPolygonVisualization _visualization, GridNode _item, GridNode _neighbor, bool _hasWall);
    }

    public interface IGridLayoutProvider
    {
        IGridLayout Layout { get; }
    }

    public interface IGridLayout
    {
        ShapeOffset GetPositionAndRotation(Vector2Int gridPosition);
        bool IsPointingUp(Vector2Int gridPosition);
    }

    public interface IGridTopologyProvider
    {
        IGridTopology Topology { get; }
    }

    public interface IGridTopology
    {
        int MaxNeighbors { get; }
        int GetNeighborsPositionsNonAlloc(Vector2Int gridPosition, Vector2Int[] outputArray);
    }

    public interface IGridGeometryProvider
    {
        IGridGeometry Geometry { get; }
    }

    public interface IGridGeometry
    {
        IPolygon GetShapeAtPosition(Vector2Int gridPosition);
    }

    public interface IGridBoundsProvider
    {
        IGridBounds Bounds { get; }
    }

    public interface IGridBounds
    {
        Vector2 GetSpaceRequiredToFitTheGrid(Vector2Int gridSize);
        Vector2 GetGridStartOffset(Vector2Int gridSize);
    }
}

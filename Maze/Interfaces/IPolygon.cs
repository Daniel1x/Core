namespace DL.Core.Maze
{
    public interface IPolygon : IPolygonShape, IPolygonRadius, IPolygonSideLength, IPolygonStartRotation
    {

    }

    public interface IPolygonSideLength
    {
        public float SideLength { get; }
    }

    public interface IPolygonStartRotation
    {
        public float StartRotation { get; }
    }

    public interface IPolygonRadius
    {
        public float OuterRadius { get; }
        public float InnerRadius { get; }
        public float RadiusDelta { get; }
    }

    public interface IPolygonSizeSetter
    {
        public void RecalculatePolygonSize(float _outerRadius);
    }

    public interface IPolygonShape
    {
        public int VertexCount { get; }
    }

    public interface IPolygonWallMaskProvider
    {
        public int GetWallMask();
    }

    public interface IPolygonWidth
    {
        public float Width { get; }
    }

    public interface IPolygonHeight
    {
        public float Height { get; }
    }

    public interface IPolygonSpikeHeight
    {
        public float SpikeHeight { get; }
    }
}

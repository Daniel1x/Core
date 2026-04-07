namespace DL.Core.Maze
{
    public interface IPolygonStateSetter
    {
        public void SetState(PolygonVisualizationState _state);
    }

    public enum PolygonVisualizationState
    {
        Default = 0,
        Visited = 1,
        Highlighted = 2,
        Target = 3,
    }

    public static class PolygonStateVisualizationExtensions
    {
        public const string k_StatePropertyName = "_State";
        public readonly static int k_StatePropertyID = UnityEngine.Shader.PropertyToID(k_StatePropertyName);

        public static readonly PolygonVisualizationState[] States = new PolygonVisualizationState[]
        {
            PolygonVisualizationState.Default,
            PolygonVisualizationState.Visited,
            PolygonVisualizationState.Highlighted,
            PolygonVisualizationState.Target,
        };
    }
}

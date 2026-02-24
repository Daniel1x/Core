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
        public const string k_IsDefaultKeyword = "_STATE_DEFAULT";
        public const string k_IsVisitedKeyword = "_STATE_VISITED";
        public const string k_IsHighlightedKeyword = "_STATE_HIGHLIGHTED";
        public const string k_IsTargetKeyword = "_STATE_TARGET";

        public static readonly string[] k_StateKeywords = new string[]
        {
            k_IsDefaultKeyword,
            k_IsVisitedKeyword,
            k_IsHighlightedKeyword,
            k_IsTargetKeyword,
        };

        public static string GetStateKeyword(this PolygonVisualizationState _state)
        {
            return _state switch
            {
                PolygonVisualizationState.Default => k_IsDefaultKeyword,
                PolygonVisualizationState.Visited => k_IsVisitedKeyword,
                PolygonVisualizationState.Highlighted => k_IsHighlightedKeyword,
                PolygonVisualizationState.Target => k_IsTargetKeyword,
                _ => k_IsDefaultKeyword,
            };
        }
    }
}

namespace DL.Core.Maze
{
    public interface IPolygonPermutationDataProvider
    {
        public PolygonPermutationData GetPermutation<T>(T _polygon) where T : IPolygonShape, IPolygonWallMaskProvider;
    }
}

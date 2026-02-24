namespace DL.Core.Maze
{
    public interface IHexagonVisualization
    {
        public bool TopLeftWall { get; set; }
        public bool TopRightWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomRightWall { get; set; }
        public bool BottomLeftWall { get; set; }
        public bool LeftWall { get; set; }
    }
}

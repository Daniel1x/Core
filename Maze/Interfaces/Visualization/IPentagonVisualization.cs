namespace DL.Core.Maze
{
    public interface IPentagonVisualization
    {
        public bool IsPointingUp { get; }
        public bool TopLeftWall { get; set; }
        public bool TopRightWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomWall { get; set; }
        public bool LeftWall { get; set; }
    }
}

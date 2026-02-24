namespace DL.Core.Maze
{
    public interface IHeptagonVisualization
    {
        public bool IsPointingUp { get; }
        public bool LeftWall { get; set; }
        public bool TopLeftWall { get; set; }
        public bool TopRightWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomRightWall { get; set; }
        public bool BottomWall { get; set; }
        public bool BottomLeftWall { get; set; }
    }
}

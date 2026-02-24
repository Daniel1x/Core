namespace DL.Core.Maze
{
    public interface IOctagonVisualization
    {
        public bool IsPointingUp { get; }
        public bool TopWall { get; set; }
        public bool TopRightWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomRightWall { get; set; }
        public bool BottomWall { get; set; }
        public bool BottomLeftWall { get; set; }
        public bool LeftWall { get; set; }
        public bool TopLeftWall { get; set; }
    }
}

namespace DL.Core.Maze
{
    public interface ITriangleVisualization
    {
        public bool IsPointingUp { get; }
        public bool LeftWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomWall { get; set; }
    }
}

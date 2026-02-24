namespace DL.Core.Maze
{
    public interface ISquareVisualization
    {
        public bool LeftWall { get; set; }
        public bool TopWall { get; set; }
        public bool RightWall { get; set; }
        public bool BottomWall { get; set; }
    }
}

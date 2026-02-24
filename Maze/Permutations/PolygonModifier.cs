namespace DL.Core.Maze
{
    public readonly struct PolygonModifier : IPolygonModifier
    {
        private readonly int bitMask;
        private readonly float angleOffset;

        public int BitMask => bitMask;
        public float AngleOffset => angleOffset;

        public PolygonModifier(int _bitMask, float _angleOffset = 0f)
        {
            bitMask = _bitMask;
            angleOffset = _angleOffset;
        }
    }
}

namespace EleCho.ScratchGame.Drawing
{
    public struct RectangleF
    {
        public readonly float X, Y, Width, Height;
        public RectangleF(float x, float y, float width, float height)
        {
            (X, Y, Width, Height) = (x, y, width, height);
        }
    }
}

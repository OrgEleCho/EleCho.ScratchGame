namespace EleCho.ScratchGame.Drawing
{
    public struct Rectangle
    {
        public readonly int X, Y, Width, Height;
        public Rectangle(int x, int y, int width, int height)
        {
            (X, Y, Width, Height) = (x, y, width, height);
        }
    }
}

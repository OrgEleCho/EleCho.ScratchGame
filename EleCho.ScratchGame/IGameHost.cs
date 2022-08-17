namespace EleCho.ScratchGame
{
    public interface IGameHost
    {
        public Graphics GameGraphics { get; }
        public Rectangle GameBounds { get; }

        public bool IsMouse(MouseButton button);
        public bool IsKeyboard(KeyboardKey key);
        public Point OriginMousePosition { get; }

        public void StartRender();
        public void StopRender();
    }
}

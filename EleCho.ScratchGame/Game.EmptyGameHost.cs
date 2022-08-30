using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame
{

    public partial class Game : ICollection<GameObject>
    {
        public class EmptyGameHost : IGameHost
        {
            public Game? Game { get; set; }

            public Graphics GameGraphics => throw new InvalidOperationException("Empty Game object.");
            public Rectangle GameBounds => Rectangle.Empty;

            public Point OriginMousePosition => Point.Empty;

            public bool IsKeyboard(KeyboardKey key) => false;
            public bool IsMouse(MouseButton button) => false;
            public void StartRender() => throw new InvalidOperationException("Empty Game object.");
            public void StopRender() => throw new InvalidOperationException("Empty Game object.");
        }
    }
}

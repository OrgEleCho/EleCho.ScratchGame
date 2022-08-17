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
            public Graphics GameGraphics => throw new InvalidOperationException("Empty Game object.");

            public Rectangle GameBounds => Rectangle.Empty;

            public Point OriginMousePosition => Point.Empty;

            public bool IsKeyboard(KeyboardKey key) => false;
            public bool IsMouse(MouseButton button) => false;
            public void StartRender() => throw new InvalidOperationException("Empty Game object.");
            public void StopRender() => throw new InvalidOperationException("Empty Game object.");
        }

        public record class GameSpriteCacheKey
        {
            public readonly Image Sprite;
            public readonly float Scale;
            public readonly float Rotation;

            public GameSpriteCacheKey(Image sprite, float scale, float rotation)
            {
                Sprite = sprite;
                Scale = scale;
                Rotation = rotation;
            }
        }

        public record class GameSpriteCacheItem
        {
            public readonly Bitmap CachedSprite;
            public readonly float Time;
            public float AccessTime;

            public GameSpriteCacheItem(Bitmap cachedSprite, float time)
            {
                CachedSprite = cachedSprite;
                AccessTime = time;
                Time = time;
            }
        }
    }
}

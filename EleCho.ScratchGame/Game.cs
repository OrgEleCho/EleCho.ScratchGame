using EleCho.ScratchGame.Utils;
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Security.Policy;

namespace EleCho.ScratchGame
{
    /// <summary>
    /// 游戏对象
    /// 游戏中坐标系与屏幕坐标系不同. 中间为 0, y 向上为正.
    /// </summary>
    public partial class Game
    {
        public Game(IGameHost host, int width, int height)
        {
            this.host = host;
            this.width = width;
            this.height = height;
            Size = new Size(Width, Height);
            gdiLeftTop = Point.Truncate(GameUtils.OriginPoint2GdiPoint(Size, Point.Empty));
            Bounds = new Rectangle(new Point(-width / 2, -height / 2), Size);

            spritesCache = new GameSpriteCache(this);
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(host.GameGraphics, new Rectangle(0, 0, width, height));
            bufferedGraphics.Graphics.Transform = new Matrix(1, 0, 0, 1, (float)width / 2, (float)height / 2);
        }

        private Game()
        {
            host = new EmptyGameHost();
            width = 0;
            height = 0;
            Size = Size.Empty;
            gdiLeftTop = Point.Empty;
            Bounds = Rectangle.Empty;

            spritesCache = new GameSpriteCache(this);
            bufferedGraphics = null;
        }

        #region 私有定义
        private readonly IGameHost host;
        private readonly int width;
        private readonly int height;

        private BufferedGraphics? bufferedGraphics;
        #endregion

        #region 基本定义
        public IGameHost Host => host;
        public int Width => width;
        public int Height => height;
        public Size Size { get; }
        public Rectangle Bounds { get; }

        public int UpdateDelay { get; set; } = 10;
        public bool IsRunning => gameRunTask != null ? !gameRunTask.IsCompleted : false;

        public Graphics Graphics => bufferedGraphics?.Graphics ??
            throw new InvalidOperationException("No GameHost object.");

        public static readonly Game Empty = new Game();
        #endregion

        #region 呈现 (背景颜色与图像)
        private readonly Point gdiLeftTop;
        public Color CanvasColor { get; set; } = Color.White;
        public Image? Background { get; set; }
        #endregion

        #region 游戏运行数据记录
        private DateTime? startTime = null;
        private DateTime? lastFrameTime = null;

        public float Time { get; set; }
        /// <summary>
        /// 帧刷新间隔
        /// </summary>
        public float DeltaTime { get; private set; }
        #endregion

        #region 性能优化
        private Image? bufferOrigin;
        private Bitmap? bufferedBackground;

        private readonly GameSpriteCache spritesCache;

        internal Bitmap GetProcessedSprite(Image origin, float scale, float rotation)
        {
            GameSpriteCacheKey key = new GameSpriteCacheKey(origin, scale, rotation);
            if (spritesCache.TryGetValue(key, out var img))
            {
                return img;
            }
            else
            {
                Bitmap processedSprite;
                GdiUtils.ProcessImage(origin, scale, rotation, out processedSprite);
                spritesCache[key] = processedSprite;
                return processedSprite;
            }
        }

        #endregion

        #region IGameHost
        public bool GetMouseState(MouseButton button) => host.IsMouse(button);

        public bool GetKeyboardState(KeyboardKey key) => host.IsKeyboard(key);

        public PointF MousePosition => GameUtils.OriginPoint2GamePoint(Size, host.OriginMousePosition);
        #endregion

        public bool IsMouseIn(GameSprite sprite)
        {
            return GameUtils.MouseInSprite(sprite, MousePosition);
        }
        public bool IsCollided(GameSprite a, GameSprite b)
        {

            return GameUtils.IsCollided(a, b, this);
        }

        public void InvokeMouse(PointF point)
        {
            foreach (GameSprite sprite in this)
            {
                if (GameUtils.MouseInSprite(sprite, point))
                    sprite.InvokeMouse();
            }
        }

        public void InvokeKeyboard(KeyboardKey key)
        {
            foreach (GameSprite sprite in this)
                sprite.InvokeKeyboard(key);
        }

        public void LoadScene(GameScene scene)
        {
            RemoveAllObjects();

            CanvasColor = scene.CanvasColor;
            Background = scene.Background;
            foreach (var sprite in scene)
                AddObject(sprite);
        }

        public void Render()
        {
            if (bufferedGraphics == null)
                return;

            bufferedGraphics.Graphics.Clear(CanvasColor);

            if (Background != null)
            {
                if (bufferOrigin != Background || bufferedBackground == null)
                {
                    bufferedBackground?.Dispose();

                    bufferOrigin = Background;
                    GdiUtils.PlaceImage(Background, Size, GdiUtils.PlacingMode.UniformToFill, out bufferedBackground);
                }

                bufferedGraphics.Graphics.DrawImageUnscaled(bufferedBackground, gdiLeftTop);
            }

            foreach (GameObject obj in this)
            {
                obj.Render();
            }

            bufferedGraphics.Render();
        }

        /// <summary>
        /// 主循环中每次循环都会执行的操作
        /// </summary>
        public void MainLoopAction()
        {
            DateTime now = DateTime.Now;

            if (startTime == null || lastFrameTime == null)
            {
                startTime = now;
                lastFrameTime = startTime;
            }

            Time = (float)(now - startTime.Value).TotalSeconds;
            DeltaTime = (float)(now - lastFrameTime.Value).TotalSeconds;

            spritesCache.Update();
            foreach (GameObject obj in this)
                obj.Update();

            lastFrameTime = now;
        }

        Task? gameRunTask;
        CancellationTokenSource? gameRunCancellation;
        private async Task MainGameLoopAsync(CancellationToken token)
        {
            while(true)
            {
                if (token.IsCancellationRequested)
                    return;

                await Task.Delay(UpdateDelay, token);
                MainLoopAction();
            }
        }

        public void StartGame()
        {
            if (gameRunCancellation != null)
                throw new InvalidOperationException("Game is already running");

            gameRunCancellation = new CancellationTokenSource();
            gameRunTask = MainGameLoopAsync(gameRunCancellation.Token);
        }

        /// <summary>
        /// 停止游戏
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void StopGame()
        {
            if (gameRunCancellation == null)
                throw new InvalidOperationException("Game is not running");

            gameRunCancellation.Cancel();
            gameRunTask?.Wait();

            gameRunTask = null;
            gameRunCancellation = null;
        }
    }
}

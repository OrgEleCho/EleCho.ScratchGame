using Microsoft.VisualBasic.Devices;
using EleCho.ScratchGame.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EleCho.ScratchGame
{
    public partial class GamePanel : Control, IGameHost
    {
        public GamePanel()
        {
            InitializeComponent();

            SetStyle(ControlStyles.Selectable, true);
        }

        public Game? Game
        {
            get => game; set
            {
                game = value;
                if (game != null)
                    MinimumSize = game.Size;
                else
                    MinimumSize = Size.Empty;
            }
        }
        public Graphics GameGraphics { get => CreateGraphics(); }
        public Rectangle GameBounds =>
            new Rectangle(Point.Empty, game?.Size ?? throw new InvalidOperationException("No Game object"));

        public int RenderDelay { get; set; } = 20;

        public Point OriginMousePosition => PointToClient(MousePosition);

        const int WM_KEYDOWN                     = 0x0100;
        const int WM_KEYUP                       = 0x0101;
        const int WM_LBUTTONDOWN                 = 0x0201;
        const int WM_LBUTTONUP                   = 0x0202;
        const int WM_MBUTTONDOWN                 = 0x0207;
        const int WM_MBUTTONUP                   = 0x0208;
        const int WM_RBUTTONDOWN                 = 0x0204;
        const int WM_RBUTTONUP                   = 0x0205;
        const int WM_XBUTTONDOWN                 = 0x020B;
        const int WM_XBUTTONUP                   = 0x020C;

        readonly Dictionary<KeyboardKey, bool> keyStates = new Dictionary<KeyboardKey, bool>();
        readonly Dictionary<MouseButton, bool> mouseStates = new Dictionary<MouseButton, bool>();

        private Game? game;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_KEYDOWN:
                    keyStates[(KeyboardKey)m.WParam] = true;
                    Game?.InvokeKeyboard((KeyboardKey)m.WParam);
                    return;
                case WM_KEYUP:
                    keyStates[(KeyboardKey)m.WParam] = false;
                    return;
                case WM_LBUTTONDOWN:
                    if (!Focused)
                        Focus();
                    mouseStates[MouseButton.Left] = true;
                    return;
                case WM_LBUTTONUP:
                    mouseStates[MouseButton.Left] = false;
                    return;
                case WM_MBUTTONDOWN:
                    if (!Focused)
                        Focus();
                    mouseStates[MouseButton.Middle] = true;
                    return;
                case WM_MBUTTONUP:
                    mouseStates[MouseButton.Middle] = false;
                    return;
                case WM_RBUTTONDOWN:
                    if (!Focused)
                        Focus();
                    mouseStates[MouseButton.Right] = true;
                    return;
                case WM_RBUTTONUP:
                    mouseStates[MouseButton.Right] = false;
                    return;
                case WM_XBUTTONDOWN:
                    if (!Focused)
                        Focus();
                    switch (((int)m.WParam >> 16) & 0xFFFF)
                    {
                        case 1:
                            mouseStates[MouseButton.XButton1] = true;
                            return;
                        case 2:
                            mouseStates[MouseButton.XButton2] = true;
                            return;
                    }
                    return;
                case WM_XBUTTONUP:
                    switch (((int)m.WParam >> 16) & 0xFFFF)
                    {
                        case 1:
                            mouseStates[MouseButton.XButton1] = false;
                            return;
                        case 2:
                            mouseStates[MouseButton.XButton2] = false;
                            return;
                    }
                    return;
            }

            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Game != null)
            {
                Game.Render();
            }

            base.OnPaint(pe);
        }

        public bool IsMouse(MouseButton button)
        {
            return mouseStates.TryGetValue(button, out bool rst) ? rst : false;
        }

        public bool IsKeyboard(KeyboardKey key)
        {
            return keyStates.TryGetValue(key, out bool v) ? v : false;
        }

        Task? renderTask;
        CancellationTokenSource? renderCancellation;

        private async Task GameRenderLoopAsync(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    return;

                await Task.Delay(RenderDelay, token);
                try
                {
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), ClientRectangle));
                }
                catch { }
            }
        }

        public void StartRender()
        {
            if (renderCancellation != null)
                throw new InvalidOperationException("Render is already running");

            renderCancellation = new CancellationTokenSource();
            renderTask = GameRenderLoopAsync(renderCancellation.Token);
        }

        public void StopRender()
        {
            if (renderCancellation == null)
                throw new InvalidOperationException("Render is not running");

            renderCancellation.Cancel();

            renderTask = null;
            renderCancellation = null;
        }
    }
}

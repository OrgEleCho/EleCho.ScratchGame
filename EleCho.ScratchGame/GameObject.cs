using EleCho.ScratchGame.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using static System.Net.Mime.MediaTypeNames;

namespace EleCho.ScratchGame
{
    public abstract class GameObject
    {
        internal Game? game;
        private Action? mouseActions;
        private Dictionary<KeyboardKey, Action> keyboardActions = new Dictionary<KeyboardKey, Action>();
        private float rotation;

        public Game Game => game ?? Game.Empty;
        public PointF Position { get; set; }
        public float Scale { get; set; } = 1;
        public float Rotation { get => rotation; set => rotation = value % 360; }
        public bool Visible { get; set; } = true;

        public Brush BoxBackground { get; set; } = Brushes.Transparent;
        public Brush Background { get; set; } = Brushes.Transparent;
        public float BorderRadius { get; set; } = 0;
        public float BorderExpansion { get; set; } = 0;

        public PointF Pivot { get; set; } = new PointF(0.5f, 0.5f);
        public GameObjectCollider Collider { get; set; } = new GameObjectCollider();

        public abstract SizeF? GetOriginSize();

        public SizeF? GetActualSize()
        {
            SizeF? originSize = GetOriginSize();
            if (!originSize.HasValue)
                return null;
            return ImgUtils.Rotate(originSize.Value * Scale, Rotation);
        }

        public virtual void InvokeKeyboard(KeyboardKey key)
        {
            if (keyboardActions.TryGetValue(key, out var action))
            {
                action.Invoke();
            }
        }

        public virtual void InvokeMouse()
        {
            mouseActions?.Invoke();
        }

        public virtual void Load()
        {

        }

        public GameObject OnKeyboard(KeyboardKey key, Action action)
        {
            keyboardActions[key] = action;
            return this;
        }

        public GameObject OnMouse(Action action)
        {
            if (mouseActions == null)
                mouseActions = action;
            else
                mouseActions += action;

            return this;
        }

        /// <summary>
        /// 开始渲染 (在渲染前调用此方法)
        /// </summary>
        /// <param name="g">绘图 Graphics</param>
        /// <param name="boxBackRect">盒子背景矩形</param>
        /// <param name="originRegion">原Region</param>
        /// <returns>是否继续绘图</returns>
        protected bool BeginBoxRender(Graphics g, [NotNullWhen(true)] out RectangleF? boxBackRect, [NotNullWhen(true)] out Region originRegion)
        {
            originRegion = g.Clip;
            boxBackRect = GdiUtils.GetActualGdiBounds(this);
            if (!boxBackRect.HasValue)
                return false;

            RectangleF _rect = boxBackRect.Value;
            float borderExpansion = BorderExpansion;
            float doubleBorderExpansion = borderExpansion * 2;
            if (borderExpansion != 0)
                boxBackRect = _rect = new RectangleF(
                    _rect.X - borderExpansion,
                    _rect.Y - borderExpansion,
                    _rect.Width + doubleBorderExpansion,
                    _rect.Height + doubleBorderExpansion);

            float borderRadius = BorderRadius;
            if (borderRadius == 0)
                return true;

            if (_rect.Width / 2 < borderRadius)
                borderRadius = _rect.Width / 2;
            if (_rect.Height / 2 < borderRadius)
                borderRadius = _rect.Height / 2;

            float x = _rect.X;
            float y = _rect.Y;
            float width = _rect.Width;
            float height = _rect.Height;
            using GraphicsPath path = new GraphicsPath();
            path.AddArc(x, y, borderRadius, borderRadius, 180, 90);
            path.AddArc(x + width - borderRadius, y, borderRadius, borderRadius, 270, 90);
            path.AddArc(x + width - borderRadius, y + height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(x, y + height - borderRadius, borderRadius, borderRadius, 90, 90);
            path.CloseAllFigures();

            g.Clip = new Region(path);

            return true;
        }

        /// <summary>
        /// 开始绘图 (方法将进行 Region 与 Transform 的设定)
        /// </summary>
        /// <param name="g">绘图 Grahpics</param>
        /// <param name="backRect">背景矩形</param>
        /// <param name="targetPoint">绘图目标点</param>
        /// <param name="originTransform">原Transform</param>
        /// <returns></returns>
        protected bool BeginRender(Graphics g, [NotNullWhen(true)] out RectangleF? backRect, [NotNullWhen(true)] out PointF? targetPoint, [NotNullWhen(true)] out Matrix? originTransform)
        {
            SizeF? _originSzie = GetOriginSize();
            if (!_originSzie.HasValue)
            {
                backRect = null;
                targetPoint = null;
                originTransform = null;
                return false;
            }

            SizeF originSize = _originSzie.Value;
            PointF pivot = Pivot;

            Matrix origin = g.Transform.Clone();
            Matrix matrix = origin.Clone();
            matrix.ScaleAndRotateAt(Scale, Scale, Rotation, GameUtils.GamePoint2GdiPoint(Position));

            targetPoint = GameUtils.GamePoint2GdiPoint(Position) - new SizeF(originSize.Width * pivot.X, originSize.Height * pivot.Y);
            originTransform = g.Transform;
            g.Transform = matrix;

            float scaledExpansion = BorderExpansion / Scale;
            float doubleScaledExpansion = scaledExpansion * 2;
            RectangleF _rect = new RectangleF(targetPoint.Value, originSize);
            _rect = new RectangleF(
                _rect.X - scaledExpansion,
                _rect.Y - scaledExpansion,
                _rect.Width + doubleScaledExpansion,
                _rect.Height + doubleScaledExpansion);
            backRect = _rect;

            float borderRadius = BorderRadius;
            if (borderRadius == 0)
                return true;

            if (_rect.Width / 2 < borderRadius)
                borderRadius = _rect.Width / 2;
            if (_rect.Height / 2 < borderRadius)
                borderRadius = _rect.Height / 2;

            float x = _rect.X;
            float y = _rect.Y;
            float width = _rect.Width;
            float height = _rect.Height;
            using GraphicsPath path = new GraphicsPath();
            path.AddArc(x, y, borderRadius, borderRadius, 180, 90);
            path.AddArc(x + width - borderRadius, y, borderRadius, borderRadius, 270, 90);
            path.AddArc(x + width - borderRadius, y + height - borderRadius, borderRadius, borderRadius, 0, 90);
            path.AddArc(x, y + height - borderRadius, borderRadius, borderRadius, 90, 90);
            path.CloseAllFigures();

            g.Clip = new Region(path);

            return true;
        }

        /// <summary>
        /// 结束渲染 (在渲染后调用此方法, 重置为原Region与Transform)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="originRegion"></param>
        protected void EndRender(Graphics g, Region originRegion, Matrix originTransform)
        {
            g.Clip = originRegion;
            g.Transform = originTransform;
        }

        /// <summary>
        /// 渲染盒背景
        /// </summary>
        /// <param name="g">绘图 Graphics</param>
        /// <param name="rect">矩形区域, 由 BeginBoxRender 获得</param>
        protected void RenderBoxBack(Graphics g, RectangleF rect)
        {
            if (BoxBackground is not SolidBrush solid || solid.Color.A != 0)
                g.FillRectangle(BoxBackground, rect);
        }

        /// <summary>
        /// 渲染背景
        /// </summary>
        /// <param name="g">绘图 Graphics</param>
        /// <param name="rect">矩形区域, 由 BeginRender 获得</param>
        protected void RenderBack(Graphics g, RectangleF rect)
        {
            if (Background is not SolidBrush solid || solid.Color.A != 0)
                g.FillRectangle(Background, rect);
        }

        public abstract void Render();

        public virtual void Start()
        {

        }

        public virtual void Unload()
        {

        }

        public virtual void Update()
        {

        }
    }
}
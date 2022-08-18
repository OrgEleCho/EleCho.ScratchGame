namespace EleCho.ScratchGame
{
    public abstract class GameObject
    {
        internal Game? game;
        private Action? mouseActions;
        private Dictionary<KeyboardKey, Action> keyboardActions = new Dictionary<KeyboardKey, Action>();
        private int rotation;

        public Game Game => game ?? Game.Empty;
        public PointF Position { get; set; }
        public float Scale { get; set; } = 1;
        public int Rotation { get => rotation; set => rotation = value % 360; }
        public bool Visible { get; set; } = true;

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

        public abstract Bitmap? GetActualCanvas();

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
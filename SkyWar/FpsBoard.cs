using EleCho.ScratchGame;

namespace SkyWar
{
    internal class FpsBoard : GameText
    {
        int logical_fps = 0;
        int render_fps = 0;

        public FpsBoard()
        {
            Background = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            BorderRadius = 5;
            BorderExpansion = 3;
        }

        Task? tickTask;
        CancellationTokenSource? cancellation;

        private async Task Tick(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                    return;
                await Task.Delay(1000);
                Text = $"Logic: {logical_fps}fps; Render: {render_fps}fps";
                logical_fps = 0;
                render_fps = 0;
            }
        }

        public override void Load()
        {
            cancellation = new CancellationTokenSource();
            tickTask = Tick(cancellation.Token);
        }

        public override void Update()
        {
            logical_fps++;

            SizeF? actualSize = GetActualSize();
            if (actualSize.HasValue)
            {
                Position = new PointF(
                    Game.Width / 2 - actualSize.Value.Width / 2 - 10,
                    Game.Height / 2 - actualSize.Value.Height / 2 - 10);
            }

            base.Update();
        }

        public override void Unload()
        {
            if (cancellation != null)
                cancellation.Cancel();
        }

        public override void Render()
        {
            render_fps++;

            base.Render();
        }
    }
}

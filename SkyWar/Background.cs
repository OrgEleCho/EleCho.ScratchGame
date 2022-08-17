using EleCho.ScratchGame;
using SkyWar.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWar
{
    internal class Background : GameSprite
    {
        public Background()
        {
            Sprite = Resources.background;
        }

        public int Speed = 100;

        public override void Update()
        {
            if (Sprite == null)
                return;

            float move = Speed * Game.DeltaTime;

            float y = Position.Y;
            y -= move;

            if (y < -Sprite.Height)
                y = Sprite.Height + Sprite.Height + y;

            Position = new PointF(Position.X, y);
        }
    }
}

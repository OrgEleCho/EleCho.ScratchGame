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
    internal class Bullet : GameSprite
    {
        static Image bullet1 = Resources.bullet1;
        static Image bullet2 = Resources.bullet2;

        public Bullet()
        {
            Sprite = bullet1;
        }

        public SizeF Speed { get; set; } = new SizeF(0, 600);

        public override void Update()
        {
            Position += new SizeF(Speed.Width * Game.DeltaTime, Speed.Height * Game.DeltaTime);

            if (!Game.Bounds.Contains(Point.Truncate(Position)))
            {
                Game.RemoveObject(this);
            }
        }
    }
}

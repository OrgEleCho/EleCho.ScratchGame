using EleCho.ScratchGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWar
{
    internal class EnermyGen : GameSprite
    {
        public EnermyGen()
        {
            EnermyGenLoop();
        }

        private async void EnermyGenLoop()
        {
            while (true)
            {
                await Task.Delay(2000);
                Game.AddObject(new Enermy()
                {
                    Position = new PointF(Random.Shared.Next(Game.Bounds.Left, Game.Bounds.Right), Game.Bounds.Bottom)
                });
            }
        }
    }
}

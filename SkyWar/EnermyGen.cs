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
        private readonly ScoreBoard score;

        public EnermyGen(ScoreBoard score)
        {
            EnermyGenLoop();
            this.score = score;
        }

        private async void EnermyGenLoop()
        {
            while (true)
            {
                await Task.Delay(2000);
                Game.AddObject(new Enermy(score)
                {
                    Position = new PointF(Random.Shared.Next(Game.Bounds.Left, Game.Bounds.Right), Game.Bounds.Bottom)
                });
            }
        }
    }
}

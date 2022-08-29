using EleCho.ScratchGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyWar
{
    internal class ScoreBoard : GameText
    {
        private int score;

        public ScoreBoard()
        {
            Font = new Font(Font.FontFamily, 16);
        }

        public int Score
        {
            get => score; set
            {
                score = value;
                Text = value.ToString();
            }
        }
        public override void Update()
        {
            SizeF? actualSize = GetActualSize();
            if (actualSize.HasValue)
            {
                Position = new PointF(
                    -Game.Width / 2 + actualSize.Value.Width / 2 + 10,
                    Game.Height / 2 - actualSize.Value.Height / 2 - 10);
            }

            base.Update();
        }
    }
}

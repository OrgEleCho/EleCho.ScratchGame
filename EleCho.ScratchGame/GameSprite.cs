using EleCho.ScratchGame.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame
{
    public class GameSprite : GameObject
    {
        public GameSprite()
        {

        }

        #region 基础定义
        public Image? Sprite { get; set; }
        public float Scale { get; set; } = 1;
        public int Rotation { get; set; }

        #endregion
        #region 鼠标键盘事件逻辑
        #endregion

        public override void Render()
        {
            if (Sprite == null || !Visible || game == null)
                return;

            PointF gdilocation = GameUtils.GamePoint2GdiPoint(Position);

            Image bufferedSprite = Game.GetProcessedSprite(Sprite, Scale, Rotation);
            SizeF size = (SizeF)bufferedSprite.Size;
            PointF originLocation = new PointF(gdilocation.X - size.Width / 2, gdilocation.Y - size.Height / 2);
            game.Graphics.DrawImage(bufferedSprite, (int)originLocation.X, (int)originLocation.Y);
        }
    }
}

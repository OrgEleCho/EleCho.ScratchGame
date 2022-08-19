using EleCho.ScratchGame.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame
{
    public class GameSprite : GameObject
    {
        public Bitmap? Sprite { get; set; }

        public GameSprite()
        {

        }

        #region 性能优化

        #endregion
        #region 鼠标键盘事件逻辑
        #endregion

        public override Bitmap? GetActualCanvas()
        {
            if (Sprite == null || !Visible || game == null)
                return null;
            return null;
        }

        public override void Render()
        {
            if (Sprite == null || !Visible || game == null)
                return;

            Graphics g = game.Graphics;
            Sprite.SetResolution(g.DpiX, g.DpiY);

            SizeF originSize = Sprite.Size;

            Matrix origin = g.Transform.Clone();
            Matrix matrix = origin.Clone();
            matrix.ScaleAndRotateAt(Scale, Scale, Rotation, GameUtils.GamePoint2GdiPoint(Position));

            PointF targetPoint = GameUtils.GamePoint2GdiPoint(Position) - originSize / 2;

            g.Transform = matrix;
            g.DrawImage(Sprite, Point.Truncate(targetPoint));

            g.Transform = origin;
        }
    }
}

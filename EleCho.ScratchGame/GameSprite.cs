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

        public override void Render()
        {
            if (Sprite == null || !Visible || game == null)
                return;

            Graphics g = game.Graphics;
            if (!BeginBoxRender(g, out RectangleF? actualRect, out Region originRegion))
                return;
            RenderBoxBack(g, actualRect.Value);

            Sprite.SetResolution(g.DpiX, g.DpiY);

            if (!BeginRender(g, out RectangleF? rect, out PointF? targetPoint, out Matrix? originTransform))
                return;
            RenderBack(g, rect.Value);
            g.DrawImage(Sprite, Point.Truncate(targetPoint.Value));

            EndRender(g, originRegion, originTransform);
        }

        public override SizeF? GetOriginSize()
        {
            return Sprite?.Size;
        }
    }
}

using EleCho.ScratchGame.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EleCho.ScratchGame
{
    public class GameText : GameObject
    {
        public Font Font { get; set; }
        public Brush Brush { get; set; }
        public string? Text { get; set; }

        public GameText()
        {
            Font = SystemFonts.DefaultFont;
            Brush = Brushes.Black;
        }


        public override void Render()
        {
            if (Text == null || !Visible || game == null)
                return;

            Graphics g = game.Graphics;
            if (!BeginBoxRender(g, out RectangleF? actualRect, out Region originRegion))
                return;
            RenderBoxBack(g, actualRect.Value);

            if (!BeginRender(g, out RectangleF? rect, out PointF? targetPoint, out Matrix? originTransform))
                return;
            RenderBack(g, rect.Value);
            g.DrawString(Text, Font, Brush, Point.Truncate(targetPoint.Value), StringFormat.GenericTypographic);

            EndRender(g, originRegion, originTransform);
        }

        public override SizeF? GetOriginSize()
        {
            if (Text == null || !Visible || game == null)
                return null;

            return game.Graphics.MeasureString(Text, Font, PointF.Empty, StringFormat.GenericTypographic);
        }
    }
}

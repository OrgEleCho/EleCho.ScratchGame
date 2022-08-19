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


        public override Bitmap? GetActualCanvas()
        {
            if (Text == null || !Visible || game == null)
                return null;

            return null;
        }

        public override void Render()
        {
            if (Text == null || !Visible || game == null)
                return;

            Graphics g = game.Graphics;
            SizeF originSize = g.MeasureString(Text, Font, PointF.Empty, StringFormat.GenericTypographic);

            Matrix origin = g.Transform.Clone();
            Matrix matrix = origin.Clone();
            matrix.ScaleAndRotateAt(Scale, Scale, Rotation, GameUtils.GamePoint2GdiPoint(Position));

            PointF targetPoint = GameUtils.GamePoint2GdiPoint(Position) - originSize / 2;

            g.Transform = matrix;
            g.DrawString(Text, Font, Brush, targetPoint, StringFormat.GenericTypographic);

            g.Transform = origin;
        }
    }
}

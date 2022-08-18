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
            SizeF scaledSize = originSize * Scale;
            SizeF newSize = ImgUtils.Rotate(scaledSize, Rotation);

            Matrix origin = g.Transform.Clone();
            Matrix matrix = origin.Clone();
            matrix.Scale(Scale, Scale);
            matrix.RotateAt(Rotation, Position);

            PointF targetPoint = GameUtils.GamePoint2GdiPoint(Position - newSize / 2 + ImgUtils.UniformOffset(newSize, scaledSize));
            targetPoint = new PointF(targetPoint.X / Scale, targetPoint.Y / Scale);

            g.Transform = matrix;
            g.DrawString(Text, Font, Brush, targetPoint, StringFormat.GenericTypographic);

            g.Transform = origin;
        }
    }
}

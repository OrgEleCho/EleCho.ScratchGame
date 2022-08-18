using EleCho.ScratchGame.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        internal static Bitmap GetProcessedText(Game game, Font font, Brush brush, string text, float scale, float rotation)
        {
            Game.GameTextCacheKey key = new Game.GameTextCacheKey(font, brush, text, scale, rotation);
            if (game.spritesCache.TryGetValue(key, out var img))
            {
                return img;
            }
            else
            {
                GdiUtils.ProcessText(game.Graphics, font, brush, text, scale, rotation, out Bitmap proccessedText);
                game.spritesCache[key] = proccessedText;
                return proccessedText;
            }
        }


        public override Bitmap? GetActualCanvas()
        {
            if (Text == null || !Visible || game == null)
                return null;

            return GetProcessedText(Game, Font, Brush, Text, Scale, Rotation);
        }

        public override void Render()
        {
            if (GetActualCanvas() is Bitmap output)
            {
                PointF gdilocation = GameUtils.GamePoint2GdiPoint(Position);
                SizeF size = (SizeF)output.Size;
                PointF originLocation = new PointF(gdilocation.X - size.Width / 2, gdilocation.Y - size.Height / 2);
                game!.Graphics.DrawImage(output, (int)originLocation.X, (int)originLocation.Y);
            }
        }
    }
}

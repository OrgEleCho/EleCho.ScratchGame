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
        public Image? Sprite { get; set; }

        public GameSprite()
        {

        }

        #region 性能优化

        internal static Bitmap GetProcessedSprite(Game game, Image origin, float scale, float rotation)
        {
            Game.GameSpriteCacheKey key = new Game.GameSpriteCacheKey(origin, scale, rotation);
            if (game.spritesCache.TryGetValue(key, out var img))
            {
                return img;
            }
            else
            {
                GdiUtils.ProcessImage(origin, scale, rotation, out Bitmap processedSprite);
                game.spritesCache[key] = processedSprite;
                return processedSprite;
            }
        }

        #endregion
        #region 鼠标键盘事件逻辑
        #endregion

        public override Bitmap? GetActualCanvas()
        {
            if (Sprite == null || !Visible || game == null)
                return null;
            return GetProcessedSprite(Game, Sprite, Scale, Rotation);
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

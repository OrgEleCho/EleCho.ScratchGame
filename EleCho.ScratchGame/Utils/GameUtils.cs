using PolygonIntersection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame.Utils
{

#if DEBUG
    public
#else
    internal
#endif
        class GameUtils
    {
        public static PointF GamePoint2OriginPoint(Size gameSize, Point gamePoint)
        {
            Point unflipped = gamePoint + gameSize / 2;
            return new PointF(unflipped.X, -unflipped.Y);
        }

        public static PointF OriginPoint2GamePoint(Size gameSize, Point originPoint)
        {
            Point unflipped = originPoint - gameSize / 2;
            return new PointF(unflipped.X, -unflipped.Y);
        }

        public static PointF GamePoint2GdiPoint(PointF point)
        {
            return new PointF(point.X, -point.Y);
        }

        public static PointF OriginPoint2GdiPoint(Size gameSize, Point originPoint)
        {
            return originPoint - gameSize / 2;
        }

        public static PointF Image2CenterOffset(Size imgSize)
        {
            return new PointF(-(float)imgSize.Width / 2, -(float)imgSize.Height / 2);
        }

        public static RectangleF GetOriginBounds(Game game, GameSprite sprite)
        {
            SizeF originSize = sprite.Sprite != null ? sprite.Sprite.Size * sprite.Scale : SizeF.Empty;
            PointF originPosition = sprite.Position - originSize / 2 + game.Size / 2;
            return new RectangleF(originPosition, originSize);
        }

        public static PointF[] GetSpriteVertexes(GameSprite sprite)
        {
            SizeF size = (sprite.Sprite?.Size ?? Size.Empty) * sprite.Scale;
            PointF position = sprite.Position - size / 2;
            PointF[] rectangle = ImgUtils.RotateAt(new RectangleF(position, size), position + size / 2, ImgUtils.Degree2Radian(sprite.Rotation));
            return rectangle;
        }

        public static bool MouseInSprite(GameSprite sprite, PointF gamePoint)
        {
            using Region region = new Region();
            using GraphicsPath path = new GraphicsPath();

            PointF[] vertexes = GetSpriteVertexes(sprite);

            path.AddPolygon(vertexes);
            region.Union(path);

            return region.IsVisible(gamePoint);
        }

        public static bool IsCollided(GameSprite a, GameSprite b, Game game)
        {
            if (a.Sprite == null || b.Sprite == null)
                return false;

            Bitmap bmp1 = game.GetProcessedSprite(a.Sprite, a.Scale, a.Rotation);
            Bitmap bmp2 = game.GetProcessedSprite(b.Sprite, b.Scale, b.Rotation);

            PointF
                pos1 = GamePoint2GdiPoint(a.Position) - bmp1.Size / 2f,
                pos2 = GamePoint2GdiPoint(b.Position) - bmp2.Size / 2f;

            return IsCollided(bmp1, bmp2, (int)pos1.X, (int)pos1.Y, (int)pos2.X, (int)pos2.Y);

            PointF[] vertexes1 = GetSpriteVertexes(a);
            PointF[] vertexes2 = GetSpriteVertexes(b);
            Polygon polygon1 = new Polygon(vertexes1);
            Polygon polygon2 = new Polygon(vertexes2);

            return Polygon.Collision(polygon1, polygon2);
        }

        /// <summary>
        /// 判断是否碰撞 (请使用 X正方向为左, Y正方向为下的坐标系)
        /// </summary>
        /// <param name="bmp1">图片1</param>
        /// <param name="bmp2">图片2</param>
        /// <param name="p1x">图片1 X坐标</param>
        /// <param name="p1y">图片1 Y坐标</param>
        /// <param name="p2x">图片2 X坐标</param>
        /// <param name="p2y">图片2 Y坐标</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static unsafe bool IsCollided(Bitmap bmp1, Bitmap bmp2, int p1x, int p1y, int p2x, int p2y)
        {
            bool IsTransparentAlwaysTrue(IntPtr bmpdata, int x, int y, int stride)
            {
                return true;
            }
            bool IsTransparent16(IntPtr bmpdata, int x, int y, int stride)
            {
                byte* ptr = (byte*)bmpdata;
                return (ptr[stride * y + x * 4 + 3] & 1) == 0;
            }
            bool IsTransparent32(IntPtr bmpdata, int x, int y, int stride)
            {
                byte* ptr = (byte*)bmpdata;
                return ptr[stride * y + x * 4 + 3] == 0;
            }
            bool IsTransparent64(IntPtr bmpdata, int x, int y, int stride)
            {
                byte* ptr = (byte*)bmpdata;
                int pos = stride * y + x * 8;
                return ptr[pos + 6] == 0 && ptr[pos + 7] == 0;
            }
            Func<IntPtr, int, int, int, bool> GetTransparentChecker(PixelFormat pixelFormat)
            {
                return pixelFormat switch
                {
                    PixelFormat.Format16bppRgb555 or
                    PixelFormat.Format16bppRgb565 or
                    PixelFormat.Format24bppRgb or
                    PixelFormat.Format32bppRgb or
                    PixelFormat.Format1bppIndexed or 
                    PixelFormat.Format4bppIndexed or
                    PixelFormat.Format8bppIndexed or 
                    PixelFormat.Format48bppRgb => IsTransparentAlwaysTrue,
                    PixelFormat.Format64bppPArgb or 
                    PixelFormat.Format64bppArgb => IsTransparent64,
                    PixelFormat.Format32bppPArgb or
                    PixelFormat.Format32bppArgb => IsTransparent32,
                    PixelFormat.Format16bppArgb1555 => IsTransparent16,
                    _ => throw new ArgumentException("Not supported pixel format"),
                };
            }

            PixelFormat p1f = bmp1.PixelFormat;
            PixelFormat p2f = bmp2.PixelFormat;

            Func<IntPtr, int, int, int, bool> transparentChecker1 = GetTransparentChecker(p1f);
            Func<IntPtr, int, int, int, bool> transparentChecker2 = GetTransparentChecker(p2f);


            int
                w1 = bmp1.Width,           // 图1 宽度
                h1 = bmp1.Height,          // 图1 高度
                w2 = bmp2.Width,           // 图2 宽度
                h2 = bmp2.Height,          // 图2 高度
                offx = p2x - p1x,          // x坐标 偏移
                offy = p2y - p1y;          // y坐标 偏移

            int
                b1top = p1y,                  // 图1 顶部坐标
                b1left = p1x,                 // 图1 左侧坐标
                b1right = p1x + w1 - 1,       // 图1 右侧坐标
                b1bottom = p1y + h1 - 1,      // 图1 底部坐标

                b2top = p2y,                  // 图2 顶部坐标
                b2left = p2x,                 // 图2 左侧坐标
                b2right = p2x + w2 - 1,       // 图2 右侧坐标
                b2bottom = p2y + h2 - 1;      // 图2 底部坐标

            int xstart, xend, ystart, yend;       // 相交部分的 x开始, x结束, y开始, y结束


            // 下面一堆 if 是查找重合区域的

            if (b2left < b1left)
            {
                if (b2right < b1left)
                    return false;
                xstart = b1left;
                if (b2right > b1right)
                    xend = b1right + 1;
                else
                    xend = b2right + 1;
            }
            else
            {
                if (b2left > b1right)
                    return false;
                xstart = b2left;
                if (b2right > b1right)
                    xend = b1right + 1;
                else
                    xend = b2right + 1;
            }

            if (b2top < b1top)
            {
                if (b2bottom < b1top)
                    return false;
                ystart = b1top;
                if (b2bottom > b1bottom)
                    yend = b1bottom + 1;
                else
                    yend = b2bottom + 1;
            }
            else
            {
                if (b2top > b1bottom)
                    return false;
                ystart = b2top;
                if (b2bottom > b1bottom)
                    yend = b1bottom + 1;
                else
                    yend = b2bottom + 1;
            }

            int
                rectx1 = xstart - p1x,      // 相交部分在图1 上的 X 坐标
                recty1 = ystart - p1y,      // 相交部分在图1 上的 Y 坐标
                rectx2 = xstart - p2x,      // 相交部分在图2 上的 X 坐标
                recty2 = ystart - p2y,      // 相交部分在图2 上的 Y 坐标
                rectw = xend - xstart,      // 相交部分宽度
                recth = yend - ystart;      // 相交部分高度

            BitmapData bmpdata1 = bmp1.LockBits(new Rectangle(0, 0, w1, h1), ImageLockMode.ReadOnly, p1f);
            BitmapData bmpdata2 = bmp2.LockBits(new Rectangle(0, 0, w2, h2), ImageLockMode.ReadOnly, p2f);

            IntPtr
                ptr1 = bmpdata1.Scan0,
                ptr2 = bmpdata2.Scan0;

            int
                stride1 = bmpdata1.Stride,
                stride2 = bmpdata2.Stride;

            for (int i = 0; i < recth; i++)
            {
                for (int j = 0; j < rectw; j++)
                {
                    if (!transparentChecker1(ptr1, j + rectx1, i + recty1, stride1) &&
                        !transparentChecker2(ptr2, j + rectx2, i + recty2, stride2))
                    {
                        bmp1.UnlockBits(bmpdata1);
                        bmp2.UnlockBits(bmpdata2);
                        return true;
                    }
                }
            }

            bmp1.UnlockBits(bmpdata1);
            bmp2.UnlockBits(bmpdata2);

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame.Utils
{
#if DEBUG
    public
#else
    internal
#endif
        class GdiUtils
    {
        public enum PlacingMode
        {
            None, Stretch, Uniform, UniformToFill
        }

        /// <summary>
        /// 处理图片 (缩放与旋转)
        /// </summary>
        /// <param name="origin">原图</param>
        /// <param name="scale">缩放</param>
        /// <param name="rotation">旋转(角度值)</param>
        /// <param name="output">输出缓冲图</param>
        public static void ProcessImage(Image origin, float scale, float rotation, out Bitmap output)
        {
            SizeF scaledSize = origin.Size * scale;
            SizeF newSize = ImgUtils.Rotate(scaledSize, rotation);
            output = new Bitmap((int)newSize.Width, (int)newSize.Height, origin.PixelFormat);

            Matrix matrix = new Matrix();
            matrix.RotateAt(rotation, new PointF(newSize.Width / 2, newSize.Height / 2));                    // rotation matrix

            PointF targetPoint = (PointF)ImgUtils.UniformOffset(newSize, scaledSize);
            using Graphics g = Graphics.FromImage(output);

            g.Transform = matrix;
            g.DrawImage(origin, (int)targetPoint.X, (int)targetPoint.Y, (int)scaledSize.Width, (int)scaledSize.Height);
        }

        public static void ProcessText(Graphics g, Font font, Brush brush, string text, float scale, float rotation, out Bitmap output)
        {
            SizeF originSize = g.MeasureString(text, font, PointF.Empty, StringFormat.GenericTypographic);
            SizeF scaledSize = originSize * scale;
            SizeF newSize = ImgUtils.Rotate(scaledSize, rotation);
            output = new Bitmap((int)newSize.Width, (int)newSize.Height, PixelFormat.Format32bppArgb);

            Matrix matrix = new Matrix();
            matrix.Scale(scale, scale);
            matrix.RotateAt(rotation, new PointF(newSize.Width / scale / 2, newSize.Height / scale / 2));

            PointF targetPoint = (PointF)ImgUtils.UniformOffset(newSize, scaledSize);
            targetPoint = new PointF(targetPoint.X / scale, targetPoint.Y / scale);
            using Graphics _g = Graphics.FromImage(output);

            _g.Transform = matrix;
            _g.DrawString(text, font, brush, targetPoint, StringFormat.GenericTypographic);
        }

        /// <summary>
        /// 在指定区域绘制指定模式放置的图片
        /// </summary>
        /// <param name="origin">原图</param>
        /// <param name="size">容器大小</param>
        /// <param name="mode">放置模式</param>
        /// <param name="output">输出图片 (大小与绘制区域大小相同)</param>
        /// <exception cref="ArgumentException"></exception>
        public static void PlaceImage(Image origin, SizeF size, PlacingMode mode, out Bitmap output)
        {
            RectangleF area = new RectangleF(PointF.Empty, size);
            SizeF childSize = origin.Size;

            RectangleF drawarea = mode switch
            {
                PlacingMode.None => ImgUtils.PlaceNone(area, childSize),
                PlacingMode.Stretch => new RectangleF(PointF.Empty, size),
                PlacingMode.Uniform => ImgUtils.PlaceUniform(area, childSize),
                PlacingMode.UniformToFill => ImgUtils.PlaceUniformToFill(area, childSize),
                _ => throw new ArgumentException("Invalid mode", nameof(mode))
            };

            output = new Bitmap((int)size.Width, (int)size.Height, origin.PixelFormat);

            using Graphics g = Graphics.FromImage(output);
            g.DrawImage(origin, (int)drawarea.X, (int)drawarea.Y, (int)drawarea.Width, (int)drawarea.Height);
        }
    }

    public static class GdiEx
    {
        public static void ScaleAndRotateAt(this Matrix m, float scaleX, float scaleY, float angle, PointF point)
        {
            m.Translate(point.X, point.Y);
            m.Scale(scaleX, scaleY);
            m.Rotate(angle);
            m.Translate(-point.X, -point.Y);
        }
    }
}

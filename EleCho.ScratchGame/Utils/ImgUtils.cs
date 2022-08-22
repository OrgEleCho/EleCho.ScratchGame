using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame.Utils
{
#if DEBUG
    public
#else
    internal
#endif
         class ImgUtils
    {
        public static SizeF UniformOffset(SizeF container, SizeF child)
        {
            return new SizeF(((float)container.Width - child.Width) / 2,
                            ((float)container.Height - child.Height) / 2);
        }

        public static RectangleF PlaceNone(RectangleF container, SizeF child)
        {
            return new RectangleF(container.Location + UniformOffset(container.Size, child), child);
        }

        public static RectangleF PlaceStretch(RectangleF container, SizeF child)
        {
            return container;
        }

        public static RectangleF PlaceUniform(RectangleF container, SizeF child)
        {
            double ratio = (double)child.Width / child.Height;          // 计算比例

            float width, height;

            SizeF uniformOffset;
            PointF uniformPoint;

            width = container.Width;
            height = (int)(width / ratio);
            if (height <= container.Height)                                                 // 如果高度不超出
            {
                uniformOffset = UniformOffset(container.Size, new SizeF(width, height));     // 居中偏移
                uniformPoint = (PointF)container.Location + uniformOffset;                          // 居中位置
                return new RectangleF(uniformPoint.X, uniformPoint.Y, width, height);
            }

            height = container.Height;
            width = (int)(height * ratio);
            uniformOffset = UniformOffset(container.Size, new SizeF(width, height));     // 居中偏移
            uniformPoint = (PointF)container.Location + uniformOffset;                          // 居中位置
            return new RectangleF(uniformPoint.X, uniformPoint.Y, width, container.Height);
        }

        public static RectangleF PlaceUniformToFill(RectangleF container, SizeF child)
        {
            double ratio = (double)child.Width / child.Height;          // 计算比例

            float width, height;

            SizeF uniformOffset;
            PointF uniformPoint;

            width = container.Width;
            height = (int)(width / ratio);
            if (height >= container.Height)                                                 // 如果高度不超出
            {
                uniformOffset = UniformOffset(container.Size, new SizeF(width, height));     // 居中偏移
                uniformPoint = (PointF)container.Location + uniformOffset;                          // 居中位置
                return new RectangleF(uniformPoint.X, uniformPoint.Y, width, height);
            }

            height = container.Height;
            width = (int)(height * ratio);
            uniformOffset = UniformOffset(container.Size, new SizeF(width, height));     // 居中偏移
            uniformPoint = (PointF)container.Location + uniformOffset;                          // 居中位置
            return new RectangleF(uniformPoint.X, uniformPoint.Y, width, container.Height);
        }

        public static RectangleF ScaleByCenter(RectangleF rectangle, float ratio)
        {
            SizeF newSize = rectangle.Size * ratio;
            SizeF offset = newSize - rectangle.Size;
            PointF newPoint = rectangle.Location - offset / 2;

            return new RectangleF(newPoint, newSize);
        }

        /// <summary>
        /// 获取矩形旋转后的大小
        /// </summary>
        /// <param name="rectSize">矩形大小</param>
        /// <param name="rotation">旋转角度(角度值)</param>
        /// <returns></returns>
        public static SizeF Rotate(SizeF rectSize, float rotation)
        {
            double width = rectSize.Width;
            double height = rectSize.Height;
            double _rotation = Degree2Radian(rotation);

            double cos = Math.Cos(_rotation);
            double sin = Math.Sin(_rotation);
            //只需要考虑到第四象限和第三象限的情况取大值(中间用绝对值就可以包括第一和第二象限)
            float newWidth = (float)(Math.Max(Math.Abs(width * cos - height * sin), Math.Abs(width * cos + height * sin)));
            float newHeight = (float)(Math.Max(Math.Abs(width * sin - height * cos), Math.Abs(width * sin + height * cos)));
            return new SizeF(newWidth, newHeight);
        }

        public static PointF RotateAt(PointF center, PointF point, float radian)
        {
            float
                x0 = center.X,
                y0 = center.Y,
                x1 = point.X,
                y1 = point.Y;

            float sina = MathF.Sin(radian);
            float cosa = MathF.Cos(radian);
            float x2 = (x1 - x0) * cosa + (y1 - y0) * sina + x0;
            float y2 = (y1 - y0) * cosa - (x1 - x0) * sina + y0;

            return new PointF(x2, y2);
        }

        public static void Rotate(float x, float y, float radian, out float outx, out float outy)
        {
            float sina = MathF.Sin(radian);
            float cosa = MathF.Cos(radian);
            outx = (x) * cosa + (y) * sina;
            outy = (y) * cosa - (x) * sina;
        }

        public static PointF[] RotateAt(PointF center, PointF[] points, float radian)
        {
            int length = points.Length;
            PointF[] result = new PointF[length];
            for (int i = 0; i < length; i++)
                result[i] = RotateAt(center, points[i], radian);
            return result;
        }

        public static PointF[] RotateAt(PointF center, RectangleF rect, float radian)
        {
            float left = rect.Left;
            float top = rect.Top;
            float right = rect.Right;
            float bottom = rect.Bottom;

            return new PointF[]
            {
                RotateAt(center, new PointF(left, top), radian),
                RotateAt(center, new PointF(right, top), radian),
                RotateAt(center, new PointF(right, bottom), radian),
                RotateAt(center, new PointF(left, bottom), radian),
            };
        }


        public static float Degree2Radian(float degree) => degree * MathF.PI / 180;
        public static float Radian2Degree(float radian) => radian * 180 / MathF.PI;
    }
}

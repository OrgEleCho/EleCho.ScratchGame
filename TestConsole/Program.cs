using EleCho.ScratchGame;
using EleCho.ScratchGame.Utils;
using PolygonIntersection;

Console.WriteLine(ImgUtils.RotateAt(new PointF(0, 0), new PointF(0, 10), ImgUtils.Degree2Radian(90)));
Console.WriteLine(Polygon.IsIn(new PointF[]
{
    new Point(0, 1),
    new Point(1, -1),
    new Point(-1, -1),
}, new PointF(0, 0)));

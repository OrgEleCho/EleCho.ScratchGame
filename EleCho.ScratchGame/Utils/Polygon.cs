using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Numerics;

namespace PolygonIntersection
{
#if DEBUG
    public
#else
    internal
#endif
        static class Polygon
    {

        private static void ProjectPolygon(Vector2 axis, Vector2[] points, ref float min, ref float max)
        {
            float d = Vector2.Dot(axis, points[0]);
            min = d;
            max = d;
            for (int i = 0; i < points.Length; i++)
            {
                d = Vector2.Dot(points[i], axis);
                if (d < min)
                {
                    min = d;
                }
                else
                {
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
        }
        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        public static bool IsCollided(PointF[] polygon1, PointF[] polygon2)
        {
            Vector2[] points1 = new Vector2[polygon1.Length];
            Vector2[] edges1 = new Vector2[polygon1.Length];
            Vector2[] points2 = new Vector2[polygon2.Length];
            Vector2[] edges2 = new Vector2[polygon2.Length];

            for (int i = 0; i < polygon1.Length; i++)
                points1[i] = (Vector2)polygon1[i];
            for (int i = 0; i < polygon2.Length; i++)
                points2[i] = (Vector2)polygon2[i];

            int end1 = polygon1.Length - 1;
            for (int i = 0; i < end1; i++)
            {
                Vector2 p1 = points1[i];
                Vector2 p2 = points1[i + 1];

                edges1[i] = (p2 - p1);
            }

            int end2 = polygon2.Length - 1;
            for (int i = 0; i < end2; i++)
            {
                Vector2 p1 = points2[i];
                Vector2 p2 = points2[i + 1];

                edges2[i] = (p2 - p1);
            }

            edges1[polygon1.Length - 1] = edges1[0] - edges1[polygon1.Length - 1];
            edges2[polygon2.Length - 1] = edges2[0] - edges2[polygon2.Length - 1];

            bool result = true;

            int edgeCountA = edges1.Length;
            int edgeCountB = edges2.Length;
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = edges1[edgeIndex];
                }
                else
                {
                    edge = edges2[edgeIndex - edgeCountA];
                }

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = Vector2.Normalize(new Vector2(-edge.Y, edge.X));

                // Find the projection of the polygon on the current axis
                float minA = 0;
                float minB = 0;
                float maxA = 0;
                float maxB = 0;
                ProjectPolygon(axis, points1, ref minA, ref maxA);
                ProjectPolygon(axis, points2, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0)
                    result = false;


                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result)
                    break;
            }

            return result;
        }
        public static bool IsIn(PointF[] polygon, PointF point)
        {
            int length = polygon.Length;
            float[] coefs = new float[length];
            for (int i = 1; i < length; i++)
            {
                PointF p = polygon[i];
                float c =
                    (point.Y - polygon[i].Y) * (p.X - polygon[i].X) -
                    (point.X - polygon[i].X) * (p.Y - polygon[i].Y);
                if (c == 0)
                    return true;
                coefs[i] = c;
            }

            int coefCount = coefs.Length;
            for (int i = 1; i < coefCount; i++)
            {
                if (coefs[i] * coefs[i - 1] < 0)
                    return false;
            }

            return true;
        }
    }
}


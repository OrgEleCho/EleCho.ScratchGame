using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Numerics;

namespace PolygonIntersection {

#if DEBUG
	public
#else
    internal
#endif
		 class Polygon {

        private readonly Vector2[] points;
		private readonly Vector2[] edges;

        public Polygon(PointF[] vertexes)
		{
			this.points = new Vector2[vertexes.Length];
			this.edges = new Vector2[vertexes.Length];
            for (int i = 0; i < vertexes.Length; i++)
            {
				this.points[i] = (Vector2)vertexes[i];
            }

			Vector2 p1;
			Vector2 p2;
			for (int i = 0; i < vertexes.Length - 1; i++)
			{
				p1 = this.points[i];
				p2 = this.points[i + 1];

				edges[i] = (p2 - p1);
			}

            edges[vertexes.Length - 1] = edges[0] - edges[vertexes.Length - 1];
		}

		public static bool Collision(Polygon a, Polygon b)
		{
			bool result = true;

			int edgeCountA = a.edges.Length;
			int edgeCountB = b.edges.Length;
			Vector2 edge;

			// Loop through all the edges of both polygons
			for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
			{
				if (edgeIndex < edgeCountA)
				{
					edge = a.edges[edgeIndex];
				}
				else
				{
					edge = b.edges[edgeIndex - edgeCountA];
				}

				// ===== 1. Find if the polygons are currently intersecting =====

				// Find the axis perpendicular to the current edge
				Vector2 axis = Vector2.Normalize(new Vector2(-edge.Y, edge.X));

				// Find the projection of the polygon on the current axis
				float minA = 0;
				float minB = 0;
				float maxA = 0;
				float maxB = 0;
				ProjectPolygon(axis, a, ref minA, ref maxA);
				ProjectPolygon(axis, b, ref minB, ref maxB);

				// Check if the polygon projections are currentlty intersecting
				if (IntervalDistance(minA, maxA, minB, maxB) > 0)
					result = false;


				// If the polygons are not intersecting and won't intersect, exit the loop
				if (!result)
					break;
			}

			return result;
		}
		private static void ProjectPolygon(Vector2 axis, Polygon polygon, ref float min, ref float max)
		{
			// To project a point on an axis use the dot product
			float d = Vector2.Dot(axis, polygon.points[0]);
			min = d;
			max = d;
			for (int i = 0; i < polygon.points.Length; i++)
			{
				d = Vector2.Dot(polygon.points[i], axis);
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
	}
}


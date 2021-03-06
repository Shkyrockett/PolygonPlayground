// <copyright file="MathematicsD.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using static System.Math;

namespace PolygonLibrary
{
    /// <summary>
    /// The Mathematics class.
    /// </summary>
    public static class MathematicsD
    {
        #region Constants
        /// <summary>
        /// The one third constant.
        /// </summary>
        public const double OneThird = 1d / 3d;

        /// <summary>
        /// The one half constant.
        /// </summary>
        public const double OneHalf = 1d / 2d;

        /// <summary>
        /// The two thirds constant.
        /// </summary>
        public const double TwoThirds = 2d / 3d;

        /// <summary>
        /// The scale per mouse wheel delta.
        /// </summary>
        public const double scale_per_delta = 0.1d / 120d;
        #endregion Constants

        #region Distance Methods
        /// <summary>
        /// Calculates the distance between two points in 2-dimensional euclidean space.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>
        /// Returns the distance between two points.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(PointF point1, PointF point2)
        {
            var dx = point2.X - point1.X;
            var dy = point2.Y - point1.Y;
            return Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// Calculate the distance squared between two points.
        /// </summary>
        /// <param name="point1">First point.</param>
        /// <param name="point2">Second point.</param>
        /// <returns>
        /// Returns the squared distance between two points.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceSquared(PointF point1, PointF point2)
        {
            var dx = point1.X - point2.X;
            var dy = point1.Y - point2.Y;
            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// Calculate the distance between point pt and the line segment p1 --&gt; p2.
        /// </summary>
        /// <param name="point">The <paramref name="point" />.</param>
        /// <param name="seg1">The <paramref name="seg1" />.</param>
        /// <param name="seg2">The <paramref name="seg2" />.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double Distnce, PointF Point) DistanceToLineSegment(PointF point, PointF seg1, PointF seg2)
        {
            var (dx, dy) = (seg2.X - seg1.X, seg2.Y - seg1.Y);
            if ((dx == 0f) && (dy == 0f))
            {
                // It's a point not a line segment.
                (dx, dy) = (point.X - seg1.X, point.Y - seg1.Y);
                return (Sqrt((dx * dx) + (dy * dy)), seg1);
            }

            // Calculate the t that minimizes the distance.
            var t = (((point.X - seg1.X) * dx) + ((point.Y - seg1.Y) * dy)) / ((dx * dx) + (dy * dy));
            PointF nearest;

            // See if this represents one of the segment's end points or a point in the middle.
            if (t < 0f)
            {
                (dx, dy, nearest) = (point.X - seg1.X, point.Y - seg1.Y, seg1);
            }
            else if (t > 1f)
            {
                (dx, dy, nearest) = (point.X - seg2.X, point.Y - seg2.Y, seg2);
            }
            else
            {
                nearest = new PointF(seg1.X + (t * dx), seg1.Y + (t * dy));
                (dx, dy) = (point.X - nearest.X, point.Y - nearest.Y);
            }

            return (Sqrt((dx * dx) + (dy * dy)), nearest);
        }

        /// <summary>
        /// Calculate the distance squared between point pt and the line segment p1 --&gt; p2.
        /// </summary>
        /// <param name="point">The <paramref name="point" />.</param>
        /// <param name="seg1">The <paramref name="seg1" />.</param>
        /// <param name="seg2">The <paramref name="seg2" />.</param>
        /// <returns>
        /// The <see cref="double" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (double Distnce, PointF Point) DistanceToLineSegmentSquared(PointF point, PointF seg1, PointF seg2)
        {
            var (dx, dy) = (seg2.X - seg1.X, seg2.Y - seg1.Y);
            if ((dx == 0f) && (dy == 0f))
            {
                // It's a point not a line segment.
                dx = point.X - seg1.X;
                dy = point.Y - seg1.Y;
                return ((dx * dx) + (dy * dy), seg1);
            }

            // Calculate the t that minimizes the distance.
            var t = (((point.X - seg1.X) * dx) + ((point.Y - seg1.Y) * dy)) / ((dx * dx) + (dy * dy));
            PointF nearest;

            // See if this represents one of the segment's end points or a point in the middle.
            if (t < 0f)
            {
                (dx, dy, nearest) = (point.X - seg1.X, point.Y - seg1.Y, seg1);
            }
            else if (t > 1f)
            {
                (dx, dy, nearest) = (point.X - seg2.X, point.Y - seg2.Y, seg2);
            }
            else
            {
                nearest = new PointF(seg1.X + (t * dx), seg1.Y + (t * dy));
                (dx, dy) = (point.X - nearest.X, point.Y - nearest.Y);
            }

            return ((dx * dx) + (dy * dy), nearest);
        }
        #endregion Distance Methods

        #region Contains Methods
        /// <summary>
        /// Determines whether the specified point is contained withing the set of regions defined by this Polygon.
        /// </summary>
        /// <param name="polygon">List of polygons.</param>
        /// <param name="point">The test point.</param>
        /// <param name="epsilon">The epsilon.</param>
        /// <returns>
        /// Returns <see cref="Inclusions.Outside" /> (0) if false, <see cref="Inclusions.Inside" /> (+1) if true, <see cref="Inclusions.Boundary" /> (-1) if the point is on a polygon boundary.
        /// </returns>
        /// <exception cref="ArgumentNullException">polygon</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Inclusions PolygonContainsPoint(List<PolygonContour> polygon, PointF point, double epsilon = double.Epsilon)
        {
            if (polygon is null)
            {
                throw new ArgumentNullException(nameof(polygon));
            }

            var returnValue = Inclusions.Outside;

            foreach (var poly in polygon)
            {
                // Use alternating rule with XOR to determine if the point is in a polygon or a hole.
                // If the point is in an odd number of polygons, it is inside. If even, it is a hole.
                returnValue ^= PolygonContourContainsPoint(poly, point, epsilon);

                // Any point on any boundary is on a boundary.
                if (returnValue == Inclusions.Boundary)
                {
                    return Inclusions.Boundary;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Determines whether the specified point is contained withing the region defined by this <see cref="PolygonContour" />.
        /// </summary>
        /// <param name="points">The points that form the corners of the polygon.</param>
        /// <param name="point">The test point.</param>
        /// <param name="epsilon">The <paramref name="epsilon" /> or minimal value to represent a change.</param>
        /// <returns>
        /// Returns <see cref="Inclusions.Outside" /> (0) if false, <see cref="Inclusions.Inside" /> (+1) if true, <see cref="Inclusions.Boundary" /> (-1) if the point is on a polygon boundary.
        /// </returns>
        /// <acknowledgment>
        /// Adapted from Clipper library: http://www.angusj.com/delphi/clipper.php
        /// See "The Point in Polygon Problem for Arbitrary Polygons" by Hormann and Agathos
        /// http://www.inf.usi.ch/hormann/papers/Hormann.2001.TPI.pdf
        /// </acknowledgment>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Inclusions PolygonContourContainsPoint(PolygonContour points, PointF point, double epsilon = double.Epsilon)
        {
            // Default value is no inclusion.
            var result = Inclusions.Outside;
            if (points is null) return result;

            // Special cases for points and line segments.
            if (points?.Count < 3)
            {
                if (points.Count == 1)
                {
                    // If the polygon has 1 point, it is a point and has no interior, but a point can intersect a point.
                    return (point.X == points[0].X && point.Y == points[0].Y) ? Inclusions.Boundary : Inclusions.Outside;
                }
                else if (points.Count == 2)
                {
                    // If the polygon has 2 points, it is a line and has no interior, but a point can intersect a line.
                    return ((point.X == points[0].X) && (point.Y == points[0].Y))
                        || ((point.X == points[1].X) && (point.Y == points[1].Y))
                        || (((point.X > points[0].X) == (point.X < points[1].X))
                        && ((point.Y > points[0].Y) == (point.Y < points[1].Y))
                        && ((point.X - points[0].X) * (points[1].Y - points[0].Y) == (point.Y - points[0].Y) * (points[1].X - points[0].X))) ? Inclusions.Boundary : Inclusions.Outside;
                }
                else
                {
                    // Empty geometry.
                    return Inclusions.Outside;
                }
            }

            // Loop through each line segment.
            var curPoint = points![0];
            for (var i = 1; i <= points.Count; ++i)
            {
                var nextPoint = i == points.Count ? points[0] : points[i];

                // Special case for horizontal lines. Check whether the point is on one of the ends, or whether the point is on the segment, if the line is horizontal.
                if (curPoint.Y == point.Y && (curPoint.X == point.X || ((nextPoint.Y == point.Y) && ((curPoint.X > point.X) == (nextPoint.X < point.X)))))
                //if ((Abs(nextPoint.Y - pY) < epsilon) && ((Abs(nextPoint.X - pX) < epsilon) || (Abs(curPoint.Y - pY) < epsilon && ((nextPoint.X > pX) == (curPoint.X < pX)))))
                {
                    return Inclusions.Boundary;
                }

                // If Point between start and end points horizontally.
                //if ((curPoint.Y < pY) == (nextPoint.Y >= pY))
                if ((nextPoint.Y < point.Y) != (curPoint.Y < point.Y)) // At least one point is below the Y threshold and the other is above or equal
                {
                    // Optimization: at least one point must be to the right of the test point
                    // If point between start and end points vertically.
                    if (nextPoint.X >= point.X)
                    {
                        if (curPoint.X > point.X)
                        {
                            result = 1 - result;
                        }
                        else
                        {
                            var determinant = ((nextPoint.X - point.X) * (curPoint.Y - point.Y)) - ((curPoint.X - point.X) * (nextPoint.Y - point.Y));
                            if (Abs(determinant) < epsilon)
                            {
                                return Inclusions.Boundary;
                            }
                            else if ((determinant > 0) == (curPoint.Y > nextPoint.Y))
                            {
                                result = 1 - result;
                            }
                        }
                    }
                    else if (curPoint.X > point.X)
                    {
                        var determinant = ((nextPoint.X - point.X) * (curPoint.Y - point.Y)) - ((curPoint.X - point.X) * (nextPoint.Y - point.Y));
                        if (Abs(determinant) < epsilon)
                        {
                            return Inclusions.Boundary;
                        }

                        if ((determinant > 0) == (curPoint.Y > nextPoint.Y))
                        {
                            result = 1 - result;
                        }
                    }
                }

                curPoint = nextPoint;
            }

            return result;
        }
        #endregion Contains Methods

        /// <summary>
        /// Scales the factor.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <param name="delta">The delta.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double MouseWheelScaleFactor(double scale, int delta)
        {
            scale += delta * scale_per_delta;
            return (scale <= 0d) ? 2d * double.Epsilon : scale;
        }

        #region Point Manipulation Methods
        /// <summary>
        /// Inverses the scale point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ScreenToObject_(PointF point, double scale)
        {
            var invScale = 1d / scale;
            return new((float)(invScale * point.X), (float)(invScale * point.Y));
        }

        /// <summary>
        /// Screens to object.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ScreenToObject(PointF point, double scale) => new((float)(point.X / scale), (float)(point.Y / scale));

        /// <summary>
        /// Inverses the translation and scale of a point.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="point">The point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ScreenToObject_(PointF offset, PointF point, double scale)
        {
            var invScale = 1f / scale;
            return new PointF((float)((point.X - offset.X) * invScale), (float)((point.Y - offset.Y) * invScale));
        }

        /// <summary>
        /// Screens to object. https://stackoverflow.com/a/37269366
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="point">The screen point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ScreenToObject(PointF offset, PointF point, double scale) => new((float)((point.X - offset.X) / scale), (float)((point.Y - offset.Y) / scale));

        /// <summary>
        /// Screens to object transposed matrix.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="screenPoint">The screen point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ScreenToObjectTransposedMatrix_(PointF offset, PointF screenPoint, double scale)
        {
            var invScale = 1d / scale;
            return new((float)((screenPoint.X * invScale) - offset.X), (float)((screenPoint.Y * invScale) - offset.Y));
        }

        /// <summary>
        /// Screens to object transposed matrix. https://stackoverflow.com/a/37269366
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="screenPoint">The screen point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ScreenToObjectTransposedMatrix(PointF offset, PointF screenPoint, double scale) => new((float)((screenPoint.X / scale) - offset.X), (float)((screenPoint.Y / scale) - offset.Y));

        /// <summary>
        /// Objects to screen.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ObjectToScreen(PointF point, double scale) => new((float)(point.X * scale), (float)(point.Y * scale));

        /// <summary>
        /// Objects to screen. https://stackoverflow.com/a/37269366
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="point">The object point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ObjectToScreen(PointF offset, PointF point, double scale) => new((float)(offset.X + (point.X * scale)), (float)(offset.Y + (point.Y * scale)));

        /// <summary>
        /// Objects to screen transposed matrix. https://stackoverflow.com/a/37269366
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="objectPoint">The object point.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ObjectToScreenTransposedMatrix(PointF offset, PointF objectPoint, double scale) => new((float)((offset.X + objectPoint.X) * scale), (float)((offset.Y + objectPoint.Y) * scale));

        /// <summary>
        /// Zooms at. https://stackoverflow.com/a/37269366
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="cursor">The cursor.</param>
        /// <param name="previousScale">The previous scale.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ZoomAt(PointF offset, PointF cursor, double previousScale, double scale)
        {
            var point = ScreenToObject(offset, cursor, previousScale);
            point = ObjectToScreen(offset, point, scale);
            return new((float)(offset.X + ((cursor.X - point.X) / scale)), (float)(offset.Y + ((cursor.Y - point.Y) / scale)));
        }

        /// <summary>
        /// Zooms at for a transposed matrix. https://stackoverflow.com/a/37269366
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="cursor">The cursor.</param>
        /// <param name="previousScale">The previous scale.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF ZoomAtTransposedMatrix(PointF offset, PointF cursor, double previousScale, double scale)
        {
            var point = ScreenToObjectTransposedMatrix(offset, cursor, previousScale);
            point = ObjectToScreenTransposedMatrix(offset, point, scale);
            return new((float)(offset.X + ((cursor.X - point.X) / scale)), (float)(offset.Y + ((cursor.Y - point.Y) / scale)));
        }
        #endregion Point Manipulation Methods
    }
}

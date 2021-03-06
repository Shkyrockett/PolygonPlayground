// <copyright file="WinformsExtentions.cs">
//     Copyright © 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using System.Drawing;
using PolygonLibrary;

namespace PolygonPlayground
{
    /// <summary>
    /// 
    /// </summary>
    public static class WinformsExtentions
    {
        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this IGeometry geometry, Graphics graphics, Brush brush, Pen pen)
        {
            switch (geometry)
            {
                case Group g:
                    DrawGeometry(g, graphics, brush, pen);
                    break;
                case Polygon g:
                    DrawGeometry(g, graphics, brush, pen);
                    break;
                case PolygonContour g:
                    DrawGeometry(g, graphics, brush, pen);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this Group geometry, Graphics graphics, Brush brush, Pen pen)
        {
            foreach (var shape in geometry)
            {
                shape.DrawGeometry(graphics, brush, pen);
            }
        }

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this Polygon geometry, Graphics graphics, Brush brush, Pen pen)
        {
            foreach (var shape in geometry)
            {
                shape.DrawGeometry(graphics, brush, pen);
            }
        }

        /// <summary>
        /// Draws the geometry.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        public static void DrawGeometry(this PolygonContour geometry, Graphics graphics, Brush brush, Pen pen)
        {
            if (geometry.Count > 0)
            {
                if (geometry.Count > 2)
                {
                    if (brush is Brush b && b != Brushes.Transparent) graphics.FillPolygon(b, geometry.Points.ToArray());
                    if (pen is Pen p && p != Pens.Transparent) graphics.DrawPolygon(p, geometry.Points.ToArray());
                }
                else if (geometry.Count > 1)
                {
                    if (pen is Pen p && p != Pens.Transparent) graphics.DrawLines(pen, geometry.ToArray());
                }
                else
                {
                    // Draw Point here.
                }
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this IGeometry geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            switch (geometry)
            {
                case Group g:
                    DrawNodes(g, graphics, brush, pen, radius);
                    break;
                case Polygon g:
                    DrawNodes(g, graphics, brush, pen, radius);
                    break;
                case PolygonContour g:
                    DrawNodes(g, graphics, brush, pen, radius);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this Group geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            foreach (var shape in geometry)
            {
                shape.DrawNodes(graphics, brush, pen, radius);
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this Polygon geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            foreach (var shape in geometry)
            {
                shape.DrawNodes(graphics, brush, pen, radius);
            }
        }

        /// <summary>
        /// Draws the nodes.
        /// </summary>
        /// <param name="geometry">The geometry.</param>
        /// <param name="graphics">The graphics.</param>
        /// <param name="brush">The brush.</param>
        /// <param name="pen">The pen.</param>
        /// <param name="radius">The radius.</param>
        public static void DrawNodes(this PolygonContour geometry, Graphics graphics, Brush brush, Pen pen, int radius)
        {
            if (geometry.Count > 0)
            {
                foreach (var corner in geometry)
                {
                    var rect = new RectangleF(corner.X - radius, corner.Y - radius, (2f * radius) + 1f, (2f * radius) + 1f);
                    graphics.FillEllipse(brush, rect);
                    graphics.DrawEllipse(pen, rect);
                }
            }
        }
    }
}

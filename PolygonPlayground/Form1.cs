﻿// <copyright file="Form1.cs">
//     Copyright © 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

using PolygonLibrary;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using static PolygonLibrary.Mathematics;

namespace PolygonPlayground
{
    /// <summary>
    /// The form1 class.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public partial class Form1
        : Form
    {
        #region Constants
        /// <summary>
        /// The "size" of an object for mouse over purposes.
        /// </summary>
        private const int objectRadius = 3;

        /// <summary>
        /// We're over an object if the distance squared
        /// between the mouse and the object is less than this.
        /// </summary>
        private const int objectRadiusSquared = objectRadius * objectRadius;

        /// <summary>
        /// The new polygon stroke
        /// </summary>
        private static readonly Pen newPolygonStroke = Pens.Green;

        /// <summary>
        /// The new polygon dashed stroke
        /// </summary>
        private static readonly Pen newPolygonDashedStroke = new Pen(Color.Green)
        {
            DashPattern = new float[] { 3f, 3f }
        };

        /// <summary>
        /// The polygon fill
        /// </summary>
        private static readonly Brush polygonFill = Brushes.AliceBlue;

        /// <summary>
        /// The polygon stroke
        /// </summary>
        private static readonly Pen polygonStroke = Pens.LightBlue;

        /// <summary>
        /// The polygon node fill
        /// </summary>
        private static readonly Brush polygonNodeFill = Brushes.LightGoldenrodYellow;

        /// <summary>
        /// The polygon node stroke
        /// </summary>
        private static readonly Pen polygonNodeStroke = Pens.Tan;
        #endregion Constants

        #region Fields
        /// <summary>
        /// The group
        /// </summary>
        private Group group;

        /// <summary>
        /// The new polygon.
        /// </summary>
        private PolygonContour newPolygon;

        /// <summary>
        /// The new point.
        /// </summary>
        private PointF newPoint;

        /// <summary>
        /// The polygon and index of the corner we are moving.
        /// </summary>
        private IGeometry<PointF> movingPolygon;

        /// <summary>
        /// The moving point.
        /// </summary>
        private int movingPoint = -1;

        /// <summary>
        /// The offset x.
        /// </summary>
        private float offsetX;

        /// <summary>
        /// The offset y.
        /// </summary>
        private float offsetY;

        /// <summary>
        /// The panning
        /// </summary>
        private bool panning;

        /// <summary>
        /// The starting point
        /// </summary>
        private PointF startingPoint = PointF.Empty;

        /// <summary>
        /// The pan point
        /// </summary>
        private PointF panPoint = PointF.Empty;

        /// <summary>
        /// The scale.
        /// </summary>
        private float scale = 1f;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Form1" /> class.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Form1()
        {
            TypeDescriptor.AddAttributes(typeof(PointF));

            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the Load event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the Paint event of the PicCanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PaintEventArgs" /> instance containing the event data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PicCanvas_Paint(object sender, PaintEventArgs e)
        {
            //base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(picCanvas.BackColor);
            g.ScaleTransform(scale, scale);
            g.TranslateTransform(panPoint.X, panPoint.Y);

            //// Draw Polygon Bounds for Envelope Reference
            //g.DrawRectangles(envelopeBoundsStroke, new RectangleF[] { polygonsBounds });

            if (group is Group shapes)
            {
                // Draw the old polygons.
                foreach (var shape in shapes)
                {
                    if (shape.Count > 0)
                    {
                        // Draw the polygon.
                        shape.DrawGeometry(g, polygonFill, polygonStroke);
                        // Draw the corners.
                        shape.DrawNodes(g, polygonNodeFill, polygonNodeStroke, objectRadius);
                    }
                }
            }

            // Draw the new polygon.
            if (newPolygon != null)
            {
                if (newPolygon.Count > 1)
                {
                    // Draw the new polygon.
                    g.DrawLines(newPolygonStroke, newPolygon.ToArray());
                }
                if (newPolygon.Count > 0)
                {
                    // Draw the newest edge.
                    g.DrawLine(newPolygonDashedStroke, newPolygon[^1], newPoint);
                }
            }

            g.ResetTransform();
            g.DrawString($"Pan Point: {panPoint}\n\rScale: {scale}\n\rMouse Pos: {MousePosition}", this.Font, Brushes.Black, new PointF(0f, 0f));
        }

        /// <summary>
        /// Handles the MouseDown event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PicCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            var translateScalePoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);
            var scalePoint = ScreenToObject(e.Location, scale);

            // See what we're over.
            if (e.Button == MouseButtons.Left)
            {
                (bool Success, IGeometry<PointF> HitPolygon) polygonResult;
                (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) cornerResult;
                (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) edgeResult;

                if (newPolygon != null)
                {
                    newPoint = translateScalePoint;
                }
                else if ((cornerResult = MouseIsOverCornerPoint(translateScalePoint, group)).Success)
                {
                    // Start dragging this corner.
                    picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                    picCanvas.MouseMove += PicCanvas_MouseMove_MovingCorner;
                    picCanvas.MouseUp += PicCanvas_MouseUp_MovingCorner;
                    picCanvas.MouseDoubleClick += PicCanvas_MouseDoubleClick_Corner;

                    // Remember the polygon and point number.
                    movingPolygon = cornerResult.HitPolygon;
                    movingPoint = cornerResult.HitPoint;

                    // Remember the offset from the mouse to the point.
                    offsetX = cornerResult.HitPolygon[cornerResult.HitPoint].X - scalePoint.X;
                    offsetY = cornerResult.HitPolygon[cornerResult.HitPoint].Y - scalePoint.Y;
                }
                else if ((edgeResult = MouseIsOverEdge(translateScalePoint, group)).Success)
                {
                    // Add a point.
                    edgeResult.HitPolygon.Insert(edgeResult.HitPoint1 + 1, edgeResult.NearestPoint);
                }
                else if ((polygonResult = MouseIsOverPolygon(translateScalePoint, group)).Success)
                {
                    // Start moving this polygon.
                    picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                    picCanvas.MouseMove += PicCanvas_MouseMove_MovingPolygon;
                    picCanvas.MouseUp += PicCanvas_MouseUp_MovingPolygon;
                    picCanvas.MouseDoubleClick += PicCanvas_MouseDoubleClick_Polygon;

                    // Remember the polygon.
                    movingPolygon = (PolygonContour)polygonResult.HitPolygon;

                    // Remember the offset from the mouse to the segment's first point.
                    offsetX = polygonResult.HitPolygon[0].X - scalePoint.X;
                    offsetY = polygonResult.HitPolygon[0].Y - scalePoint.Y;
                }
                else
                {
                    // Start a new polygon.
                    newPolygon = new PolygonContour();
                    newPoint = translateScalePoint;
                    newPolygon.Add(translateScalePoint);

                    // Get ready to work on the new polygon.
                    picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                    picCanvas.MouseMove += PicCanvas_MouseMove_Drawing;
                    picCanvas.MouseUp += PicCanvas_MouseUp_Drawing;
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                panning = true;
                startingPoint = translateScalePoint;
                panPoint = ScreenToObjectTransposedMatrix(startingPoint, e.Location, scale);

                // Get ready to work on the new polygon.
                picCanvas.MouseMove -= PicCanvas_MouseMove_NotDrawing;
                picCanvas.MouseMove += PicCanvas_MouseMove_Panning;
                picCanvas.MouseUp += PicCanvas_MouseUp_Panning;
            }

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// See if we're over a polygon or corner point.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" />.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_NotDrawing(object sender, MouseEventArgs e)
        {
            var mousePoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);
            picCanvas.Cursor =
                MouseIsOverCornerPoint(mousePoint, group).Success ? Cursors.Arrow :
                MouseIsOverEdge(mousePoint, group).Success ? Cursors.Cross :
                MouseIsOverPolygon(mousePoint, group).Success ? Cursors.Hand :
                Cursors.Cross;
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Move the next point in the new polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" />.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_Drawing(object sender, MouseEventArgs e)
        {
            newPoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Finish moving the selected polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" />.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseUp_Drawing(object sender, MouseEventArgs e)
        {
            var mousePoint = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);

            // We are already drawing a polygon.
            // If it's the right mouse button, finish this polygon.
            if (e.Button == MouseButtons.Right)
            {
                // Finish this polygon.
                if (newPolygon?.Count > 2)
                {
                    if (group is not Group)
                    {
                        group = new Group();
                    }

                    group.Add(newPolygon);
                }

                newPolygon = null;

                // We no longer are drawing.
                picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
                picCanvas.MouseMove -= PicCanvas_MouseMove_Drawing;
                picCanvas.MouseUp -= PicCanvas_MouseUp_Drawing;
            }
            else
            {
                // Add a point to this polygon.
                if (newPolygon?[^1] != mousePoint)
                {
                    newPolygon.Add(mousePoint);
                }
            }

            //polygonsBounds = PolygonBounds(polygons).Value;
            //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Move the selected corner.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" />.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_MovingCorner(object sender, MouseEventArgs e)
        {
            var mousePoint = ScreenToObject(e.Location, scale);

            // Move the point.
            movingPolygon[movingPoint] = new PointF(mousePoint.X + offsetX, mousePoint.Y + offsetY);

            //if (movingPolygon is IEnvelope)
            //{
            //    envelope = (IEnvelope)movingPolygon;
            //}

            //polygonsBounds = PolygonBounds(polygons).Value;
            //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Finish moving the selected corner.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" />.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseUp_MovingCorner(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingCorner;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingCorner;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Corner;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the PicCanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void PicCanvas_MouseDoubleClick_Corner(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingCorner;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingCorner;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Corner;

            var (Success, movingPolygon, movingPoint) = MouseIsOverCornerPoint(ScreenToObjectTransposedMatrix(panPoint, e.Location, scale), group);

            if (Success)
            {
                movingPolygon.RemoveAt(movingPoint);
                if (movingPolygon.Count == 0)
                {
                    group.Remove(movingPolygon as PolygonContour);
                }

                //polygonsBounds = PolygonBounds(polygons).Value;
                //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Move the selected polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" />.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseMove_MovingPolygon(object sender, MouseEventArgs e)
        {
            var mousePoint = ScreenToObject(e.Location, scale);

            // See how far the first point will move.
            var dx = mousePoint.X + offsetX - movingPolygon[0].X;
            var dy = mousePoint.Y + offsetY - movingPolygon[0].Y;

            if (dx == 0 && dy == 0)
            {
                return;
            }

            // Move the polygon.
            for (var i = 0; i < movingPolygon.Count; i++)
            {
                movingPolygon[i] = new PointF(movingPolygon[i].X + dx, movingPolygon[i].Y + dy);
            }

            //polygonsBounds = PolygonBounds(polygons).Value;
            //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Finish moving the selected polygon.
        /// </summary>
        /// <param name="sender">The <paramref name="sender" />.</param>
        /// <param name="e">The mouse event arguments.</param>
        private void PicCanvas_MouseUp_MovingPolygon(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingPolygon;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingPolygon;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Polygon;
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the PicCanvas control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void PicCanvas_MouseDoubleClick_Polygon(object sender, MouseEventArgs e)
        {
            var mouse_pt = ScreenToObjectTransposedMatrix(panPoint, e.Location, scale);

            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_MovingPolygon;
            picCanvas.MouseUp -= PicCanvas_MouseUp_MovingPolygon;
            picCanvas.MouseDoubleClick -= PicCanvas_MouseDoubleClick_Polygon;

            var (Success, movingPolygon) = MouseIsOverPolygon(mouse_pt, group);
            if (Success)
            {
                group.Remove(movingPolygon as PolygonContour);

                //polygonsBounds = PolygonBounds(polygons).Value;
                //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Handles the Panning event of the PicCanvas_MouseMove control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void PicCanvas_MouseMove_Panning(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle && panning)
            {
                panPoint = ScreenToObjectTransposedMatrix(startingPoint, e.Location, scale);

                //polygonsBounds = PolygonBounds(polygons).Value;
                //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Handles the MouseUp Panning event of the PicCanvas_MouseUp control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        private void PicCanvas_MouseUp_Panning(object sender, MouseEventArgs e)
        {
            picCanvas.MouseMove += PicCanvas_MouseMove_NotDrawing;
            picCanvas.MouseMove -= PicCanvas_MouseMove_Panning;
            picCanvas.MouseUp -= PicCanvas_MouseUp_Panning;

            if (e.Button == MouseButtons.Middle && panning)
            {
                panning = false;

                //polygonsBounds = PolygonBounds(polygons).Value;
                //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

                // Redraw.
                picCanvas.Invalidate();
            }
        }

        /// <summary>
        /// Handles the MouseWheel event of the Form1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseEventArgs" /> instance containing the event data.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PicCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            var previousScale = scale;
            scale = MouseWheelScaleFactor(scale, e.Delta);
            scale = scale < scale_per_delta ? scale_per_delta : scale;

            panPoint = ZoomAtTransposedMatrix(panPoint, e.Location, previousScale, scale);

            //polygonsBounds = PolygonBounds(polygons).Value;
            //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Handles the Click event of the ButtonResetPan control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ButtonResetPan_Click(object sender, EventArgs e)
        {
            panPoint = new PointF(0f, 0f);

            //polygonsBounds = PolygonBounds(polygons).Value;
            //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }

        /// <summary>
        /// Handles the Click event of the ButtonResetScale control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ButtonResetScale_Click(object sender, EventArgs e)
        {
            scale = 1;

            //polygonsBounds = PolygonBounds(polygons).Value;
            //polygonsDistorted = Distort(polygons, polygonsBounds, envelope);

            // Redraw.
            picCanvas.Invalidate();
        }
        #endregion Event Handlers

        #region Methods
        /// <summary>
        /// See if the mouse is over a corner point.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="group">The group.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, Group group)
        {
            if (group is Group shapes)
            {
                // See if we're over a corner point.
                foreach (var shape in shapes)
                {
                    switch (shape)
                    {
                        case Group g:
                            {
                                var result = MouseIsOverCornerPoint(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        case Polygon g:
                            {
                                var result = MouseIsOverCornerPoint(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        case PolygonContour g:
                            {
                                var result = MouseIsOverCornerPoint(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return (false, null, -1);
        }

        /// <summary>
        /// See if the mouse is over a corner point.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="polygon">The polygon.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, Polygon polygon)
        {
            // See if we're over a corner point.
            foreach (var contour in polygon)
            {
                // See if we're over one of the polygon's corner points.
                for (var i = 0; i < contour.Count; i++)
                {
                    // See if we're over this point.
                    if (DistanceSquared(contour[i], mousePoint) < objectRadiusSquared * 2)
                    {
                        // We're over this point.
                        return (true, contour, i);
                    }
                }
            }

            return (false, null, -1);
        }

        /// <summary>
        /// See if the mouse is over a corner point.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="polygonContour">The polygon contour.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint) MouseIsOverCornerPoint(PointF mousePoint, PolygonContour polygonContour)
        {
            // See if we're over one of the polygon's corner points.
            for (var i = 0; i < polygonContour.Count; i++)
            {
                // See if we're over this point.
                if (DistanceSquared(polygonContour[i], mousePoint) < objectRadiusSquared * 2)
                {
                    // We're over this point.
                    return (true, polygonContour, i);
                }
            }

            return (false, null, -1);
        }

        /// <summary>
        /// See if the mouse is over a polygon's edge.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="group">The group.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) MouseIsOverEdge(PointF mousePoint, Group group)
        {
            if (group is Group shapes)
            {
                // Examine each polygon in reverse order to check the ones on top first.
                for (var polygonIndex = shapes.Count - 1; polygonIndex >= 0; polygonIndex--)
                {
                    var polygon = shapes[polygonIndex];
                    switch (polygon)
                    {
                        case Group g:
                            {
                                var result = MouseIsOverEdge(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        case Polygon g:
                            {
                                var result = MouseIsOverEdge(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        case PolygonContour g:
                            {
                                var result = MouseIsOverEdge(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return (false, null, -1, -1, new PointF(0, 0));
        }

        /// <summary>
        /// See if the mouse is over a polygon's edge.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="polygon">The polygon.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) MouseIsOverEdge(PointF mousePoint, Polygon polygon)
        {
            // Examine each polygon in reverse order to check the ones on top first.
            for (var polygonIndex = polygon.Count - 1; polygonIndex >= 0; polygonIndex--)
            {
                var contour = polygon[polygonIndex];

                // See if we're over one of the polygon's segments.
                var cursorIndex = contour.Count - 1;
                for (var pointIndex = 0; pointIndex < contour.Count; pointIndex++)
                {
                    // See if we're over the segment between these points.
                    var query = DistanceToLineSegmentSquared(mousePoint, contour[cursorIndex], contour[pointIndex]);
                    if (query.Distnce < objectRadiusSquared)
                    {
                        // We are over this segment.
                        return (true, contour, cursorIndex, pointIndex, query.Point);
                    }

                    // Get the index of the polygon's next point.
                    cursorIndex = pointIndex;
                }
            }

            return (false, null, -1, -1, new PointF(0, 0));
        }

        /// <summary>
        /// See if the mouse is over a polygon's edge.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="contour">The polygon contour.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon, int HitPoint1, int HitPoint2, PointF NearestPoint) MouseIsOverEdge(PointF mousePoint, PolygonContour contour)
        {
            // See if we're over one of the polygon's segments.
            var cursorIndex = contour.Count - 1;
            for (var pointIndex = 0; pointIndex < contour.Count; pointIndex++)
            {
                // See if we're over the segment between these points.
                var query = DistanceToLineSegmentSquared(mousePoint, contour[cursorIndex], contour[pointIndex]);
                if (query.Distnce < objectRadiusSquared)
                {
                    // We are over this segment.
                    return (true, contour, cursorIndex, pointIndex, query.Point);
                }

                // Get the index of the polygon's next point.
                cursorIndex = pointIndex;
            }

            return (false, null, -1, -1, new PointF(0, 0));
        }

        /// <summary>
        /// See if the mouse is over a polygon's body.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="group">The polygons.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon) MouseIsOverPolygon(PointF mousePoint, Group group)
        {
            if (group is Group shapes)
            {
                // Examine each polygon in reverse order to check the ones on top first.
                for (var i = shapes.Count - 1; i >= 0; i--)
                {
                    var polygon = shapes[i];
                    switch (polygon)
                    {
                        case Group g:
                            {
                                var result = MouseIsOverPolygon(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        case Polygon g:
                            {
                                var result = MouseIsOverPolygon(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        case PolygonContour g:
                            {
                                var result = MouseIsOverPolygon(mousePoint, g);
                                if (result.Success) return result;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return (false, null);
        }

        /// <summary>
        /// See if the mouse is over a polygon's body.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="polygons">The polygons.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon) MouseIsOverPolygon(PointF mousePoint, Polygon polygons)
        {
            // Examine each polygon in reverse order to check the ones on top first.
            for (var i = polygons.Count - 1; i >= 0; i--)
            {
                var inclusions = PolygonContourContainsPoint(polygons[i], mousePoint);
                if (inclusions == Inclusions.Inside || inclusions == Inclusions.Boundary)
                {
                    return (true, polygons[i]);
                }
            }

            return (false, null);
        }

        /// <summary>
        /// See if the mouse is over a polygon's body.
        /// </summary>
        /// <param name="mousePoint">The mouse point.</param>
        /// <param name="contour">The polygons.</param>
        /// <returns>
        /// The <see cref="bool" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (bool Success, IGeometry<PointF> HitPolygon) MouseIsOverPolygon(PointF mousePoint, PolygonContour contour)
        {
            // Examine each polygon in reverse order to check the ones on top first.
            var inclusions = PolygonContourContainsPoint(contour, mousePoint);
            if (inclusions == Inclusions.Inside || inclusions == Inclusions.Boundary)
            {
                return (true, contour);
            }

            return (false, null);
        }
        #endregion
    }
}
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
using System.Drawing;
using System.Runtime.CompilerServices;

namespace PolygonLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public static class Extentions
    {
        #region Subtract Point
        /// <summary>
        /// Subtracts the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Subtract(this PointF minuend, PointF subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);

        /// <summary>
        /// Subtracts the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Subtract(this PointF minuend, Point subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);

        /// <summary>
        /// Subtracts the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Subtract(this Point minuend, PointF subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);

        /// <summary>
        /// Subtracts the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Subtract(this Point minuend, Point subtrahend) => new(minuend.X - subtrahend.X, minuend.Y - subtrahend.Y);
        #endregion

        #region Add Point
        /// <summary>
        /// Adds the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Add(this PointF minuend, PointF subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);

        /// <summary>
        /// Adds the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Add(this PointF minuend, Point subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);

        /// <summary>
        /// Adds the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Add(this Point minuend, PointF subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);

        /// <summary>
        /// Adds the specified subtrahend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Add(this Point minuend, Point subtrahend) => new(minuend.X + subtrahend.X, minuend.Y + subtrahend.Y);
        #endregion

        #region Scale Point
        /// <summary>
        /// Scales the specified multiplier.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Scale(this PointF multiplicand, float scaler) => new(multiplicand.X * scaler, multiplicand.Y * scaler);

        /// <summary>
        /// Scales the specified multiplier.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Scale(this PointF multiplicand, double scaler) => new((float)(multiplicand.X * scaler), (float)(multiplicand.Y * scaler));

        /// <summary>
        /// Scales the specified multiplier.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Scale(this PointF multiplicand, SizeF scaler) => new(multiplicand.X * scaler.Width, multiplicand.Y * scaler.Height);

        /// <summary>
        /// Scales the specified scaler.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Scale(this Point multiplicand, float scaler) => new(multiplicand.X * scaler, multiplicand.Y * scaler);

        /// <summary>
        /// Scales the specified scaler.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Scale(this Point multiplicand, double scaler) => new((float)(multiplicand.X * scaler), (float)(multiplicand.Y * scaler));

        /// <summary>
        /// Scales the specified scaler.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PointF Scale(this Point multiplicand, SizeF scaler) => new(multiplicand.X * scaler.Width, multiplicand.Y * scaler.Height);

        /// <summary>
        /// Scales the specified scaler.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Scale(this Point multiplicand, int scaler) => new(multiplicand.X * scaler, multiplicand.Y * scaler);

        /// <summary>
        /// Scales the specified scaler.
        /// </summary>
        /// <param name="multiplicand">The multiplicand.</param>
        /// <param name="scaler">The scaler.</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point Scale(this Point multiplicand, Size scaler) => new(multiplicand.X * scaler.Width, multiplicand.Y * scaler.Height);
        #endregion Scale Point

        #region Point To String
        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="format">The format.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this PointF point, string format, IFormatProvider provider) => $"{{X={point.X.ToString(format, provider)}, Y={point.Y.ToString(format, provider)}}}";

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="format">The format.</param>
        /// <param name="provider">The provider.</param>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this Point point, string format, IFormatProvider provider) => $"{{X={point.X.ToString(format, provider)}, Y={point.Y.ToString(format, provider)}}}";
        #endregion Point To String
    }
}

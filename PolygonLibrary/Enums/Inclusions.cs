﻿// <copyright file="Inclusions.cs">
//     Copyright © 2019 - 2020 Shkyrockett. All rights reserved.
// </copyright>
// <author id="shkyrockett">Shkyrockett</author>
// <license>
//     Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </license>
// <summary></summary>
// <remarks></remarks>

namespace PolygonLibrary;

/// <summary>
/// Enumeration of the inclusion of a point within a shape.
/// </summary>
public enum Inclusions
    : sbyte
{
    /// <summary>
    /// Touches the boundary of the shape.
    /// </summary>
    Boundary = -1,

    /// <summary>
    /// Point lies outside the shape.
    /// </summary>
    Outside = 0,

    /// <summary>
    /// Point is contained inside the shape.
    /// </summary>
    Inside = 1,
}

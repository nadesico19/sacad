/* Copyright (c) 2022 Chin Ako <nadesico19@gmail.com>
 * sacad is licensed under Mulan PubL v2.
 * You can use this software according to the terms and conditions of the Mulan PubL v2.
 * You may obtain a copy of Mulan PubL v2 at:
 *          http://license.coscl.org.cn/MulanPubL-2.0
 * THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND,
 * EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,
 * MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
 * See the Mulan PubL v2 for more details.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType("sacad.acdb.Curve")]
    public class Curve : Entity
    {
    }

    [ArxEntity(typeof(AcDb.Arc))]
    [PyType("sacad.acdb.Arc")]
    public sealed class Arc : Curve
    {
        public Vector3d center;
        public double? end_angle;
        public Vector3d normal;
        public double? radius;
        public double? start_angle;
        public double? thickness;
        public double? total_angle;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.Arc>(db);
            var arc = (AcDb.Arc)obj;

            if (center != null) arc.Center = center.ToPoint3d();
            if (end_angle.HasValue) arc.EndAngle = end_angle.Value;
            if (normal != null) arc.Normal = normal.ToVector3d();
            if (radius.HasValue) arc.Radius = radius.Value;
            if (start_angle.HasValue) arc.StartAngle = start_angle.Value;
            if (thickness.HasValue) arc.Thickness = thickness.Value;

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var arc = (AcDb.Arc)obj;

            center = arc.Center;
            end_angle = arc.EndAngle;
            normal = arc.Normal;
            radius = arc.Radius;
            start_angle = arc.StartAngle;
            thickness = arc.Thickness;
            total_angle = arc.TotalAngle;

            return base.FromArx(obj, db);
        }
    }

    [ArxEntity(typeof(AcDb.Line))]
    [PyType("sacad.acdb.Line")]
    public sealed class Line : Curve
    {
        public Vector3d end_point;
        public Vector3d normal;
        public Vector3d start_point;
        public double? thickness;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.Line>(db);
            var line = (AcDb.Line)obj;

            if (end_point != null) line.EndPoint = end_point.ToPoint3d();
            if (normal != null) line.Normal = normal.ToVector3d();
            if (start_point != null) line.StartPoint = start_point.ToPoint3d();
            if (thickness.HasValue) line.Thickness = thickness.Value;

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var line = (AcDb.Line)obj;

            end_point = line.EndPoint;
            normal = line.Normal;
            start_point = line.StartPoint;
            thickness = line.Thickness;

            return base.FromArx(obj, db);
        }
    }

    [PyType("sacad.acdb.Vertex")]
    public sealed class Vertex
    {
        public Vector2d point;
        public double? bulge;
        public double? start_width;
        public double? end_width;
    }

    [ArxEntity(typeof(AcDb.Polyline))]
    [PyType("sacad.acdb.Polyline")]
    public sealed class Polyline : Curve
    {
        public bool? closed;
        public double? constant_width;
        public double? elevation;
        public Vector3d normal;
        public double? thickness;
        public PyWrapper<Vertex>[] vertices;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.Polyline>(db);
            var polyline = (AcDb.Polyline)obj;

            if (closed.HasValue) polyline.Closed = closed.Value;
            if (elevation.HasValue) polyline.Elevation = elevation.Value;
            if (normal != null) polyline.Normal = normal.ToVector3d();
            if (thickness.HasValue) polyline.Thickness = thickness.Value;

            foreach (var v in vertices)
            {
                polyline.AddVertexAt(polyline.NumberOfVertices,
                    v.__mbr__.point.ToPoint2d(),
                    v.__mbr__.bulge ?? 0,
                    v.__mbr__.start_width ?? constant_width ?? 0,
                    v.__mbr__.end_width ?? constant_width ?? 0);
            }

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var polyline = (AcDb.Polyline)obj;

            closed = Util.ToOptional(polyline.Closed);
            elevation = Util.ToOptional(polyline.Elevation);
            normal = polyline.Normal;
            thickness = Util.ToOptional(polyline.Thickness);

            vertices = Enumerable.Range(0, polyline.NumberOfVertices)
                .Select(i => PyWrapper<Vertex>.Create(new Vertex
                {
                    point = polyline.GetPoint2dAt(i),
                    bulge = Util.ToOptional(polyline.GetBulgeAt(i)),
                    start_width = Util.ToOptional(polyline.GetStartWidthAt(i)),
                    end_width = Util.ToOptional(polyline.GetEndWidthAt(i)),
                }))
                .ToArray();

            var minWidth = vertices.Min(v =>
                Math.Min(v.__mbr__.start_width ?? 0, v.__mbr__.end_width ?? 0));
            var maxWidth = vertices.Max(v =>
                Math.Max(v.__mbr__.start_width ?? 0, v.__mbr__.end_width ?? 0));

            if (polyline.HasWidth &&
                EqualityComparer<double>.Default.Equals(minWidth, maxWidth))
                constant_width = minWidth;

            return base.FromArx(obj, db);
        }
    }
}
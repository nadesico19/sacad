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
    [PyType(Name = "sacad.acdb.Curve")]
    public class Curve : Entity
    {
        protected static T New<T>(AcDb.Database db) where T : AcDb.Entity, new()
        {
            var entity = new T();
            entity.SetDatabaseDefaults(db);
            return entity;
        }
    }

    [PyType(Name = "sacad.acdb.Vertex")]
    public class Vertex : PyObject
    {
        public Vector2d point;
        public double? bulge;
        public double? start_width;
        public double? end_width;
    }

    [PyType(Name = "sacad.acdb.Polyline")]
    public class Polyline : Curve
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
                polyline.AddVertexAt(polyline.NumberOfVertices, v.__mbr__.point.ToPoint2d(),
                    v.__mbr__.bulge ?? 0, v.__mbr__.start_width ?? constant_width ?? 0,
                    v.__mbr__.end_width ?? constant_width ?? 0);
            }

            return base.ToArx(obj, db);
        }

        public override DbObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var polyline = (AcDb.Polyline)obj;

            closed = polyline.Closed;
            elevation = polyline.Elevation;
            normal = polyline.Normal;
            thickness = polyline.Thickness;

            vertices = Enumerable.Range(0, polyline.NumberOfVertices)
                .Select(i => PyWrapper<Vertex>.Create(new Vertex
                {
                    point = polyline.GetPoint2dAt(i),
                    bulge = polyline.GetBulgeAt(i),
                    start_width = polyline.GetStartWidthAt(i),
                    end_width = polyline.GetEndWidthAt(i),
                }))
                .ToArray();

            if (polyline.HasWidth && EqualityComparer<double>.Default.Equals(
                    vertices.Min(
                        v => Math.Min(v.__mbr__.start_width ?? 0, v.__mbr__.end_width ?? 0)),
                    vertices.Max(
                        v => Math.Max(v.__mbr__.start_width ?? 0, v.__mbr__.end_width ?? 0))))
            {
                constant_width = polyline.ConstantWidth;
            }

            return base.FromArx(obj, db);
        }
    }
}
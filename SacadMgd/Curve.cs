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

using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType(Name = "sacad.acdb.Curve")]
    public class Curve : Entity
    {
    }

    public class VertexWrapper : PyWrapper<Vertex>
    {
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
        public VertexWrapper[] vertices;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? new AcDb.Polyline();
            var ent = (AcDb.Polyline)obj;

            foreach (var v in vertices)
            {
                ent.AddVertexAt(ent.NumberOfVertices, (AcGe.Point2d)v.__mbr__.point,
                    v.__mbr__.bulge ?? 0, v.__mbr__.start_width ?? 0, v.__mbr__.end_width ?? 0);
            }

            if (normal != null) ent.Normal = (AcGe.Vector3d)normal;
            if (constant_width.HasValue) ent.ConstantWidth = constant_width.Value;
            if (elevation.HasValue) ent.Elevation = elevation.Value;
            if (closed.HasValue) ent.Closed = closed.Value;

            return base.ToArx(obj, db);
        }
    }
}
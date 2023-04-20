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

using Newtonsoft.Json;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    public sealed class Vector2d
    {
        public string __cls__ = "sacad.acge.Vector2d";
        public double[] __mbr__;

        [JsonIgnore] public double X => __mbr__[0];

        [JsonIgnore] public double Y => __mbr__[1];

        public AcGe.Vector2d ToVector2d() => (AcGe.Vector2d)this;

        public AcGe.Point2d ToPoint2d() => (AcGe.Point2d)this;

        public static explicit operator AcGe.Vector2d(Vector2d v) =>
            new AcGe.Vector2d(v.X, v.Y);

        public static explicit operator AcGe.Point2d(Vector2d v) =>
            new AcGe.Point2d(v.X, v.Y);

        public static implicit operator Vector2d(AcGe.Vector2d v) =>
            new Vector2d { __mbr__ = v.ToArray() };

        public static implicit operator Vector2d(AcGe.Point2d p) =>
            new Vector2d { __mbr__ = p.ToArray() };
    }

    public sealed class Vector3d
    {
        public string __cls__ = "sacad.acge.Vector3d";
        public double[] __mbr__;

        [JsonIgnore] public double X => __mbr__[0];

        [JsonIgnore] public double Y => __mbr__[1];

        [JsonIgnore] public double Z => __mbr__[2];

        public AcGe.Vector3d ToVector3d() => (AcGe.Vector3d)this;

        public AcGe.Point3d ToPoint3d() => (AcGe.Point3d)this;

        public static explicit operator AcGe.Vector3d(Vector3d v) =>
            new AcGe.Vector3d(v.X, v.Y, v.Z);

        public static explicit operator AcGe.Point3d(Vector3d v) =>
            new AcGe.Point3d(v.X, v.Y, v.Z);

        public static implicit operator Vector3d(AcGe.Vector3d v) =>
            new Vector3d { __mbr__ = v.ToArray() };

        public static implicit operator Vector3d(AcGe.Point3d p) =>
            new Vector3d { __mbr__ = p.ToArray() };
    }
}
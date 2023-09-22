﻿/* Copyright (c) 2022 Chin Ako <nadesico19@gmail.com>
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
using System.Reflection;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    using BlockTable = Dictionary<string, PyWrapper<BlockTableRecord>>;
    using DimStyleTable = Dictionary<string, PyWrapper<DimStyleTableRecord>>;
    using LayerTable = Dictionary<string, PyWrapper<LayerTableRecord>>;
    using LinetypeTable = Dictionary<string, PyWrapper<LinetypeTableRecord>>;
    using TextStyleTable = Dictionary<string, PyWrapper<TextStyleTableRecord>>;

    [PyType("sacad.acdb.Database")]
    public sealed class Database
    {
        public BlockTable block_table;
        public DimStyleTable dim_style_table;
        public LayerTable layer_table;
        public LinetypeTable linetype_table;
        public TextStyleTable text_style_table;

        public BlockTable GetBlockTable() => Ensure(ref block_table);
        public DimStyleTable GetDimStyleTable() => Ensure(ref dim_style_table);
        public LayerTable GetLayerTable() => Ensure(ref layer_table);
        public LinetypeTable GetLinetypeTable() => Ensure(ref linetype_table);

        public TextStyleTable GetTextStyleTable() =>
            Ensure(ref text_style_table);

        public BlockTableRecord GetBlockTableRecord(string name)
        {
            if (!GetBlockTable().ContainsKey(name))
            {
                block_table[name] = PyWrapper<BlockTableRecord>
                    .Create(new BlockTableRecord
                        { entities = new List<PyWrapper<Entity>>() });
            }

            return block_table[name].__mbr__;
        }

        public BlockTableRecord GetModelSpace() =>
            GetBlockTableRecord("*MODEL_SPACE");

        private static T Ensure<T>(ref T table) where T : class, new() =>
            table ?? (table = new T());
    }

    [PyType("sacad.acdb.DBObject")]
    public class DBObject
    {
        public long? id;

        public virtual AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
            => obj;

        public virtual DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            id = obj.Id.OldIdPtr.ToInt64();
            return this;
        }
    }

    [PyType("sacad.acdb.Entity")]
    public class Entity : DBObject
    {
        public PyWrapper<Color> color;
        public int? color_index;
        public PyWrapper<Extents3d> geometric_extents;
        public string layer;
        public string linetype;
        public double? linetype_scale;
        public AcDb.LineWeight? line_weight;
        public bool? visible;
        public Matrix3d matrix;

        protected static T New<T>(AcDb.Database db) where T : AcDb.Entity, new()
        {
            var entity = new T();
            entity.SetDatabaseDefaults(db);
            return entity;
        }

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var entity = (AcDb.Entity)obj;

            if (color != null)
                entity.Color = color.__mbr__.ToArx();
            else if (color_index.HasValue)
                entity.ColorIndex = color_index.Value;

            if (layer != null && db.GetLayer(layer) != null)
                entity.Layer = layer;
            if (linetype != null && db.GetLinetype(linetype) != null)
                entity.Linetype = linetype;
            if (linetype_scale.HasValue)
                entity.LinetypeScale = linetype_scale.Value;
            if (line_weight.HasValue) entity.LineWeight = line_weight.Value;
            if (visible.HasValue) entity.Visible = visible.Value;

            if (matrix != null) entity.TransformBy(matrix.ToMatrix3d());

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var entity = (AcDb.Entity)obj;

            if (entity.Color != null)
                color = PyWrapper<Color>.Create(Color.FromArx(entity.Color));
            color_index = entity.ColorIndex;

            if (entity.Bounds.HasValue)
            {
                geometric_extents =
                    PyWrapper<Extents3d>.Create(
                        (Extents3d)entity.GeometricExtents);
            }

            layer = entity.Layer;
            linetype = entity.Linetype;
            linetype_scale = entity.LinetypeScale;
            line_weight = entity.LineWeight;
            visible = entity.Visible;

            return base.FromArx(obj, db);
        }

        public static Entity Convert(AcDb.Entity arxEntity, AcDb.Database db)
        {
            Type type;
            return ArxTypes.TryGetValue(arxEntity.GetType(), out type)
                ? (Activator.CreateInstance(type) as Entity)?.FromArx(
                    arxEntity, db) as Entity
                : null;
        }

        internal static void RegisterALl()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var entType in asm.GetTypes().Where(t =>
                         t.GetCustomAttribute<ArxEntityAttribute>() != null))
            {
                ArxTypes[entType.GetCustomAttribute<ArxEntityAttribute>()
                    .ArxType] = entType;
            }
        }

        private static readonly Dictionary<Type, Type> ArxTypes =
            new Dictionary<Type, Type>();
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ArxEntityAttribute : Attribute
    {
        public readonly Type ArxType;

        public ArxEntityAttribute(Type arxType)
        {
            ArxType = arxType;
        }
    }

    [ArxEntity(typeof(AcDb.DBText))]
    [PyType("sacad.acdb.DBText")]
    public sealed class DBText : Entity
    {
        public Vector3d alignment_point;
        public double? height;
        public AcDb.TextHorizontalMode? horizontal_mode;
        public bool? is_mirrored_in_x;
        public bool? is_mirrored_in_y;
        public AcDb.AttachmentPoint? justify;
        public Vector3d normal;
        public double? oblique;
        public Vector3d position;
        public double? rotation;
        public string text_string;
        public string text_style_name;
        public double? thickness;
        public AcDb.TextVerticalMode? vertical_mode;
        public double? width_factor;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.DBText>(db);
            var dbText = (AcDb.DBText)obj;

            if (height.HasValue) dbText.Height = height.Value;
            if (horizontal_mode.HasValue)
                dbText.HorizontalMode = horizontal_mode.Value;
            if (is_mirrored_in_x.HasValue)
                dbText.IsMirroredInX = is_mirrored_in_x.Value;
            if (is_mirrored_in_y.HasValue)
                dbText.IsMirroredInY = is_mirrored_in_y.Value;
            if (justify.HasValue) dbText.Justify = justify.Value;
            if (normal != null) dbText.Normal = normal.ToVector3d();
            if (oblique.HasValue) dbText.Oblique = oblique.Value;
            if (rotation.HasValue) dbText.Rotation = rotation.Value;
            dbText.TextString = text_string ?? string.Empty;
            if (thickness.HasValue) dbText.Thickness = thickness.Value;
            if (vertical_mode.HasValue)
                dbText.VerticalMode = vertical_mode.Value;
            if (width_factor.HasValue) dbText.WidthFactor = width_factor.Value;

            if (text_style_name != null)
            {
                var style = db.GetTextStyle(text_style_name);
                if (style != null)
                {
                    if (!oblique.HasValue)
                        dbText.Oblique = style.ObliquingAngle;
                    if (!width_factor.HasValue)
                        dbText.WidthFactor = style.XScale;
                    if (!height.HasValue && style.TextSize > 0)
                        dbText.Height = style.TextSize;
                    dbText.Annotative = style.Annotative;

                    dbText.TextStyleId = style.ObjectId;
                }
            }

            if (dbText.HorizontalMode == AcDb.TextHorizontalMode.TextFit)
            {
                dbText.AlignmentPoint =
                    (alignment_point ?? Vector3d.Origin).ToPoint3d();
                dbText.Position = (position ?? Vector3d.Origin).ToPoint3d();
            }
            else if (dbText.Justify != AcDb.AttachmentPoint.BaseLeft)
            {
                dbText.AlignmentPoint =
                    (alignment_point ?? position ?? Vector3d.Origin)
                    .ToPoint3d();
            }
            else
            {
                dbText.Position =
                    (position ?? alignment_point ?? Vector3d.Origin)
                    .ToPoint3d();
            }

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dbText = (AcDb.DBText)obj;

            alignment_point = dbText.AlignmentPoint;
            height = dbText.Height;
            horizontal_mode = Util.ToOptional(dbText.HorizontalMode);
            is_mirrored_in_x = Util.ToOptional(dbText.IsMirroredInX);
            is_mirrored_in_y = Util.ToOptional(dbText.IsMirroredInY);
            justify = dbText.Justify;
            normal = dbText.Normal;
            oblique = Util.ToOptional(dbText.Oblique);
            position = dbText.Position;
            rotation = Util.ToOptional(dbText.Rotation);
            text_string = dbText.TextString;
            text_style_name = dbText.TextStyleName;
            thickness = Util.ToOptional(dbText.Thickness);
            vertical_mode = Util.ToOptional(dbText.VerticalMode);
            width_factor = dbText.WidthFactor;

            return base.FromArx(obj, db);
        }
    }

    [PyType("sacad.acdb.HatchLoop")]
    public class HatchLoop
    {
        public AcDb.HatchLoopTypes? loop_type;
        public PyWrapper<Polyline> polyline;
        public PyWrapper<Curve>[] curves;
    }

    [ArxEntity(typeof(AcDb.Hatch))]
    [PyType("sacad.acdb.Hatch")]
    public class Hatch : Entity
    {
        public bool? associative;
        public PyWrapper<Color> background_color;
        public double? elevation;
        public double? gradient_angle;
        public string gradient_name;
        public bool? gradient_one_color_mode;
        public float? gradient_shift;
        public AcDb.GradientPatternType? gradient_type;
        public AcDb.HatchObjectType? hatch_object_type;
        public AcDb.HatchStyle? hatch_style;
        public Vector3d normal;
        public Vector2d origin;
        public double? pattern_angle;
        public bool? pattern_double;
        public string pattern_name;
        public double? pattern_scale;
        public double? pattern_space;
        public AcDb.HatchPatternType? pattern_type;
        public float? shade_tint_value;

        public PyWrapper<HatchLoop>[] hatch_loops;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.Hatch>(db);
            var hatch = obj as AcDb.Hatch;

            if (hatch_object_type == AcDb.HatchObjectType.HatchObject)
            {
                GenHatch(hatch, db);
            }

            // TODO gradient type

            return base.ToArx(obj, db);
        }

        private void GenHatch(AcDb.Hatch hatch, AcDb.Database db)
        {
            if (!(hatch_loops?.Length > 0)) return;

            if (pattern_scale.HasValue)
                hatch.PatternScale = pattern_scale.Value;
            if (pattern_double.HasValue)
                hatch.PatternDouble = pattern_double.Value;
            if (hatch_style.HasValue)
                hatch.HatchStyle = hatch_style.Value;

            hatch.SetHatchPattern(
                pattern_type ?? AcDb.HatchPatternType.PreDefined, pattern_name);
            db.AddToModelSpace(hatch);

            if (pattern_angle.HasValue && pattern_angle.Value != 0)
                hatch.PatternAngle = pattern_angle.Value;
            if (associative.HasValue)
                hatch.Associative = associative.Value;

            foreach (var loop in hatch_loops.Select(e => e.__mbr__))
            {
                var hatchLoop = new AcDb.HatchLoop(
                    loop.loop_type ?? AcDb.HatchLoopTypes.Default);

                var polyline = loop.polyline?.__mbr__;
                if (polyline?.vertices?.Length > 1)
                {
                    foreach (var vertex in polyline.vertices
                                 .Select(e => e.__mbr__))
                    {
                        hatchLoop.Polyline.Add(
                            new AcDb.BulgeVertex(vertex.point.ToPoint2d(),
                                vertex.bulge ?? 0));
                    }

                    var firstv = polyline.vertices.First().__mbr__;
                    var lastv = polyline.vertices.Last().__mbr__;

                    if (firstv.point.ToPoint2d().GetDistanceTo(
                            lastv.point.ToPoint2d()) > 1e-6)
                    {
                        hatchLoop.Polyline.Add(
                            new AcDb.BulgeVertex(firstv.point.ToPoint2d(),
                                firstv.bulge ?? 0));
                    }
                }
                else if (loop.curves?.Length > 0)
                {
                    foreach (var curve in loop.curves.Select(e => e.__mbr__))
                    {
                        AcGe.Curve2d curve2d = null;

                        if (curve is Line)
                        {
                            var line = (Line)curve;
                            curve2d = new AcGe.LineSegment2d(
                                new AcGe.Point2d(line.start_point.X,
                                    line.start_point.Y),
                                new AcGe.Point2d(line.end_point.X,
                                    line.end_point.Y));
                        }
                        else if (curve is Arc)
                        {
                            var arc = (Arc)curve;

                            var center = new AcGe.Point2d(
                                arc.center.X, arc.center.Y);
                            var radius = arc.radius ?? 0;
                            var start_angle = arc.start_angle ?? 0;
                            var end_angle = arc.end_angle ?? 0;

                            var start_ponit = center + radius
                                * AcGe.Vector2d.XAxis.RotateBy(start_angle);
                            var end_ponit = center + radius
                                * AcGe.Vector2d.XAxis.RotateBy(end_angle);
                            var point_on_arc = center + radius
                                * AcGe.Vector2d.XAxis.RotateBy(
                                    (start_angle + end_angle) / 2);

                            curve2d = new AcGe.CircularArc2d(
                                start_ponit, point_on_arc, end_ponit);
                        }
                        // TODO

                        if (curve2d != null) hatchLoop.Curves.Add(curve2d);
                    }
                }

                hatch.AppendLoop(hatchLoop);
            }

            hatch.EvaluateHatch(true);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var hatch = (AcDb.Hatch)obj;

            hatch_object_type = hatch.HatchObjectType;
            pattern_type = hatch.PatternType;
            pattern_name = hatch.PatternName;
            pattern_angle = Util.ToOptional(hatch.PatternAngle);
            pattern_double = Util.ToOptional(hatch.PatternDouble);
            pattern_scale = hatch.PatternScale;
            hatch_style = hatch.HatchStyle;
            associative = Util.ToOptional(hatch.Associative);

            var hatchLoops = new List<PyWrapper<HatchLoop>>();

            for (var i = 0; i < hatch.NumberOfLoops; i++)
            {
                var loop = hatch.GetLoopAt(i);
                if (loop.IsPolyline)
                {
                    var polyline = new Polyline
                    {
                        vertices = loop.Polyline.OfType<AcDb.BulgeVertex>()
                            .Select(v => PyWrapper<Vertex>
                                .Create(new Vertex
                                {
                                    point = v.Vertex, bulge = v.Bulge
                                }))
                            .ToArray(),
                    };

                    hatchLoops.Add(PyWrapper<HatchLoop>.Create(
                        new HatchLoop
                        {
                            loop_type = loop.LoopType,
                            polyline = PyWrapper<Polyline>.Create(polyline),
                        }));

                    continue;
                }

                var curves = new List<PyWrapper<Curve>>();
                foreach (var curve2d in loop.Curves.OfType<AcGe.Curve2d>())
                {
                    if (curve2d is AcGe.LineSegment2d)
                    {
                        var line2d = (AcGe.LineSegment2d)curve2d;
                        curves.Add(PyWrapper<Curve>.Create(
                            new Line
                            {
                                start_point = line2d.StartPoint,
                                end_point = line2d.EndPoint,
                            }));
                    }
                    else if (curve2d is AcGe.CircularArc2d)
                    {
                        var arc2d = (AcGe.CircularArc2d)curve2d;
                        curves.Add(PyWrapper<Curve>.Create(
                            new Arc
                            {
                                center = arc2d.Center,
                                radius = arc2d.Radius,
                                start_angle = arc2d.StartAngle,
                                end_angle = arc2d.EndAngle,
                            }));
                    }
                    // TODO
                }

                hatchLoops.Add(PyWrapper<HatchLoop>.Create(
                    new HatchLoop
                    {
                        loop_type = loop.LoopType,
                        curves = curves.ToArray(),
                    }));
            }

            hatch_loops = hatchLoops.ToArray();

            return base.FromArx(obj, db);
        }
    }

    [PyType("sacad.acdb.Extents3d")]
    public sealed class Extents3d
    {
        public Vector3d min_point;
        public Vector3d max_point;

        public static implicit operator Extents3d(AcDb.Extents3d ext) =>
            new Extents3d
                { min_point = ext.MinPoint, max_point = ext.MaxPoint };
    }
}
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
using AcDb = Autodesk.AutoCAD.DatabaseServices;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType(Name = "sacad.acdb.SymbolTableRecord")]
    public class SymbolTableRecord : DbObject
    {
        public string name;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var symbol = obj as AcDb.SymbolTableRecord;
            if (symbol != null) symbol.Name = name;

            return base.ToArx(obj, db);
        }

        public virtual AcDb.SymbolTableRecord GetFromSymbolTable(
            AcDb.Database db)
        {
            throw new NotImplementedException();
        }

        public virtual AcDb.ObjectId AddToSymbolTable(
            AcDb.SymbolTableRecord symbol,
            AcDb.Database db)
        {
            throw new NotImplementedException();
        }
    }

    [PyType(Name = "sacad.acdb.BlockTableRecord")]
    public sealed class BlockTableRecord : SymbolTableRecord
    {
        public List<PyWrapper<Entity>> entities;
    }

    [PyType(Name = "sacad.acdb.LayerTableRecord")]
    public sealed class LayerTableRecord : SymbolTableRecord
    {
        public PyWrapper<Color> color;
        public bool? is_frozen;
        public bool? is_locked;
        public bool? is_off;
        public bool? is_plottable;
        public LineWeight? line_weight;
        public string linetype;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? new AcDb.LayerTableRecord();
            var layer = (AcDb.LayerTableRecord)obj;

            if (color != null) layer.Color = color.__mbr__.ToArx();
            if (is_frozen.HasValue) layer.IsFrozen = is_frozen.Value;
            if (is_locked.HasValue) layer.IsLocked = is_locked.Value;
            if (is_off.HasValue) layer.IsOff = is_off.Value;
            if (is_plottable.HasValue) layer.IsPlottable = is_plottable.Value;
            if (line_weight.HasValue)
                layer.LineWeight = (AcDb.LineWeight)line_weight.Value;
            if (!string.IsNullOrWhiteSpace(linetype))
            {
                var ltype = db.GetLinetype(linetype);
                if (ltype != null) layer.LinetypeObjectId = ltype.ObjectId;
            }

            return base.ToArx(obj, db);
        }

        public override DbObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var layer = (AcDb.LayerTableRecord)obj;
            var trans = db.TransactionManager.TopTransaction;

            color = PyWrapper<Color>.Create(Color.FromArx(layer.Color));
            line_weight = (LineWeight)layer.LineWeight;
            is_frozen = Util.ToOptional(layer.IsFrozen);
            is_locked = Util.ToOptional(layer.IsLocked);
            is_off = Util.ToOptional(layer.IsOff);
            is_plottable = Util.ToOptional(layer.IsPlottable);

            if (layer.LinetypeObjectId.IsValid)
            {
                linetype = (trans.GetObject(layer.LinetypeObjectId,
                    AcDb.OpenMode.ForRead) as AcDb.LayerTableRecord)?.Name;
            }

            return base.FromArx(obj, db);
        }

        public override AcDb.SymbolTableRecord GetFromSymbolTable(
            AcDb.Database db) => db.GetLayer(name);

        public override AcDb.ObjectId AddToSymbolTable(
            AcDb.SymbolTableRecord symbol, AcDb.Database db) =>
            db.AddLayer((AcDb.LayerTableRecord)symbol);
    }

    [PyType(Name = "sacad.acdb.LinetypeSegment")]
    public sealed class LinetypeSegment
    {
        public double? dash_length;
        public bool? shape_is_ucs_oriented;
        public int? shape_number;
        public Vector2d shape_offset;
        public double? shape_rotation;
        public double? shape_scale;
        public string shape_style;
        public string text;
    }

    [PyType(Name = "sacad.acdb.LayerTableRecord")]
    public sealed class LinetypeTableRecord : SymbolTableRecord
    {
        public string comments;
        public PyWrapper<LinetypeSegment>[] segments;
        public bool? is_scaled_to_fit;
        public double? pattern_length;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? new AcDb.LinetypeTableRecord();
            var linetype = (AcDb.LinetypeTableRecord)obj;

            if (segments?.Length > 0)
            {
                linetype.NumDashes = segments.Length;

                for (var i = 0; i < segments.Length; i++)
                {
                    var seg = segments[i].__mbr__;
                    if (seg.dash_length.HasValue)
                        linetype.SetDashLengthAt(i, seg.dash_length.Value);
                    if (seg.shape_is_ucs_oriented.HasValue)
                        linetype.SetShapeIsUcsOrientedAt(i,
                            seg.shape_is_ucs_oriented.Value);
                    if (seg.shape_number.HasValue)
                        linetype.SetShapeNumberAt(i, seg.shape_number.Value);
                    if (seg.shape_offset != null)
                    {
                        linetype.SetShapeOffsetAt(i,
                            seg.shape_offset.ToVector2d());
                    }

                    if (seg.shape_rotation.HasValue)
                        linetype.SetShapeRotationAt(i,
                            seg.shape_rotation.Value);
                    if (seg.shape_scale.HasValue)
                        linetype.SetShapeScaleAt(i, seg.shape_scale.Value);
                    if (string.IsNullOrEmpty(seg.text))
                        linetype.SetTextAt(i, seg.text);
                    if (seg.shape_style != null)
                    {
                        var shape = db.GetTextStyle(seg.shape_style) ??
                                    db.GetShapeStyle(seg.shape_style);
                        if (shape != null)
                            linetype.SetShapeStyleAt(i, shape.ObjectId);
                    }
                }
            }

            if (comments != null) linetype.Comments = comments;
            if (is_scaled_to_fit.HasValue)
                linetype.IsScaledToFit = is_scaled_to_fit.Value;
            if (pattern_length.HasValue)
                linetype.PatternLength = pattern_length.Value;

            return base.ToArx(obj, db);
        }

        public override DbObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var linetype = (AcDb.LinetypeTableRecord)obj;
            var trans = db.TransactionManager.TopTransaction;

            if (linetype.NumDashes > 0)
            {
                var allSegments = new List<PyWrapper<LinetypeSegment>>();
                for (var i = 0; i < linetype.NumDashes; i++)
                {
                    string shapeStyle = null;
                    if (linetype.ShapeStyleAt(i).IsValid)
                    {
                        var symbol = (AcDb.TextStyleTableRecord)trans.GetObject(
                            linetype.ShapeStyleAt(i), AcDb.OpenMode.ForRead);
                        shapeStyle = symbol.IsShapeFile
                            ? symbol.FileName
                            : symbol.Name;
                    }

                    string text = null;
                    try
                    {
                        text = linetype.TextAt(i);
                    }
                    catch
                    {
                        // eNotApplicable
                    }

                    allSegments.Add(PyWrapper<LinetypeSegment>.Create(
                        new LinetypeSegment
                        {
                            dash_length = linetype.DashLengthAt(i),
                            shape_is_ucs_oriented = Util.ToOptional(
                                linetype.ShapeIsUcsOrientedAt(i)),
                            shape_number = Util.ToOptional(
                                linetype.ShapeNumberAt(i)),
                            shape_offset = Util.ToOptional(
                                linetype.ShapeOffsetAt(i)),
                            shape_rotation = Util.ToOptional(
                                linetype.ShapeRotationAt(i)),
                            shape_scale = linetype.ShapeScaleAt(i),
                            shape_style = shapeStyle,
                            text = text,
                        }));
                }

                segments = allSegments.ToArray();
            }

            comments = linetype.Comments;
            is_scaled_to_fit = Util.ToOptional(linetype.IsScaledToFit);
            pattern_length = Util.ToOptional(linetype.PatternLength);

            return base.FromArx(obj, db);
        }

        public override AcDb.SymbolTableRecord GetFromSymbolTable(
            AcDb.Database db) => db.GetLinetype(name);

        public override AcDb.ObjectId AddToSymbolTable(
            AcDb.SymbolTableRecord symbol, AcDb.Database db) =>
            db.AddLineype((AcDb.LinetypeTableRecord)symbol);
    }
}
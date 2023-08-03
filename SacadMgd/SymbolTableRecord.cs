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
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcGi = Autodesk.AutoCAD.GraphicsInterface;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType("sacad.acdb.SymbolTableRecord")]
    public class SymbolTableRecord : DBObject
    {
        public string name;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var symbol = obj as AcDb.SymbolTableRecord;
            if (symbol != null) symbol.Name = name;

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var symbol = obj as AcDb.SymbolTableRecord;
            if (symbol != null) name = symbol.Name;

            return base.FromArx(obj, db);
        }

        public virtual AcDb.SymbolTableRecord GetFromSymbolTable(
            AcDb.Database db)
        {
            throw new NotImplementedException();
        }

        public virtual AcDb.ObjectId AddToSymbolTable(
            AcDb.SymbolTableRecord symbol, AcDb.Database db)
        {
            throw new NotImplementedException();
        }
    }

    [PyType("sacad.acdb.BlockTableRecord")]
    public sealed class BlockTableRecord : SymbolTableRecord
    {
        public List<PyWrapper<Entity>> entities;

        // TODO override
    }

    [PyType("sacad.acdb.DimStyleTableRecord")]
    public sealed class DimStyleTableRecord : SymbolTableRecord
    {
        public int? dimadec;
        public bool? dimalt;
        public int? dimaltd;
        public double? dimaltf;
        public double? dimaltrnd;
        public int? dimalttd;
        public int? dimalttz;
        public int? dimaltu;
        public int? dimaltz;
        public string dimapost;
        public int? dimarcsym;
        public double? dimasz;
        public int? dimatfit;
        public int? dimaunit;
        public int? dimazin;
        public string dimblk;
        public string dimblk1;
        public string dimblk2;
        public double? dimcen;
        public PyWrapper<Color> dimclrd;
        public PyWrapper<Color> dimclre;
        public PyWrapper<Color> dimclrt;
        public int? dimdec;
        public double? dimdle;
        public double? dimdli;
        public string dimdsep;
        public double? dimexe;
        public double? dimexo;
        public int? dimfrac;
        public double? dimfxlen;
        public bool? dimfxlenOn;
        public double? dimgap;
        public double? dimjogang;
        public int? dimjust;
        public string dimldrblk;
        public double? dimlfac;
        public bool? dimlim;
        public string dimltex1;
        public string dimltex2;
        public string dimltype;
        public int? dimlunit;
        public AcDb.LineWeight? dimlwd;
        public AcDb.LineWeight? dimlwe;
        public string dimpost;
        public double? dimrnd;
        public bool? dimsah;
        public double? dimscale;
        public bool? dimsd1;
        public bool? dimsd2;
        public bool? dimse1;
        public bool? dimse2;
        public bool? dimsoxd;
        public int? dimtad;
        public int? dimtdec;
        public double? dimtfac;
        public int? dimtfill;
        public PyWrapper<Color> dimtfillclr;
        public bool? dimtih;
        public bool? dimtix;
        public double? dimtm;
        public int? dimtmove;
        public bool? dimtofl;
        public bool? dimtoh;
        public bool? dimtol;
        public int? dimtolj;
        public double? dimtp;
        public double? dimtsz;
        public double? dimtvp;
        public string dimtxsty;
        public double? dimtxt;
        public int? dimtzin;
        public bool? dimupt;
        public int? dimzin;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? new AcDb.DimStyleTableRecord();
            var dimStyle = (AcDb.DimStyleTableRecord)obj;

            if (dimadec.HasValue) dimStyle.Dimadec = dimadec.Value;
            if (dimalt.HasValue) dimStyle.Dimalt = dimalt.Value;
            if (dimaltd.HasValue) dimStyle.Dimaltd = dimaltd.Value;
            if (dimaltf.HasValue) dimStyle.Dimaltf = dimaltf.Value;
            if (dimaltrnd.HasValue) dimStyle.Dimaltrnd = dimaltrnd.Value;
            if (dimalttd.HasValue) dimStyle.Dimalttd = dimalttd.Value;
            if (dimalttz.HasValue) dimStyle.Dimalttz = dimalttz.Value;
            if (dimaltu.HasValue) dimStyle.Dimaltu = dimaltu.Value;
            if (dimaltz.HasValue) dimStyle.Dimaltz = dimaltz.Value;
            if (dimapost != null) dimStyle.Dimapost = dimapost;
            if (dimarcsym.HasValue) dimStyle.Dimarcsym = dimarcsym.Value;
            if (dimasz.HasValue) dimStyle.Dimasz = dimasz.Value;
            if (dimatfit.HasValue) dimStyle.Dimatfit = dimatfit.Value;
            if (dimaunit.HasValue) dimStyle.Dimaunit = dimaunit.Value;
            if (dimazin.HasValue) dimStyle.Dimazin = dimazin.Value;

            if (dimblk != null)
            {
                var blkId = db.GetDimblk("DIMBLK", dimblk);
                if (blkId.IsValid) dimStyle.Dimblk = blkId;
            }

            // TODO does not display the specified block...
            if (dimblk1 != null)
            {
                var blkId = db.GetDimblk("DIMBLK1", dimblk1);
                if (blkId.IsValid) dimStyle.Dimblk1 = blkId;
            }

            // TODO does not display the specified block...
            if (dimblk2 != null)
            {
                var blkId = db.GetDimblk("DIMBLK2", dimblk2);
                if (blkId.IsValid) dimStyle.Dimblk2 = blkId;
            }

            if (dimcen.HasValue) dimStyle.Dimcen = dimcen.Value;
            if (dimclrd != null) dimStyle.Dimclrd = dimclrd.__mbr__.ToArx();
            if (dimclre != null) dimStyle.Dimclre = dimclre.__mbr__.ToArx();
            if (dimclrt != null) dimStyle.Dimclrt = dimclrt.__mbr__.ToArx();
            if (dimdec.HasValue) dimStyle.Dimdec = dimdec.Value;
            if (dimdle.HasValue) dimStyle.Dimdle = dimdle.Value;
            if (dimdli.HasValue) dimStyle.Dimdli = dimdli.Value;
            if (!string.IsNullOrEmpty(dimdsep)) dimStyle.Dimdsep = dimdsep[0];
            if (dimexe.HasValue) dimStyle.Dimexe = dimexe.Value;
            if (dimexo.HasValue) dimStyle.Dimexo = dimexo.Value;
            if (dimfrac.HasValue) dimStyle.Dimfrac = dimfrac.Value;
            if (dimfxlen.HasValue) dimStyle.Dimfxlen = dimfxlen.Value;
            if (dimfxlenOn.HasValue) dimStyle.DimfxlenOn = dimfxlenOn.Value;
            if (dimgap.HasValue) dimStyle.Dimgap = dimgap.Value;
            if (dimjogang.HasValue) dimStyle.Dimjogang = dimjogang.Value;
            if (dimjust.HasValue) dimStyle.Dimjust = dimjust.Value;

            if (dimldrblk != null)
            {
                var blkId = db.GetDimblk("DIMLDRBLK", dimldrblk);
                if (blkId.IsValid) dimStyle.Dimldrblk = blkId;
            }

            if (dimlfac.HasValue) dimStyle.Dimlfac = dimlfac.Value;
            if (dimlim.HasValue) dimStyle.Dimlim = dimlim.Value;

            if (dimltex1 != null)
            {
                var linetype = db.GetLinetype(dimltex1);
                if (linetype != null) dimStyle.Dimltex1 = linetype.Id;
            }

            if (dimltex2 != null)
            {
                var linetype = db.GetLinetype(dimltex2);
                if (linetype != null) dimStyle.Dimltex2 = linetype.Id;
            }

            if (dimltype != null)
            {
                var linetype = db.GetLinetype(dimltype);
                if (linetype != null) dimStyle.Dimltype = linetype.Id;
            }

            if (dimlunit.HasValue) dimStyle.Dimlunit = dimlunit.Value;
            if (dimlwd.HasValue) dimStyle.Dimlwd = dimlwd.Value;
            if (dimlwe.HasValue) dimStyle.Dimlwe = dimlwe.Value;
            if (dimpost != null) dimStyle.Dimpost = dimpost;
            if (dimrnd.HasValue) dimStyle.Dimrnd = dimrnd.Value;
            if (dimsah.HasValue) dimStyle.Dimsah = dimsah.Value;
            if (dimscale.HasValue) dimStyle.Dimscale = dimscale.Value;
            if (dimsd1.HasValue) dimStyle.Dimsd1 = dimsd1.Value;
            if (dimsd2.HasValue) dimStyle.Dimsd2 = dimsd2.Value;
            if (dimse1.HasValue) dimStyle.Dimse1 = dimse1.Value;
            if (dimse2.HasValue) dimStyle.Dimse2 = dimse2.Value;
            if (dimsoxd.HasValue) dimStyle.Dimsoxd = dimsoxd.Value;
            if (dimtad.HasValue) dimStyle.Dimtad = dimtad.Value;
            if (dimtdec.HasValue) dimStyle.Dimtdec = dimtdec.Value;
            if (dimtfac.HasValue) dimStyle.Dimtfac = dimtfac.Value;
            if (dimtfill.HasValue) dimStyle.Dimtfill = dimtfill.Value;
            if (dimtfillclr != null)
                dimStyle.Dimtfillclr = dimtfillclr.__mbr__.ToArx();
            if (dimtih.HasValue) dimStyle.Dimtih = dimtih.Value;
            if (dimtix.HasValue) dimStyle.Dimtix = dimtix.Value;
            if (dimtm.HasValue) dimStyle.Dimtm = dimtm.Value;
            if (dimtmove.HasValue) dimStyle.Dimtmove = dimtmove.Value;
            if (dimtofl.HasValue) dimStyle.Dimtofl = dimtofl.Value;
            if (dimtoh.HasValue) dimStyle.Dimtoh = dimtoh.Value;
            if (dimtol.HasValue) dimStyle.Dimtol = dimtol.Value;
            if (dimtolj.HasValue) dimStyle.Dimtolj = dimtolj.Value;
            if (dimtp.HasValue) dimStyle.Dimtp = dimtp.Value;
            if (dimtsz.HasValue) dimStyle.Dimtsz = dimtsz.Value;
            if (dimtvp.HasValue) dimStyle.Dimtvp = dimtvp.Value;

            if (dimtxsty != null)
            {
                var style = db.GetTextStyle(dimtxsty);
                if (style != null) dimStyle.Dimtxsty = style.ObjectId;
            }

            if (dimtxt.HasValue) dimStyle.Dimtxt = dimtxt.Value;
            if (dimtzin.HasValue) dimStyle.Dimtzin = dimtzin.Value;
            if (dimupt.HasValue) dimStyle.Dimupt = dimupt.Value;
            if (dimzin.HasValue) dimStyle.Dimzin = dimzin.Value;

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dimStyle = (AcDb.DimStyleTableRecord)obj;
            var trans = db.TransactionManager.TopTransaction;

            dimadec = dimStyle.Dimadec;
            dimalt = dimStyle.Dimalt;
            dimaltd = dimStyle.Dimaltd;
            dimaltf = dimStyle.Dimaltf;
            dimaltrnd = dimStyle.Dimaltrnd;
            dimalttd = dimStyle.Dimalttd;
            dimalttz = dimStyle.Dimalttz;
            dimaltu = dimStyle.Dimaltu;
            dimaltz = dimStyle.Dimaltz;
            dimapost = dimStyle.Dimapost;
            dimarcsym = dimStyle.Dimarcsym;
            dimasz = dimStyle.Dimasz;
            dimatfit = dimStyle.Dimatfit;
            dimaunit = dimStyle.Dimaunit;
            dimazin = dimStyle.Dimazin;

            if (dimStyle.Dimblk.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimblk, AcDb.OpenMode.ForRead);
                dimblk = symbol.Name;
            }

            if (dimStyle.Dimblk1.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimblk1, AcDb.OpenMode.ForRead);
                dimblk1 = symbol.Name;
            }

            if (dimStyle.Dimblk2.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimblk2, AcDb.OpenMode.ForRead);
                dimblk2 = symbol.Name;
            }

            dimcen = dimStyle.Dimcen;
            dimclrd = PyWrapper<Color>.Create(Color.FromArx(dimStyle.Dimclrd));
            dimclre = PyWrapper<Color>.Create(Color.FromArx(dimStyle.Dimclre));
            dimclrt = PyWrapper<Color>.Create(Color.FromArx(dimStyle.Dimclrt));
            dimdec = dimStyle.Dimdec;
            dimdle = dimStyle.Dimdle;
            dimdli = dimStyle.Dimdli;
            dimdsep = dimStyle.Dimdsep.ToString();
            dimexe = dimStyle.Dimexe;
            dimexo = dimStyle.Dimexo;
            dimfrac = dimStyle.Dimfrac;
            dimfxlen = dimStyle.Dimfxlen;
            dimfxlenOn = dimStyle.DimfxlenOn;
            dimgap = dimStyle.Dimgap;
            dimjogang = dimStyle.Dimjogang;
            dimjust = dimStyle.Dimjust;

            if (dimStyle.Dimldrblk.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimldrblk, AcDb.OpenMode.ForRead);
                dimldrblk = symbol.Name;
            }

            dimlfac = dimStyle.Dimlfac;
            dimlim = dimStyle.Dimlim;

            if (dimStyle.Dimltex1.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimltex1, AcDb.OpenMode.ForRead);
                dimltex1 = symbol.Name;
            }

            if (dimStyle.Dimltex2.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimltex2, AcDb.OpenMode.ForRead);
                dimltex2 = symbol.Name;
            }

            if (dimStyle.Dimltype.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimltype, AcDb.OpenMode.ForRead);
                dimltype = symbol.Name;
            }

            dimlunit = dimStyle.Dimlunit;
            dimlwd = dimStyle.Dimlwd;
            dimlwe = dimStyle.Dimlwe;
            dimpost = dimStyle.Dimpost;
            dimrnd = dimStyle.Dimrnd;
            dimsah = dimStyle.Dimsah;
            dimscale = dimStyle.Dimscale;
            dimsd1 = dimStyle.Dimsd1;
            dimsd2 = dimStyle.Dimsd2;
            dimse1 = dimStyle.Dimse1;
            dimse2 = dimStyle.Dimse2;
            dimsoxd = dimStyle.Dimsoxd;
            dimtad = dimStyle.Dimtad;
            dimtdec = dimStyle.Dimtdec;
            dimtfac = dimStyle.Dimtfac;
            dimtfill = dimStyle.Dimtfill;
            dimtfillclr = PyWrapper<Color>.Create(
                Color.FromArx(dimStyle.Dimtfillclr));
            dimtih = dimStyle.Dimtih;
            dimtix = dimStyle.Dimtix;
            dimtm = dimStyle.Dimtm;
            dimtmove = dimStyle.Dimtmove;
            dimtofl = dimStyle.Dimtofl;
            dimtoh = dimStyle.Dimtoh;
            dimtol = dimStyle.Dimtol;
            dimtolj = dimStyle.Dimtolj;
            dimtp = dimStyle.Dimtp;
            dimtsz = dimStyle.Dimtsz;
            dimtvp = dimStyle.Dimtvp;

            if (dimStyle.Dimtxsty.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimtxsty, AcDb.OpenMode.ForRead);
                dimtxsty = symbol.Name;
            }

            dimtxt = dimStyle.Dimtxt;
            dimtzin = dimStyle.Dimtzin;
            dimupt = dimStyle.Dimupt;
            dimzin = dimStyle.Dimzin;

            return base.FromArx(obj, db);
        }

        public override AcDb.SymbolTableRecord GetFromSymbolTable(
            AcDb.Database db) => db.GetDimStyle(name);

        public override AcDb.ObjectId AddToSymbolTable(
            AcDb.SymbolTableRecord symbol, AcDb.Database db) =>
            db.AddDimStyle((AcDb.DimStyleTableRecord)symbol);
    }

    [PyType("sacad.acdb.LayerTableRecord")]
    public sealed class LayerTableRecord : SymbolTableRecord
    {
        public PyWrapper<Color> color;
        public bool? is_frozen;
        public bool? is_locked;
        public bool? is_off;
        public bool? is_plottable;
        public AcDb.LineWeight? line_weight;
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
            if (line_weight.HasValue) layer.LineWeight = line_weight.Value;
            if (linetype != null)
            {
                var ltype = db.GetLinetype(linetype);
                if (ltype != null) layer.LinetypeObjectId = ltype.ObjectId;
            }

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var layer = (AcDb.LayerTableRecord)obj;
            var trans = db.TransactionManager.TopTransaction;

            color = PyWrapper<Color>.Create(Color.FromArx(layer.Color));
            line_weight = layer.LineWeight;
            is_frozen = Util.ToOptional(layer.IsFrozen);
            is_locked = Util.ToOptional(layer.IsLocked);
            is_off = Util.ToOptional(layer.IsOff);
            is_plottable = Util.ToOptional(layer.IsPlottable);

            if (layer.LinetypeObjectId.IsValid)
            {
                linetype = (trans.GetObject(layer.LinetypeObjectId,
                    AcDb.OpenMode.ForRead) as AcDb.LinetypeTableRecord)?.Name;
            }

            return base.FromArx(obj, db);
        }

        public override AcDb.SymbolTableRecord GetFromSymbolTable(
            AcDb.Database db) => db.GetLayer(name);

        public override AcDb.ObjectId AddToSymbolTable(
            AcDb.SymbolTableRecord symbol, AcDb.Database db) =>
            db.AddLayer((AcDb.LayerTableRecord)symbol);
    }

    [PyType("sacad.acdb.LinetypeSegment")]
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

    [PyType("sacad.acdb.LinetypeTableRecord")]
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
                    if (seg.text != null)
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

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
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

    [PyType("sacad.acdb.FontDescriptor")]
    public sealed class FontDescriptor
    {
        public bool? bold;
        public int? character_set;
        public bool? italic;
        public int? pitch_and_family;
        public string type_face;
    }

    [PyType("sacad.acdb.TextStyleTableRecord")]
    public sealed class TextStyleTableRecord : SymbolTableRecord
    {
        public string big_font_file_name;
        public string file_name;
        public int? flag_bits;
        public PyWrapper<FontDescriptor> font;
        public bool? is_shape_file;
        public bool? is_vertical;
        public double? obliquing_angle;
        public double? prior_size;
        public double? text_size;
        public double? x_scale;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? new AcDb.TextStyleTableRecord();
            var textStyle = (AcDb.TextStyleTableRecord)obj;

            if (font?.__mbr__ != null)
            {
                textStyle.Font = new AcGi.FontDescriptor(
                    font.__mbr__.type_face,
                    font.__mbr__.bold ?? false,
                    font.__mbr__.italic ?? false,
                    font.__mbr__.character_set ?? 0,
                    font.__mbr__.pitch_and_family ?? 0);
            }

            if (!string.IsNullOrEmpty(big_font_file_name))
                textStyle.BigFontFileName = big_font_file_name;
            if (!string.IsNullOrEmpty(file_name))
                textStyle.FileName = file_name;
            if (flag_bits.HasValue)
                textStyle.FlagBits = Convert.ToByte(flag_bits.Value);
            if (is_shape_file.HasValue)
                textStyle.IsShapeFile = is_shape_file.Value;
            if (is_vertical.HasValue) textStyle.IsVertical = is_vertical.Value;
            if (obliquing_angle.HasValue)
                textStyle.ObliquingAngle = obliquing_angle.Value;
            if (prior_size.HasValue) textStyle.PriorSize = prior_size.Value;
            if (text_size.HasValue) textStyle.TextSize = text_size.Value;
            if (x_scale.HasValue) textStyle.XScale = x_scale.Value;

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var textStyle = (AcDb.TextStyleTableRecord)obj;

            var tfont = textStyle.Font;
            if (!string.IsNullOrEmpty(tfont.TypeFace))
            {
                font = PyWrapper<FontDescriptor>.Create(new FontDescriptor
                {
                    bold = tfont.Bold,
                    italic = tfont.Italic,
                    character_set = tfont.CharacterSet,
                    pitch_and_family = tfont.PitchAndFamily,
                    type_face = tfont.TypeFace,
                });
            }

            big_font_file_name = textStyle.BigFontFileName;
            file_name = textStyle.FileName;
            flag_bits = Util.ToOptional(textStyle.FlagBits);
            is_shape_file = Util.ToOptional(textStyle.IsShapeFile);
            is_vertical = Util.ToOptional(textStyle.IsVertical);
            obliquing_angle = Util.ToOptional(textStyle.ObliquingAngle);
            prior_size = textStyle.PriorSize;
            text_size = textStyle.TextSize;
            x_scale = textStyle.XScale;

            return base.FromArx(obj, db);
        }

        public override AcDb.SymbolTableRecord GetFromSymbolTable(
            AcDb.Database db) => is_shape_file == true
            ? db.GetShapeStyle(file_name)
            : db.GetTextStyle(name);

        public override AcDb.ObjectId AddToSymbolTable(
            AcDb.SymbolTableRecord symbol, AcDb.Database db) =>
            db.AddTextStyle((AcDb.TextStyleTableRecord)symbol);
    }
}
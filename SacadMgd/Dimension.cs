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
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType("sacad.acdb.Dimension")]
    public class Dimension : Entity
    {
        public PyWrapper<DimStyleTableRecord> override_style;

        public string alternate_prefix;
        public string alternate_suffix;
        public bool? alt_suppress_leading_zeros;
        public bool? alt_suppress_trailing_zeros;
        public bool? alt_suppress_zero_feet;
        public bool? alt_suppress_zero_inches;
        public bool? alt_tolerance_suppress_leading_zeros;
        public bool? alt_tolerance_suppress_trailing_zeros;
        public bool? alt_tolerance_suppress_zero_feet;
        public bool? alt_tolerance_suppress_zero_inches;
        public double? center_mark_size;
        public AcDb.DimensionCenterMarkType? center_mark_type;
        public string dimension_style_name;
        public string dimension_text;
        public double? elevation;
        public double? horizontal_rotation;
        public double? measurement;
        public Vector3d normal;
        public string prefix;
        public string suffix;
        public bool? suppress_angular_leading_zeros;
        public bool? suppress_angular_trailing_zeros;
        public bool? suppress_leading_zeros;
        public bool? suppress_trailing_zeros;
        public bool? suppress_zero_feet;
        public bool? suppress_zero_inches;
        public AcDb.AttachmentPoint? text_attachment;
        public double? text_line_spacing_factor;
        public AcDb.LineSpacingStyle? text_line_spacing_style;
        public Vector2d text_offset;
        public Vector3d text_position;
        public double? text_rotation;
        public bool? tolerance_suppress_leading_zeros;
        public bool? tolerance_suppress_trailing_zeros;
        public bool? tolerance_suppress_zero_feet;
        public bool? tolerance_suppress_zero_inches;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.Dimension)obj;

            if (alternate_prefix != null)
                dim.AlternatePrefix = alternate_prefix;
            if (alternate_suffix != null)
                dim.AlternateSuffix = alternate_suffix;
            if (alt_suppress_leading_zeros.HasValue)
                dim.AltSuppressLeadingZeros = alt_suppress_leading_zeros.Value;

            if (alt_suppress_trailing_zeros.HasValue)
            {
                dim.AltSuppressTrailingZeros =
                    alt_suppress_trailing_zeros.Value;
            }

            if (alt_suppress_zero_feet.HasValue)
                dim.AltSuppressZeroFeet = alt_suppress_zero_feet.Value;
            if (alt_suppress_zero_inches.HasValue)
                dim.AltSuppressZeroInches = alt_suppress_zero_inches.Value;

            if (alt_tolerance_suppress_leading_zeros.HasValue)
            {
                dim.AltToleranceSuppressLeadingZeros =
                    alt_tolerance_suppress_leading_zeros.Value;
            }

            if (alt_tolerance_suppress_trailing_zeros.HasValue)
            {
                dim.AltToleranceSuppressTrailingZeros =
                    alt_tolerance_suppress_trailing_zeros.Value;
            }

            if (alt_tolerance_suppress_zero_feet.HasValue)
            {
                dim.AltToleranceSuppressZeroFeet =
                    alt_tolerance_suppress_zero_feet.Value;
            }

            if (alt_tolerance_suppress_zero_inches.HasValue)
            {
                dim.AltToleranceSuppressZeroInches =
                    alt_tolerance_suppress_zero_inches.Value;
            }

            if (dimension_style_name != null)
            {
                var style = db.GetDimStyle(dimension_style_name);
                if (style != null) dim.DimensionStyle = style.ObjectId;
            }

            if (dimension_text != null) dim.DimensionText = dimension_text;
            if (elevation.HasValue) dim.Elevation = elevation.Value;
            if (horizontal_rotation.HasValue)
                dim.HorizontalRotation = horizontal_rotation.Value;
            if (normal != null) dim.Normal = normal.ToVector3d();
            if (prefix != null) dim.Prefix = prefix;
            if (suffix != null) dim.Suffix = suffix;

            if (suppress_angular_leading_zeros.HasValue)
            {
                dim.SuppressAngularLeadingZeros =
                    suppress_angular_leading_zeros.Value;
            }

            if (suppress_angular_trailing_zeros.HasValue)
            {
                dim.SuppressAngularTrailingZeros =
                    suppress_angular_trailing_zeros.Value;
            }

            if (suppress_leading_zeros.HasValue)
                dim.SuppressLeadingZeros = suppress_leading_zeros.Value;
            if (suppress_trailing_zeros.HasValue)
                dim.SuppressTrailingZeros = suppress_trailing_zeros.Value;
            if (suppress_zero_feet.HasValue)
                dim.SuppressZeroFeet = suppress_zero_feet.Value;
            if (suppress_zero_inches.HasValue)
                dim.SuppressZeroInches = suppress_zero_inches.Value;
            if (text_attachment.HasValue)
                dim.TextAttachment = text_attachment.Value;
            if (text_line_spacing_factor.HasValue)
                dim.TextLineSpacingFactor = text_line_spacing_factor.Value;
            if (text_line_spacing_style.HasValue)
                dim.TextLineSpacingStyle = text_line_spacing_style.Value;

            if (text_offset != null)
            {
                var offset = new AcGe.Vector3d(text_offset.X, text_offset.Y, 0);
                if (matrix != null)
                {
                    var mtx = matrix.ToMatrix3d();
                    matrix = null;

                    dim.TransformBy(mtx);
                    offset = offset.TransformBy(mtx);
                }

                dim.GenerateLayout();
                dim.TextPosition += offset;
            }

            if (text_position != null)
                dim.TextPosition = text_position.ToPoint3d();
            if (text_rotation.HasValue) dim.TextRotation = text_rotation.Value;

            if (tolerance_suppress_leading_zeros.HasValue)
            {
                dim.ToleranceSuppressLeadingZeros =
                    tolerance_suppress_leading_zeros.Value;
            }

            if (tolerance_suppress_trailing_zeros.HasValue)
            {
                dim.ToleranceSuppressTrailingZeros =
                    tolerance_suppress_trailing_zeros.Value;
            }

            if (tolerance_suppress_zero_feet.HasValue)
            {
                dim.ToleranceSuppressZeroFeet =
                    tolerance_suppress_zero_feet.Value;
            }

            if (tolerance_suppress_zero_inches.HasValue)
            {
                dim.ToleranceSuppressZeroInches =
                    tolerance_suppress_zero_inches.Value;
            }

            if (override_style != null)
            {
                var o = override_style.__mbr__;

                if (o.dimadec.HasValue) dim.Dimadec = o.dimadec.Value;
                if (o.dimalt.HasValue) dim.Dimalt = o.dimalt.Value;
                if (o.dimaltd.HasValue) dim.Dimaltd = o.dimaltd.Value;
                if (o.dimaltf.HasValue) dim.Dimaltf = o.dimaltf.Value;
                if (o.dimaltrnd.HasValue) dim.Dimaltrnd = o.dimaltrnd.Value;
                if (o.dimalttd.HasValue) dim.Dimalttd = o.dimalttd.Value;
                if (o.dimalttz.HasValue) dim.Dimalttz = o.dimalttz.Value;
                if (o.dimaltu.HasValue) dim.Dimaltu = o.dimaltu.Value;
                if (o.dimaltz.HasValue) dim.Dimaltz = o.dimaltz.Value;
                if (o.dimapost != null) dim.Dimapost = o.dimapost;
                if (o.dimarcsym.HasValue) dim.Dimarcsym = o.dimarcsym.Value;
                if (o.dimasz.HasValue) dim.Dimasz = o.dimasz.Value;
                if (o.dimatfit.HasValue) dim.Dimatfit = o.dimatfit.Value;
                if (o.dimaunit.HasValue) dim.Dimaunit = o.dimaunit.Value;
                if (o.dimazin.HasValue) dim.Dimazin = o.dimazin.Value;

                if (o.dimblk != null)
                {
                    if (o.dimblk == string.Empty)
                    {
                        dim.Dimblk = AcDb.ObjectId.Null;
                    }
                    else
                    {
                        var blkId = db.GetDimblk("DIMBLK", o.dimblk);
                        if (blkId.IsValid) dim.Dimblk = blkId;
                    }
                }

                if (o.dimblk1 != null)
                {
                    if (o.dimblk1 == string.Empty)
                    {
                        dim.Dimblk1 = AcDb.ObjectId.Null;
                    }
                    else
                    {
                        var blkId = db.GetDimblk("DIMBLK1", o.dimblk1);
                        if (blkId.IsValid) dim.Dimblk1 = blkId;
                    }
                }

                if (o.dimblk2 != null)
                {
                    if (o.dimblk2 == string.Empty)
                    {
                        dim.Dimblk2 = AcDb.ObjectId.Null;
                    }
                    else
                    {
                        var blkId = db.GetDimblk("DIMBLK2", o.dimblk2);
                        if (blkId.IsValid) dim.Dimblk2 = blkId;
                    }
                }

                if (o.dimcen.HasValue) dim.Dimcen = o.dimcen.Value;
                if (o.dimclrd != null) dim.Dimclrd = o.dimclrd.__mbr__.ToArx();
                if (o.dimclre != null) dim.Dimclre = o.dimclre.__mbr__.ToArx();
                if (o.dimclrt != null) dim.Dimclrt = o.dimclrt.__mbr__.ToArx();
                if (o.dimdec.HasValue) dim.Dimdec = o.dimdec.Value;
                if (o.dimdle.HasValue) dim.Dimdle = o.dimdle.Value;
                if (o.dimdli.HasValue) dim.Dimdli = o.dimdli.Value;
                if (!string.IsNullOrEmpty(o.dimdsep))
                    dim.Dimdsep = o.dimdsep[0];
                if (o.dimexe.HasValue) dim.Dimexe = o.dimexe.Value;
                if (o.dimexo.HasValue) dim.Dimexo = o.dimexo.Value;
                if (o.dimfrac.HasValue) dim.Dimfrac = o.dimfrac.Value;
                if (o.dimfxlen.HasValue) dim.Dimfxlen = o.dimfxlen.Value;
                if (o.dimfxlenOn.HasValue) dim.DimfxlenOn = o.dimfxlenOn.Value;
                if (o.dimgap.HasValue) dim.Dimgap = o.dimgap.Value;
                if (o.dimjogang.HasValue) dim.Dimjogang = o.dimjogang.Value;
                if (o.dimjust.HasValue) dim.Dimjust = o.dimjust.Value;

                if (o.dimldrblk != null)
                {
                    var blkId = db.GetDimblk("DIMLDRBLK", o.dimldrblk);
                    if (blkId.IsValid) dim.Dimldrblk = blkId;
                }

                if (o.dimlfac.HasValue) dim.Dimlfac = o.dimlfac.Value;
                if (o.dimlim.HasValue) dim.Dimlim = o.dimlim.Value;

                if (o.dimltex1 != null)
                {
                    var ltype = db.GetLinetype(o.dimltex1);
                    if (ltype != null) dim.Dimltex1 = ltype.ObjectId;
                }

                if (o.dimltex2 != null)
                {
                    var ltype = db.GetLinetype(o.dimltex2);
                    if (ltype != null) dim.Dimltex2 = ltype.ObjectId;
                }

                if (o.dimltype != null)
                {
                    var ltype = db.GetLinetype(o.dimltype);
                    if (ltype != null) dim.Dimltype = ltype.ObjectId;
                }

                if (o.dimlunit.HasValue) dim.Dimlunit = o.dimlunit.Value;
                if (o.dimlwd.HasValue) dim.Dimlwd = o.dimlwd.Value;
                if (o.dimlwe.HasValue) dim.Dimlwe = o.dimlwe.Value;
                if (o.dimpost != null) dim.Dimpost = o.dimpost;
                if (o.dimrnd.HasValue) dim.Dimrnd = o.dimrnd.Value;
                if (o.dimsah.HasValue) dim.Dimsah = o.dimsah.Value;
                if (o.dimscale.HasValue) dim.Dimscale = o.dimscale.Value;
                if (o.dimsd1.HasValue) dim.Dimsd1 = o.dimsd1.Value;
                if (o.dimsd2.HasValue) dim.Dimsd2 = o.dimsd2.Value;
                if (o.dimse1.HasValue) dim.Dimse1 = o.dimse1.Value;
                if (o.dimse2.HasValue) dim.Dimse2 = o.dimse2.Value;
                if (o.dimsoxd.HasValue) dim.Dimsoxd = o.dimsoxd.Value;
                if (o.dimtad.HasValue) dim.Dimtad = o.dimtad.Value;
                if (o.dimtdec.HasValue) dim.Dimtdec = o.dimtdec.Value;
                if (o.dimtfac.HasValue) dim.Dimtfac = o.dimtfac.Value;
                if (o.dimtfill.HasValue) dim.Dimtfill = o.dimtfill.Value;
                if (o.dimtfillclr != null)
                    dim.Dimtfillclr = o.dimtfillclr.__mbr__.ToArx();
                if (o.dimtih.HasValue) dim.Dimtih = o.dimtih.Value;
                if (o.dimtix.HasValue) dim.Dimtix = o.dimtix.Value;
                if (o.dimtm.HasValue) dim.Dimtm = o.dimtm.Value;
                if (o.dimtmove.HasValue) dim.Dimtmove = o.dimtmove.Value;
                if (o.dimtofl.HasValue) dim.Dimtofl = o.dimtofl.Value;
                if (o.dimtoh.HasValue) dim.Dimtoh = o.dimtoh.Value;
                if (o.dimtol.HasValue) dim.Dimtol = o.dimtol.Value;
                if (o.dimtolj.HasValue) dim.Dimtolj = o.dimtolj.Value;
                if (o.dimtp.HasValue) dim.Dimtp = o.dimtp.Value;
                if (o.dimtsz.HasValue) dim.Dimtsz = o.dimtsz.Value;
                if (o.dimtvp.HasValue) dim.Dimtvp = o.dimtvp.Value;
                if (o.dimtxt.HasValue) dim.Dimtxt = o.dimtxt.Value;
                if (o.dimtzin.HasValue) dim.Dimtzin = o.dimtzin.Value;
                if (o.dimupt.HasValue) dim.Dimupt = o.dimupt.Value;
                if (o.dimzin.HasValue) dim.Dimzin = o.dimzin.Value;

                if (!string.IsNullOrEmpty(o.dimtxsty))
                    dim.TextStyleId = db.GetTextStyle(o.dimtxsty).ObjectId;
            }

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.Dimension)obj;

            alternate_prefix = dim.AlternatePrefix;
            alternate_suffix = dim.AlternateSuffix;
            alt_suppress_leading_zeros = dim.AltSuppressLeadingZeros;
            alt_suppress_trailing_zeros = dim.AltSuppressTrailingZeros;
            alt_suppress_zero_feet = dim.AltSuppressZeroFeet;
            alt_suppress_zero_inches = dim.AltSuppressZeroInches;
            alt_tolerance_suppress_leading_zeros =
                dim.AltToleranceSuppressLeadingZeros;
            alt_tolerance_suppress_trailing_zeros =
                dim.AltToleranceSuppressTrailingZeros;
            alt_tolerance_suppress_zero_feet = dim.AltToleranceSuppressZeroFeet;
            alt_tolerance_suppress_zero_inches =
                dim.AltToleranceSuppressZeroInches;
            center_mark_size = dim.CenterMarkSize;
            center_mark_type = dim.CenterMarkType;
            dimension_style_name = dim.DimensionStyleName;
            dimension_text = dim.DimensionText;
            elevation = dim.Elevation;
            horizontal_rotation = dim.HorizontalRotation;
            measurement = dim.Measurement;
            normal = dim.Normal;
            prefix = dim.Prefix;
            suffix = dim.Suffix;
            suppress_angular_leading_zeros = dim.SuppressAngularLeadingZeros;
            suppress_angular_trailing_zeros = dim.SuppressAngularTrailingZeros;
            suppress_leading_zeros = dim.SuppressLeadingZeros;
            suppress_trailing_zeros = dim.SuppressTrailingZeros;
            suppress_zero_feet = dim.SuppressZeroFeet;
            suppress_zero_inches = dim.SuppressZeroInches;
            text_attachment = dim.TextAttachment;
            text_line_spacing_factor = dim.TextLineSpacingFactor;
            text_line_spacing_style = dim.TextLineSpacingStyle;
            text_position = dim.TextPosition;
            text_rotation = dim.TextRotation;
            tolerance_suppress_leading_zeros =
                dim.ToleranceSuppressLeadingZeros;
            tolerance_suppress_trailing_zeros =
                dim.ToleranceSuppressTrailingZeros;
            tolerance_suppress_zero_feet = dim.ToleranceSuppressZeroFeet;
            tolerance_suppress_zero_inches = dim.ToleranceSuppressZeroInches;

            var trans = db.TransactionManager.TopTransaction;
            var dimStyle = dim.DimensionStyle.IsValid
                ? (AcDb.DimStyleTableRecord)trans.GetObject(dim.DimensionStyle,
                    AcDb.OpenMode.ForRead)
                : DefaultStyle;

            if (dim.Dimadec != dimStyle.Dimadec)
                GetOverrides().dimadec = dim.Dimadec;
            if (dim.Dimalt != dimStyle.Dimalt)
                GetOverrides().dimalt = dim.Dimalt;
            if (dim.Dimaltd != dimStyle.Dimaltd)
                GetOverrides().dimaltd = dim.Dimaltd;
            if (Math.Abs(dim.Dimaltf - dimStyle.Dimaltf) > Tolerance)
                GetOverrides().dimaltf = dim.Dimaltf;
            if (Math.Abs(dim.Dimaltrnd - dimStyle.Dimaltrnd) > Tolerance)
                GetOverrides().dimaltrnd = dim.Dimaltrnd;
            if (dim.Dimalttd != dimStyle.Dimalttd)
                GetOverrides().dimalttd = dim.Dimalttd;
            if (dim.Dimalttz != dimStyle.Dimalttz)
                GetOverrides().dimalttz = dim.Dimalttz;
            if (dim.Dimaltu != dimStyle.Dimaltu)
                GetOverrides().dimaltu = dim.Dimaltu;
            if (dim.Dimaltz != dimStyle.Dimaltz)
                GetOverrides().dimaltz = dim.Dimaltz;
            if (dim.Dimapost != dimStyle.Dimapost)
                GetOverrides().dimapost = dim.Dimapost;
            if (dim.Dimarcsym != dimStyle.Dimarcsym)
                GetOverrides().dimarcsym = dim.Dimarcsym;
            if (Math.Abs(dim.Dimasz - dimStyle.Dimasz) > Tolerance)
                GetOverrides().dimasz = dim.Dimasz;
            if (dim.Dimatfit != dimStyle.Dimatfit)
                GetOverrides().dimatfit = dim.Dimatfit;
            if (dim.Dimaunit != dimStyle.Dimaunit)
                GetOverrides().dimaunit = dim.Dimaunit;
            if (dim.Dimazin != dimStyle.Dimazin)
                GetOverrides().dimazin = dim.Dimazin;

            if (dim.Dimblk != dimStyle.Dimblk && dim.Dimblk.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dim.Dimblk, AcDb.OpenMode.ForRead);
                GetOverrides().dimblk = symbol.Name;
            }

            if (dim.Dimblk1 != dimStyle.Dimblk1 && dim.Dimblk1.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dim.Dimblk1, AcDb.OpenMode.ForRead);
                GetOverrides().dimblk1 = symbol.Name;
            }

            if (dim.Dimblk2 != dimStyle.Dimblk2 && dim.Dimblk2.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dim.Dimblk2, AcDb.OpenMode.ForRead);
                GetOverrides().dimblk2 = symbol.Name;
            }

            if (Math.Abs(dim.Dimcen - dimStyle.Dimcen) > Tolerance)
                GetOverrides().dimcen = dim.Dimcen;

            if (dim.Dimclrd != dimStyle.Dimclrd)
            {
                GetOverrides().dimclrd = PyWrapper<Color>.Create(
                    Color.FromArx(dim.Dimclrd));
            }

            if (dim.Dimclre != dimStyle.Dimclre)
            {
                GetOverrides().dimclre = PyWrapper<Color>.Create(
                    Color.FromArx(dim.Dimclre));
            }

            if (dim.Dimclrt != dimStyle.Dimclrt)
            {
                GetOverrides().dimclrt = PyWrapper<Color>.Create(
                    Color.FromArx(dim.Dimclrt));
            }

            if (dim.Dimdec != dimStyle.Dimdec)
                GetOverrides().dimdec = dim.Dimdec;
            if (Math.Abs(dim.Dimdle - dimStyle.Dimdle) > Tolerance)
                GetOverrides().dimdle = dim.Dimdle;
            if (Math.Abs(dim.Dimdli - dimStyle.Dimdli) > Tolerance)
                GetOverrides().dimdli = dim.Dimdli;
            if (dim.Dimdsep != dimStyle.Dimdsep)
                GetOverrides().dimdsep = dim.Dimdsep.ToString();
            if (Math.Abs(dim.Dimexe - dimStyle.Dimexe) > Tolerance)
                GetOverrides().dimexe = dim.Dimexe;
            if (Math.Abs(dim.Dimexo - dimStyle.Dimexo) > Tolerance)
                GetOverrides().dimexo = dim.Dimexo;
            if (dim.Dimfrac != dimStyle.Dimfrac)
                GetOverrides().dimfrac = dim.Dimfrac;
            if (Math.Abs(dim.Dimfxlen - dimStyle.Dimfxlen) > Tolerance)
                GetOverrides().dimfxlen = dim.Dimfxlen;
            if (dim.DimfxlenOn != dimStyle.DimfxlenOn)
                GetOverrides().dimfxlenOn = dim.DimfxlenOn;
            if (Math.Abs(dim.Dimgap - dimStyle.Dimgap) > Tolerance)
                GetOverrides().dimgap = dim.Dimgap;
            if (Math.Abs(dim.Dimjogang - dimStyle.Dimjogang) > Tolerance)
                GetOverrides().dimjogang = dim.Dimjogang;
            if (dim.Dimjust != dimStyle.Dimjust)
                GetOverrides().dimjust = dim.Dimjust;

            if (dim.Dimldrblk != dimStyle.Dimldrblk &&
                dim.Dimldrblk.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dim.Dimldrblk, AcDb.OpenMode.ForRead);
                GetOverrides().dimldrblk = symbol.Name;
            }

            if (Math.Abs(dim.Dimlfac - dimStyle.Dimlfac) > Tolerance)
                GetOverrides().dimlfac = dim.Dimlfac;
            if (dim.Dimlim != dimStyle.Dimlim)
                GetOverrides().dimlim = dim.Dimlim;

            if (dim.Dimltex1 != dimStyle.Dimltex1 && dim.Dimltex1.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dim.Dimltex1, AcDb.OpenMode.ForRead);
                GetOverrides().dimltex1 = symbol.Name;
            }

            if (dim.Dimltex2 != dimStyle.Dimltex2 && dim.Dimltex2.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dim.Dimltex2, AcDb.OpenMode.ForRead);
                GetOverrides().dimltex2 = symbol.Name;
            }

            if (dim.Dimltype != dimStyle.Dimltype && dim.Dimltype.IsValid)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dim.Dimltype, AcDb.OpenMode.ForRead);
                GetOverrides().dimltype = symbol.Name;
            }

            if (dim.Dimlunit != dimStyle.Dimlunit)
                GetOverrides().dimlunit = dim.Dimlunit;
            if (dim.Dimlwd != dimStyle.Dimlwd)
                GetOverrides().dimlwd = dim.Dimlwd;
            if (dim.Dimlwe != dimStyle.Dimlwe)
                GetOverrides().dimlwe = dim.Dimlwe;
            if (dim.Dimpost != dimStyle.Dimpost)
                GetOverrides().dimpost = dim.Dimpost;
            if (Math.Abs(dim.Dimrnd - dimStyle.Dimrnd) > Tolerance)
                GetOverrides().dimrnd = dim.Dimrnd;
            if (dim.Dimsah != dimStyle.Dimsah)
                GetOverrides().dimsah = dim.Dimsah;
            if (Math.Abs(dim.Dimscale - dimStyle.Dimscale) > Tolerance)
                GetOverrides().dimscale = dim.Dimscale;
            if (dim.Dimsd1 != dimStyle.Dimsd1)
                GetOverrides().dimsd1 = dim.Dimsd1;
            if (dim.Dimsd2 != dimStyle.Dimsd2)
                GetOverrides().dimsd2 = dim.Dimsd2;
            if (dim.Dimse1 != dimStyle.Dimse1)
                GetOverrides().dimse1 = dim.Dimse1;
            if (dim.Dimse2 != dimStyle.Dimse2)
                GetOverrides().dimse2 = dim.Dimse2;
            if (dim.Dimsoxd != dimStyle.Dimsoxd)
                GetOverrides().dimsoxd = dim.Dimsoxd;
            if (dim.Dimtad != dimStyle.Dimtad)
                GetOverrides().dimtad = dim.Dimtad;
            if (dim.Dimtdec != dimStyle.Dimtdec)
                GetOverrides().dimtdec = dim.Dimtdec;
            if (Math.Abs(dim.Dimtfac - dimStyle.Dimtfac) > Tolerance)
                GetOverrides().dimtfac = dim.Dimtfac;
            if (dim.Dimtfill != dimStyle.Dimtfill)
                GetOverrides().dimtfill = dim.Dimtfill;

            if (dim.Dimtfillclr != dimStyle.Dimtfillclr)
            {
                GetOverrides().dimtfillclr = PyWrapper<Color>.Create(
                    Color.FromArx(dim.Dimtfillclr));
            }

            if (dim.Dimtih != dimStyle.Dimtih)
                GetOverrides().dimtih = dim.Dimtih;
            if (dim.Dimtix != dimStyle.Dimtix)
                GetOverrides().dimtix = dim.Dimtix;
            if (Math.Abs(dim.Dimtm - dimStyle.Dimtm) > Tolerance)
                GetOverrides().dimtm = dim.Dimtm;
            if (dim.Dimtmove != dimStyle.Dimtmove)
                GetOverrides().dimtmove = dim.Dimtmove;
            if (dim.Dimtofl != dimStyle.Dimtofl)
                GetOverrides().dimtofl = dim.Dimtofl;
            if (dim.Dimtoh != dimStyle.Dimtoh)
                GetOverrides().dimtoh = dim.Dimtoh;
            if (dim.Dimtol != dimStyle.Dimtol)
                GetOverrides().dimtol = dim.Dimtol;
            if (dim.Dimtolj != dimStyle.Dimtolj)
                GetOverrides().dimtolj = dim.Dimtolj;
            if (Math.Abs(dim.Dimtp - dimStyle.Dimtp) > Tolerance)
                GetOverrides().dimtp = dim.Dimtp;
            if (Math.Abs(dim.Dimtsz - dimStyle.Dimtsz) > Tolerance)
                GetOverrides().dimtsz = dim.Dimtsz;
            if (Math.Abs(dim.Dimtvp - dimStyle.Dimtvp) > Tolerance)
                GetOverrides().dimtvp = dim.Dimtvp;
            if (Math.Abs(dim.Dimtxt - dimStyle.Dimtxt) > Tolerance)
                GetOverrides().dimtxt = dim.Dimtxt;
            if (dim.Dimtzin != dimStyle.Dimtzin)
                GetOverrides().dimtzin = dim.Dimtzin;
            if (dim.Dimupt != dimStyle.Dimupt)
                GetOverrides().dimupt = dim.Dimupt;
            if (dim.Dimzin != dimStyle.Dimzin)
                GetOverrides().dimzin = dim.Dimzin;

            if (dim.TextStyleId.IsValid && dim.TextStyleId != dimStyle.Dimtxsty)
            {
                var symbol = (AcDb.SymbolTableRecord)trans.GetObject(
                    dimStyle.Dimtxsty, AcDb.OpenMode.ForRead);
                GetOverrides().dimtxsty = symbol.Name;
            }

            return base.FromArx(obj, db);
        }

        private DimStyleTableRecord GetOverrides()
        {
            if (override_style == null)
            {
                override_style = PyWrapper<DimStyleTableRecord>.Create(
                    new DimStyleTableRecord());
            }

            return override_style.__mbr__;
        }

        private const double Tolerance = 1E-6;

        private static readonly AcDb.DimStyleTableRecord DefaultStyle =
            new AcDb.DimStyleTableRecord();
    }

    [ArxEntity(typeof(AcDb.AlignedDimension))]
    [PyType("sacad.acdb.AlignedDimension")]
    public sealed class AlignedDimension : Dimension
    {
        public Vector3d dim_line_point;
        public double? oblique;
        public Vector3d x_line1_point;
        public Vector3d x_line2_point;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.AlignedDimension>(db);
            var dim = (AcDb.AlignedDimension)obj;

            dim.DimLinePoint = dim_line_point.ToPoint3d();
            if (oblique.HasValue) dim.Oblique = oblique.Value;
            dim.XLine1Point = x_line1_point.ToPoint3d();
            dim.XLine2Point = x_line2_point.ToPoint3d();

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.AlignedDimension)obj;

            dim_line_point = dim.DimLinePoint;
            oblique = Util.ToOptional(dim.Oblique);
            x_line1_point = dim.XLine1Point;
            x_line2_point = dim.XLine2Point;

            return base.FromArx(obj, db);
        }
    }

    [ArxEntity(typeof(AcDb.ArcDimension))]
    [PyType("sacad.acdb.ArcDimension")]
    public sealed class ArcDimension : Dimension
    {
        public double? arc_end_param;
        public Vector3d arc_point;
        public double? arc_start_param;
        public int? arc_symbol_type;
        public Vector3d center_point;
        public bool? has_leader;
        public Vector3d leader1_point;
        public Vector3d leader2_point;
        public Vector3d x_line1_point;
        public Vector3d x_line2_point;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New(db);
            var dim = (AcDb.ArcDimension)obj;

            if (arc_end_param.HasValue) dim.ArcEndParam = arc_end_param.Value;
            if (arc_point != null) dim.ArcPoint = arc_point.ToPoint3d();
            if (arc_start_param.HasValue)
                dim.ArcStartParam = arc_start_param.Value;
            if (arc_symbol_type.HasValue)
                dim.ArcSymbolType = arc_symbol_type.Value;
            if (center_point != null)
                dim.CenterPoint = center_point.ToPoint3d();
            if (has_leader.HasValue)
                dim.HasLeader = has_leader.Value;
            if (leader1_point != null)
                dim.Leader1Point = leader1_point.ToPoint3d();
            if (leader2_point != null)
                dim.Leader2Point = leader2_point.ToPoint3d();
            if (x_line1_point != null)
                dim.XLine1Point = x_line1_point.ToPoint3d();
            if (x_line2_point != null)
                dim.XLine2Point = x_line2_point.ToPoint3d();

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.ArcDimension)obj;

            arc_end_param = dim.ArcEndParam;
            arc_point = dim.ArcPoint;
            arc_start_param = dim.ArcStartParam;
            arc_symbol_type = dim.ArcSymbolType;
            center_point = dim.CenterPoint;
            has_leader = dim.HasLeader;
            leader1_point = dim.Leader1Point;
            leader2_point = dim.Leader2Point;
            x_line1_point = dim.XLine1Point;
            x_line2_point = dim.XLine2Point;

            return base.FromArx(obj, db);
        }

        private static AcDb.ArcDimension New(AcDb.Database db)
        {
            var dim = new AcDb.ArcDimension(
                Vector3d.Origin.ToPoint3d(),
                Vector3d.Origin.ToPoint3d(),
                Vector3d.Origin.ToPoint3d(),
                Vector3d.Origin.ToPoint3d(),
                null,
                AcDb.ObjectId.Null
            );

            dim.SetDatabaseDefaults(db);

            return dim;
        }
    }

    [ArxEntity(typeof(AcDb.DiametricDimension))]
    [PyType("sacad.acdb.DiametricDimension")]
    public sealed class DiametricDimension : Dimension
    {
        public Vector3d chord_point;
        public Vector3d far_chord_point;
        public double? leader_length;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.DiametricDimension>(db);
            var dim = (AcDb.DiametricDimension)obj;

            if (chord_point != null) dim.ChordPoint = chord_point.ToPoint3d();
            if (far_chord_point != null)
                dim.FarChordPoint = far_chord_point.ToPoint3d();
            if (leader_length.HasValue) dim.LeaderLength = leader_length.Value;

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.DiametricDimension)obj;

            chord_point = dim.ChordPoint;
            far_chord_point = dim.FarChordPoint;
            leader_length = dim.LeaderLength;

            return base.FromArx(obj, db);
        }
    }

    [ArxEntity(typeof(AcDb.LineAngularDimension2))]
    [PyType("sacad.acdb.LineAngularDimension2")]
    public sealed class LineAngularDimension2 : Dimension
    {
        public Vector3d arc_point;
        public Vector3d x_line1_end;
        public Vector3d x_line1_start;
        public Vector3d x_line2_end;
        public Vector3d x_line2_start;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.LineAngularDimension2>(db);
            var dim = (AcDb.LineAngularDimension2)obj;

            if (arc_point != null) dim.ArcPoint = arc_point.ToPoint3d();
            if (x_line1_end != null) dim.XLine1End = x_line1_end.ToPoint3d();
            if (x_line1_start != null)
                dim.XLine1Start = x_line1_start.ToPoint3d();
            if (x_line2_end != null) dim.XLine2End = x_line2_end.ToPoint3d();
            if (x_line2_start != null)
                dim.XLine2Start = x_line2_start.ToPoint3d();

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.LineAngularDimension2)obj;

            arc_point = dim.ArcPoint;
            x_line1_end = dim.XLine1End;
            x_line1_start = dim.XLine1Start;
            x_line2_end = dim.XLine2End;
            x_line2_start = dim.XLine2Start;

            return base.FromArx(obj, db);
        }
    }

    [ArxEntity(typeof(AcDb.Point3AngularDimension))]
    [PyType("sacad.acdb.Point3AngularDimension")]
    public sealed class Point3AngularDimension : Dimension
    {
        public Vector3d arc_point;
        public Vector3d center_point;
        public Vector3d x_line1_point;
        public Vector3d x_line2_point;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.Point3AngularDimension>(db);
            var dim = (AcDb.Point3AngularDimension)obj;

            if (arc_point != null) dim.ArcPoint = arc_point.ToPoint3d();
            if (center_point != null)
                dim.CenterPoint = center_point.ToPoint3d();
            if (x_line1_point != null)
                dim.XLine1Point = x_line1_point.ToPoint3d();
            if (x_line2_point != null)
                dim.XLine2Point = x_line2_point.ToPoint3d();

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.Point3AngularDimension)obj;

            arc_point = dim.ArcPoint;
            center_point = dim.CenterPoint;
            x_line1_point = dim.XLine1Point;
            x_line2_point = dim.XLine2Point;

            return base.FromArx(obj, db);
        }
    }

    [ArxEntity(typeof(AcDb.RadialDimension))]
    [PyType("sacad.acdb.RadialDimension")]
    public sealed class RadialDimension : Dimension
    {
        public Vector3d center;
        public Vector3d chord_point;
        public double? leader_length;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.RadialDimension>(db);
            var dim = (AcDb.RadialDimension)obj;

            if (center != null) dim.Center = center.ToPoint3d();
            if (chord_point != null) dim.ChordPoint = chord_point.ToPoint3d();
            if (leader_length.HasValue) dim.LeaderLength = leader_length.Value;

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.RadialDimension)obj;

            center = dim.Center;
            chord_point = dim.ChordPoint;
            leader_length = dim.LeaderLength;

            return base.FromArx(obj, db);
        }
    }

    [ArxEntity(typeof(AcDb.RadialDimensionLarge))]
    [PyType("sacad.acdb.RadialDimensionLarge")]
    public sealed class RadialDimensionLarge : Dimension
    {
        public Vector3d center;
        public Vector3d chord_point;
        public double? jog_angle;
        public Vector3d jog_point;
        public Vector3d override_center;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.RadialDimensionLarge>(db);
            var dim = (AcDb.RadialDimensionLarge)obj;

            if (center != null) dim.Center = center.ToPoint3d();
            if (chord_point != null) dim.ChordPoint = chord_point.ToPoint3d();
            if (jog_angle.HasValue) dim.JogAngle = jog_angle.Value;
            if (jog_point != null) dim.JogPoint = jog_point.ToPoint3d();
            if (override_center != null)
                dim.OverrideCenter = override_center.ToPoint3d();

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.RadialDimensionLarge)obj;

            center = dim.Center;
            chord_point = dim.ChordPoint;
            jog_angle = dim.JogAngle;
            jog_point = dim.JogPoint;
            override_center = dim.OverrideCenter;

            return base.FromArx(obj, db);
        }
    }

    [ArxEntity(typeof(AcDb.RotatedDimension))]
    [PyType("sacad.acdb.RotatedDimension")]
    public sealed class RotatedDimension : Dimension
    {
        public Vector3d dim_line_point;
        public double? oblique;
        public double? rotation;
        public Vector3d x_line1_point;
        public Vector3d x_line2_point;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? New<AcDb.RotatedDimension>(db);
            var dim = (AcDb.RotatedDimension)obj;

            if (dim_line_point != null)
                dim.DimLinePoint = dim_line_point.ToPoint3d();
            if (oblique.HasValue) dim.Oblique = oblique.Value;
            if (rotation.HasValue) dim.Rotation = rotation.Value;
            if (x_line1_point != null)
                dim.XLine1Point = x_line1_point.ToPoint3d();
            if (x_line2_point != null)
                dim.XLine2Point = x_line2_point.ToPoint3d();

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.RotatedDimension)obj;

            dim_line_point = dim.DimLinePoint;
            oblique = dim.Oblique;
            rotation = dim.Rotation;
            x_line1_point = dim.XLine1Point;
            x_line2_point = dim.XLine2Point;

            return base.FromArx(obj, db);
        }
    }
}
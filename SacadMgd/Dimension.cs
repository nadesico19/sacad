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

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType(Name = "sacad.acdb.Dimension")]
    public class Dimension : Entity
    {
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

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var dim = (AcDb.Dimension)obj;

            // TODO

            return base.FromArx(obj, db);
        }
    }

    [PyType(Name = "sacad.acdb.AlignedDimension")]
    public class AlignedDimension : Dimension
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
}
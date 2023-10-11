using System;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    public class DictionaryItem : DBObject
    {
        public string name;

        public virtual AcDb.DBObject GetFromDict(AcDb.Database db)
        {
            throw new NotImplementedException();
        }

        public virtual AcDb.ObjectId AddToDict(AcDb.DBObject obj,
            AcDb.Database db)
        {
            throw new NotImplementedException();
        }
    }

    [PyType("sacad.acdb.MLeaderStyle")]
    public sealed class MLeaderStyle : DictionaryItem
    {
        public double? arrow_size;
        public double? break_size;
        public AcDb.ContentType? content_type;
        public double? dogleg_length;
        public double? landing_gap;
        public AcDb.TextAngleType? text_angle_type;
        public AcDb.TextAlignmentType? text_alignment_type;
        public AcDb.TextAttachmentType? text_attachment_type;
        public string text_style_name;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? new AcDb.MLeaderStyle();
            var style = (AcDb.MLeaderStyle)obj;

            if (arrow_size.HasValue) style.ArrowSize = arrow_size.Value;
            if (break_size.HasValue) style.BreakSize = break_size.Value;
            if (content_type.HasValue) style.ContentType = content_type.Value;
            if (dogleg_length.HasValue)
                style.DoglegLength = dogleg_length.Value;
            if (landing_gap.HasValue) style.LandingGap = landing_gap.Value;
            if (text_angle_type.HasValue)
                style.TextAngleType = text_angle_type.Value;
            if (text_alignment_type.HasValue)
                style.TextAlignmentType = text_alignment_type.Value;
            if (text_attachment_type.HasValue)
                style.TextAttachmentType = text_attachment_type.Value;

            if (text_style_name != null)
            {
                var textStyle = db.GetTextStyle(text_style_name);
                if (textStyle != null) style.TextStyleId = textStyle.ObjectId;
            }

            return base.ToArx(obj, db);
        }

        public override DBObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var style = (AcDb.MLeaderStyle)obj;
            var trans = db.TransactionManager.TopTransaction;

            arrow_size = style.ArrowSize;
            break_size = style.BreakSize;
            content_type = style.ContentType;
            dogleg_length = style.DoglegLength;
            landing_gap = style.LandingGap;
            name = style.Name;
            text_angle_type = style.TextAngleType;
            text_alignment_type = style.TextAlignmentType;
            text_attachment_type = style.TextAttachmentType;

            if (style.TextStyleId.IsValid)
            {
                var symbol = (AcDb.TextStyleTableRecord)trans.GetObject(
                    style.TextStyleId, AcDb.OpenMode.ForRead);
                text_style_name = symbol.Name;
            }

            return base.FromArx(obj, db);
        }

        public override AcDb.DBObject GetFromDict(AcDb.Database db)
            => db.GetMLeaderStyle(name);

        public override AcDb.ObjectId AddToDict(AcDb.DBObject obj,
            AcDb.Database db)
            => db.AddMLeaderStyle((AcDb.MLeaderStyle)obj, name);
    }
}
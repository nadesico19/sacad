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
using System.Reflection;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

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
                    PyWrapper<Extents3d>.Create(entity.GeometricExtents);
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
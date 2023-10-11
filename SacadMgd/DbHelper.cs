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

using System.Diagnostics.CodeAnalysis;
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

namespace SacadMgd
{
    public static class DbHelper
    {
        public static AcDb.TextStyleTableRecord GetTextStyle(
            this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.TextStyleTable)trans.GetObject(
                db.TextStyleTableId, AcDb.OpenMode.ForRead);
            if (table.Has(name))
            {
                return (AcDb.TextStyleTableRecord)trans.GetObject(
                    table[name], AcDb.OpenMode.ForRead);
            }

            return null;
        }

        public static AcDb.TextStyleTableRecord GetShapeStyle(
            this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.TextStyleTable)trans.GetObject(
                db.TextStyleTableId, AcDb.OpenMode.ForRead);

            foreach (var id in table)
            {
                if (!id.IsValid) continue;

                var record = (AcDb.TextStyleTableRecord)trans.GetObject(id,
                    AcDb.OpenMode.ForRead);
                if (!record.IsShapeFile) continue;

                if (record.FileName == name) return record;
            }

            return null;
        }

        public static AcDb.DimStyleTableRecord GetDimStyle(
            this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.DimStyleTable)trans.GetObject(db.DimStyleTableId,
                AcDb.OpenMode.ForRead);
            if (table.Has(name))
            {
                return (AcDb.DimStyleTableRecord)trans.GetObject(table[name],
                    AcDb.OpenMode.ForRead);
            }

            return null;
        }

        public static AcDb.LinetypeTableRecord GetLinetype(
            this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.LinetypeTable)trans.GetObject(db.LinetypeTableId,
                AcDb.OpenMode.ForRead);
            if (table.Has(name))
            {
                return (AcDb.LinetypeTableRecord)trans.GetObject(table[name],
                    AcDb.OpenMode.ForRead);
            }

            return null;
        }

        public static AcDb.BlockTableRecord GetBlock(this AcDb.Database db,
            string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.BlockTable)trans.GetObject(db.BlockTableId,
                AcDb.OpenMode.ForRead);
            if (table.Has(name))
            {
                return (AcDb.BlockTableRecord)trans.GetObject(table[name],
                    AcDb.OpenMode.ForRead);
            }

            return null;
        }

        public static AcDb.LayerTableRecord GetLayer(this AcDb.Database db,
            string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.LayerTable)trans.GetObject(db.LayerTableId,
                AcDb.OpenMode.ForRead);
            if (table.Has(name))
            {
                return (AcDb.LayerTableRecord)trans.GetObject(table[name],
                    AcDb.OpenMode.ForRead);
            }

            return null;
        }

        public static AcDb.MLeaderStyle GetMLeaderStyle(this AcDb.Database db,
            string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var dict = (AcDb.DBDictionary)trans.GetObject(
                db.MLeaderStyleDictionaryId, AcDb.OpenMode.ForRead);
            if (dict.Contains(name))
            {
                return (AcDb.MLeaderStyle)trans.GetObject(dict.GetAt(name),
                    AcDb.OpenMode.ForRead);
            }

            return null;
        }

        [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
        public static AcDb.ObjectId GetDimblk(this AcDb.Database db,
            string dimBlkSysVar, string arrowName)
        {
            var block = db.GetBlock(arrowName);
            if (block != null) return block.ObjectId;

            var oldArrowName = AcAp.Application.GetSystemVariable(dimBlkSysVar);
            try
            {
                AcAp.Application.SetSystemVariable(dimBlkSysVar, arrowName);
            }
            finally
            {
                if (!string.IsNullOrEmpty(oldArrowName as string))
                {
                    AcAp.Application.SetSystemVariable(dimBlkSysVar,
                        oldArrowName);
                }
            }

            block = db.GetBlock(arrowName);
            return block != null ? block.ObjectId : AcDb.ObjectId.Null;
        }

        public static AcDb.ObjectId AddTextStyle(this AcDb.Database db,
            AcDb.TextStyleTableRecord textStyle)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.TextStyleTable)trans.GetObject(
                db.TextStyleTableId, AcDb.OpenMode.ForWrite);
            return table.Add(textStyle);
        }

        public static AcDb.ObjectId AddLineype(this AcDb.Database db,
            AcDb.LinetypeTableRecord linetype)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.LinetypeTable)trans.GetObject(db.LinetypeTableId,
                AcDb.OpenMode.ForWrite);
            return table.Add(linetype);
        }

        public static AcDb.ObjectId AddLayer(this AcDb.Database db,
            AcDb.LayerTableRecord layer)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.LayerTable)trans.GetObject(db.LayerTableId,
                AcDb.OpenMode.ForWrite);
            return table.Add(layer);
        }

        public static AcDb.ObjectId AddDimStyle(this AcDb.Database db,
            AcDb.DimStyleTableRecord dimStyle)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.DimStyleTable)trans.GetObject(db.DimStyleTableId,
                AcDb.OpenMode.ForWrite);
            return table.Add(dimStyle);
        }

        public static AcDb.ObjectId AddMLeaderStyle(this AcDb.Database db,
            AcDb.MLeaderStyle mLeaderStyle, string name)
            => mLeaderStyle.PostMLeaderStyleToDb(db, name);

        public static AcDb.ObjectId AddBlock(this AcDb.Database db,
            AcDb.BlockTableRecord block)
        {
            var trans = db.TransactionManager.TopTransaction;
            var table = (AcDb.BlockTable)trans.GetObject(db.BlockTableId,
                AcDb.OpenMode.ForWrite);
            return table.Add(block);
        }

        public static AcDb.ObjectId AddToModelSpace(this AcDb.Database db,
            AcDb.Entity entity)
        {
            var modelSpace = db.GetBlock(AcDb.BlockTableRecord.ModelSpace);

            try
            {
                modelSpace.UpgradeOpen();
                return modelSpace.AppendEntity(entity);
            }
            finally
            {
                modelSpace.DowngradeOpen();
            }
        }
    }
}
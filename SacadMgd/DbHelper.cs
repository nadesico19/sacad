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

using System.Diagnostics;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

namespace SacadMgd
{
    public static class DbHelper
    {
        public static AcDb.TextStyleTableRecord GetTextStyle(this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.TextStyleTable)trans.GetObject(db.TextStyleTableId,
                AcDb.OpenMode.ForRead);
            if (tbl.Has(name))
                return (AcDb.TextStyleTableRecord)trans.GetObject(tbl[name], AcDb.OpenMode.ForRead);

            return null;
        }

        public static AcDb.DimStyleTableRecord GetDimStyle(this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.DimStyleTable)trans.GetObject(db.DimStyleTableId,
                AcDb.OpenMode.ForRead);
            if (tbl.Has(name))
                return (AcDb.DimStyleTableRecord)trans.GetObject(tbl[name], AcDb.OpenMode.ForRead);

            return null;
        }

        public static AcDb.LinetypeTableRecord GetLinetype(this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.LinetypeTable)trans.GetObject(db.LinetypeTableId,
                AcDb.OpenMode.ForRead);
            if (tbl.Has(name))
                return (AcDb.LinetypeTableRecord)trans.GetObject(tbl[name], AcDb.OpenMode.ForRead);

            return null;
        }

        public static AcDb.BlockTableRecord GetBlock(this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.BlockTable)trans.GetObject(db.BlockTableId, AcDb.OpenMode.ForRead);
            if (tbl.Has(name))
                return (AcDb.BlockTableRecord)trans.GetObject(tbl[name], AcDb.OpenMode.ForRead);

            return null;
        }

        public static AcDb.LayerTableRecord GetLayer(this AcDb.Database db, string name)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.LayerTable)trans.GetObject(db.LayerTableId, AcDb.OpenMode.ForRead);
            if (tbl.Has(name))
                return (AcDb.LayerTableRecord)trans.GetObject(tbl[name], AcDb.OpenMode.ForRead);

            return null;
        }

        public static AcDb.ObjectId AddTextStyle(this AcDb.Database db,
            AcDb.TextStyleTableRecord textStyle)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.TextStyleTable)trans.GetObject(db.TextStyleTableId,
                AcDb.OpenMode.ForWrite);
            return tbl.Add(textStyle);
        }

        public static AcDb.ObjectId AddLineype(this AcDb.Database db,
            AcDb.LinetypeTableRecord linetype)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.LinetypeTable)trans.GetObject(db.LinetypeTableId,
                AcDb.OpenMode.ForWrite);
            return tbl.Add(linetype);
        }

        public static AcDb.ObjectId AddLayer(this AcDb.Database db, AcDb.LayerTableRecord layer)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.LayerTable)trans.GetObject(db.LayerTableId, AcDb.OpenMode.ForWrite);
            return tbl.Add(layer);
        }

        public static AcDb.ObjectId AddDimStyle(this AcDb.Database db,
            AcDb.DimStyleTableRecord dimStyle)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.DimStyleTable)trans.GetObject(db.DimStyleTableId,
                AcDb.OpenMode.ForWrite);
            return tbl.Add(dimStyle);
        }

        public static AcDb.ObjectId AddBlock(this AcDb.Database db, AcDb.BlockTableRecord block)
        {
            var trans = db.TransactionManager.TopTransaction;
            var tbl = (AcDb.BlockTable)trans.GetObject(db.BlockTableId, AcDb.OpenMode.ForWrite);
            return tbl.Add(block);
        }

        public static AcDb.ObjectId AddToModelSpace(this AcDb.Database db, AcDb.Entity entity)
        {
            var mdlSpc = db.GetBlock(AcDb.BlockTableRecord.ModelSpace);
            Debug.Assert(mdlSpc != null, nameof(mdlSpc) + " != null");

            try
            {
                mdlSpc.UpgradeOpen();
                return mdlSpc.AppendEntity(entity);
            }
            finally
            {
                mdlSpc.DowngradeOpen();
            }
        }
    }
}
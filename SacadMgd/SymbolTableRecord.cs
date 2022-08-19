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
    public class SymbolTableRecordWrapper : PyWrapper<SymbolTableRecord>
    {
    }

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

        public virtual AcDb.SymbolTableRecord GetArxFromSymbolTable(AcDb.Database db)
        {
            throw new NotImplementedException();
        }

        public virtual AcDb.ObjectId AddArxToSymbolTable(AcDb.SymbolTableRecord symbol,
            AcDb.Database db)
        {
            throw new NotImplementedException();
        }
    }

    [PyType(Name = "sacad.acdb.BlockTableRecord")]
    public class BlockTableRecord : SymbolTableRecord
    {
        public List<EntityWrapper> entities;
    }

    [PyType(Name = "sacad.acdb.LayerTableRecord")]
    public class LayerTableRecord : SymbolTableRecord
    {
        public ColorWrapper color;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            obj = obj ?? new AcDb.LayerTableRecord();
            var symbol = (AcDb.LayerTableRecord)obj;

            if (color != null) symbol.Color = color.__mbr__.ToArx();

            return base.ToArx(obj, db);
        }

        public override AcDb.SymbolTableRecord GetArxFromSymbolTable(AcDb.Database db) =>
            db.GetLayer(name);

        public override AcDb.ObjectId AddArxToSymbolTable(AcDb.SymbolTableRecord symbol,
            AcDb.Database db) => db.AddLayer((AcDb.LayerTableRecord)symbol);
    }
}
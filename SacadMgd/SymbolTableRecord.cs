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

        public virtual AcDb.SymbolTableRecord GetFromSymbolTable(AcDb.Database db)
        {
            throw new NotImplementedException();
        }

        public virtual AcDb.ObjectId AddToSymbolTable(AcDb.SymbolTableRecord symbol,
            AcDb.Database db)
        {
            throw new NotImplementedException();
        }
    }

    [PyType(Name = "sacad.acdb.BlockTableRecord")]
    public class BlockTableRecord : SymbolTableRecord
    {
        public List<PyWrapper<Entity>> entities;
    }

    [PyType(Name = "sacad.acdb.LayerTableRecord")]
    public class LayerTableRecord : SymbolTableRecord
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
            var symbol = (AcDb.LayerTableRecord)obj;

            if (color != null) symbol.Color = color.__mbr__.ToArx();
            if (is_frozen.HasValue) symbol.IsFrozen = is_frozen.Value;
            if (is_locked.HasValue) symbol.IsLocked = is_locked.Value;
            if (is_off.HasValue) symbol.IsOff = is_off.Value;
            if (is_plottable.HasValue) symbol.IsPlottable = is_plottable.Value;
            if (line_weight.HasValue) symbol.LineWeight = (AcDb.LineWeight)line_weight.Value;

            // TODO linetype

            return base.ToArx(obj, db);
        }

        public override AcDb.SymbolTableRecord GetFromSymbolTable(AcDb.Database db) =>
            db.GetLayer(name);

        public override AcDb.ObjectId AddToSymbolTable(AcDb.SymbolTableRecord symbol,
            AcDb.Database db) => db.AddLayer((AcDb.LayerTableRecord)symbol);
    }
}
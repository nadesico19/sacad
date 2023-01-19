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

using System.Collections.Generic;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType(Name = "sacad.acdb.Database")]
    public class Database : PyObject
    {
        public Dictionary<string, PyWrapper<SymbolTableRecord>> blocktable;
        public Dictionary<string, PyWrapper<SymbolTableRecord>> layertable;
    }

    [PyType(Name = "sacad.acdb.DBObject")]
    public class DbObject : PyObject
    {
        public long id;

        public virtual AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db) => obj;

        public virtual DbObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            id = obj.Id.OldIdPtr.ToInt64();
            return this;
        }
    }

    [PyType(Name = "sacad.acdb.Entity")]
    public class Entity : DbObject
    {
        public string Layer;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var ent = (AcDb.Entity)obj;

            if (!string.IsNullOrWhiteSpace(Layer)) ent.Layer = Layer;

            return base.ToArx(obj, db);
        }

        public override DbObject FromArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var entity = (AcDb.Entity)obj;

            Layer = entity.Layer;

            return base.FromArx(obj, db);
        }
    }
}
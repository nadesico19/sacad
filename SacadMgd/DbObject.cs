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
    public class DbObjectWrapper : PyWrapper<DbObject>
    {
    }

    [PyType(Name = "sacad.acdb.DBObject")]
    public class DbObject : PyObject
    {
        public int? id;

        public virtual AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            return obj;
        }
    }

    public class EntityWrapper : PyWrapper<Entity>
    {
    }

    [PyType(Name = "sacad.acdb.Entity")]
    public class Entity : DbObject
    {
        public string Layer;

        public override AcDb.DBObject ToArx(AcDb.DBObject obj, AcDb.Database db)
        {
            var ent = obj as AcDb.Entity;
            if (ent != null)
            {
                if (!string.IsNullOrWhiteSpace(Layer)) ent.Layer = Layer;
            }

            return base.ToArx(obj, db);
        }
    }
}
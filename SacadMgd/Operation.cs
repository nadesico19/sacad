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
using System.Diagnostics;
using AcDb = Autodesk.AutoCAD.DatabaseServices;

namespace SacadMgd
{
    public static class Operation
    {
        public static Result DoDb(string message)
        {
            var dbQuery = Util.Deserialize<DbQueryWrapper>(message);

            switch (dbQuery.__mbr__.query_type)
            {
                case "DB_INSERT":
                    return DoDbInsert((DbInsertQuery)dbQuery.__mbr__);
            }

            throw new NotImplementedException();
        }

        public static Result DoDoc(string message)
        {
            throw new NotImplementedException();
        }

        public static Result DoSession(string message)
        {
            throw new NotImplementedException();
        }

        private static Result DoDbInsert(DbInsertQuery dbQuery)
        {
            var result = new DbInsertResult();

            var db = AcDb.HostApplicationServices.WorkingDatabase;
            try
            {
                Database clientDb = dbQuery?.database?.__mbr__;

                using (var trans = db.TransactionManager.StartTransaction())
                {
                    InsertSymbol(db, clientDb?.layertable, dbQuery?.upsert == true,
                        ref result.num_inserted, ref result.num_inserted);

                    if (clientDb?.blocktable?.ContainsKey(AcDb.BlockTableRecord.ModelSpace) == true)
                    {
                        var modelSpace = (BlockTableRecord)clientDb
                            .blocktable[AcDb.BlockTableRecord.ModelSpace].__mbr__;
                        foreach (var entity in modelSpace.entities)
                        {
                            var newEntity = (AcDb.Entity)entity.__mbr__.ToArx(null, db);
                            db.AddToModelSpace(newEntity);
                            trans.AddNewlyCreatedDBObject(newEntity, true);

                            result.num_inserted++;
                        }
                    }

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                Util.ConsoleWriteLine(ex);
                result.message = $"Unhandled exception: {ex.Message}";
            }

            return result;
        }

        private static void InsertSymbol(AcDb.Database db,
            Dictionary<string, SymbolTableRecordWrapper> symTbl, bool upsert, ref int inserted,
            ref int updated)
        {
            if (symTbl == null)
            {
                return;
            }

            var trans = db.TransactionManager.TopTransaction;
            Debug.Assert(trans != null, nameof(trans) + " != null");

            foreach (var symbol in symTbl.Values)
            {
                AcDb.SymbolTableRecord oldSymbol = symbol.__mbr__.GetArxFromSymbolTable(db);
                if (oldSymbol != null)
                {
                    if (upsert != true) continue;

                    oldSymbol.UpgradeOpen();
                    symbol.__mbr__.ToArx(oldSymbol, db);
                    oldSymbol.DowngradeOpen();

                    updated++;
                }
                else
                {
                    var newSymbol = (AcDb.SymbolTableRecord)symbol.__mbr__.ToArx(null, db);
                    symbol.__mbr__.AddArxToSymbolTable(newSymbol, db);
                    trans.AddNewlyCreatedDBObject(newSymbol, true);

                    inserted++;
                }
            }
        }
    }
}
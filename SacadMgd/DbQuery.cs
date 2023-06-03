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
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcEi = Autodesk.AutoCAD.EditorInput;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    public abstract class DbQuery
    {
        public string query_type;
        public PyWrapper<Database> database;

        public abstract Result Execute();
    }

    public enum ZoomMode
    {
        None,
        Added,
        All,
    }

    public sealed class DbInsertQuery : DbQuery
    {
        public Vector3d insertion_point;
        public bool? prompt_insertion_point;
        public bool? upsert;
        public ZoomMode? zoom_mode;
        public double? zoom_factor;

        public override Result Execute()
        {
            var result = new DbInsertResult();

            var db = AcDb.HostApplicationServices.WorkingDatabase;
            try
            {
                var clientDb = database?.__mbr__;

                AcDb.Extents3d extents;
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    InsertSymbol(db, clientDb?.text_style_table, result);
                    InsertSymbol(db, clientDb?.linetype_table, result);
                    InsertSymbol(db, clientDb?.layer_table, result);
                    InsertSymbol(db, clientDb?.dim_style_table, result);

                    if (prompt_insertion_point == true)
                        PromptInsertionPoint(db);

                    extents = InsertModelSpace(clientDb, db, trans, result);

                    trans.Commit();
                }

                if (result.num_inserted + result.num_updated > 0)
                {
                    result.status = result.num_failure > 0
                        ? Status.Warning
                        : Status.Success;
                }
                else
                {
                    result.status = result.num_failure > 0
                        ? Status.Failure
                        : Status.Success;
                }

                if (result.status != Status.Failure)
                {
                    if (zoom_mode.HasValue && zoom_mode != ZoomMode.None)
                    {
                        if (zoom_mode == ZoomMode.All)
                        {
                            db.UpdateExt(false);
                            extents = new AcDb.Extents3d(db.Extmin, db.Extmax);
                        }

                        ZoomExtents(db, extents);
                    }
                }
            }
            catch (Exception ex)
            {
                Util.ConsoleWriteLine(ex);
                result.message = $"Unhandled exception: {ex.Message}";
            }

            return result;
        }

        private void InsertSymbol<T>(AcDb.Database db,
            Dictionary<string, PyWrapper<T>> symbolTable,
            DbInsertResult result) where T : SymbolTableRecord
        {
            if (symbolTable == null) return;

            var trans = db.TransactionManager.TopTransaction;

            foreach (var symbol in symbolTable.Values)
            {
                try
                {
                    var oldSymbol = symbol.__mbr__.GetFromSymbolTable(db);
                    if (oldSymbol != null)
                    {
                        if (upsert != true) continue;

                        oldSymbol.UpgradeOpen();
                        symbol.__mbr__.ToArx(oldSymbol, db);
                        oldSymbol.DowngradeOpen();

                        result.num_updated++;
                    }
                    else
                    {
                        var newSymbol = (AcDb.SymbolTableRecord)symbol.__mbr__
                            .ToArx(null, db);
                        symbol.__mbr__.AddToSymbolTable(newSymbol, db);
                        trans.AddNewlyCreatedDBObject(newSymbol, true);

                        result.num_inserted++;
                    }
                }
                catch (Exception ex)
                {
                    Util.ConsoleWriteLine(
                        $"{symbol.__cls__} insertion failed: {ex.Message}");
                    result.num_failure++;
                }
            }
        }

        private void PromptInsertionPoint(AcDb.Database db)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            if (doc.Database != db) return;

            var resPoint = doc.Editor.GetPoint(
                "Please specify the insertion point:");
            if (resPoint.Status != AcEi.PromptStatus.OK)
            {
                throw new OperationCanceledException(
                    "Failed to get the insertion point.");
            }

            insertion_point = resPoint.Value;
        }

        private AcDb.Extents3d InsertModelSpace(Database clientDb,
            AcDb.Database db, AcDb.Transaction trans, DbInsertResult result)
        {
            var extents = new AcDb.Extents3d();
            if (clientDb?.block_table?.ContainsKey(
                    AcDb.BlockTableRecord.ModelSpace) != true)
                return extents;

            var transform = insertion_point != null
                ? AcGe.Matrix3d.Displacement(insertion_point.ToVector3d())
                : (AcGe.Matrix3d?)null;

            var modelSpace = clientDb
                .block_table[AcDb.BlockTableRecord.ModelSpace].__mbr__;
            foreach (var entity in modelSpace.entities)
            {
                try
                {
                    var newEntity = (AcDb.Entity)entity.__mbr__.ToArx(null, db);

                    if (transform.HasValue)
                        newEntity.TransformBy(transform.Value);

                    db.AddToModelSpace(newEntity);
                    trans.AddNewlyCreatedDBObject(newEntity, true);

                    if (zoom_mode == ZoomMode.Added &&
                        newEntity.Bounds.HasValue)
                    {
                        (newEntity as AcDb.Dimension)?.GenerateLayout();
                        extents.AddExtents(newEntity.GeometricExtents);
                    }

                    result.num_inserted++;
                }
                catch (Exception ex)
                {
                    Util.ConsoleWriteLine(
                        $"{entity.__cls__} insertion failed: {ex.Message}");
                    result.num_failure++;
                }
            }

            return extents;
        }

        private void ZoomExtents(AcDb.Database db, AcDb.Extents3d ext)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            if (doc.Database != db) return;

            if (ext.MinPoint.X >= ext.MaxPoint.X ||
                ext.MinPoint.Y >= ext.MaxPoint.Y)
            {
                return;
            }

            var scaleFactor = zoom_factor ?? 1;

            var view = doc.Editor.GetCurrentView();
            view.CenterPoint = new AcGe.Point2d(
                (ext.MinPoint.X + ext.MaxPoint.X) / 2,
                (ext.MinPoint.Y + ext.MaxPoint.Y) / 2);
            view.Width = (ext.MaxPoint.X - ext.MinPoint.X) * scaleFactor;
            view.Height = (ext.MaxPoint.Y - ext.MinPoint.Y) * scaleFactor;

            doc.Editor.SetCurrentView(view);
        }
    }

    public sealed class DbSelectQuery : DbQuery
    {
        public Mode? mode;
        public int? table_flags;

        public override Result Execute()
        {
            var result = new DbSelectResult
            {
                db = PyWrapper<Database>.Create(new Database())
            };

            var db = AcDb.HostApplicationServices.WorkingDatabase;
            try
            {
                using (db.TransactionManager.StartTransaction())
                {
                    switch (mode)
                    {
                        case Mode.GetTables:
                            GetTable(db, result);
                            break;
                        case Mode.GetUserSelection:
                            GetUserSelection(db, result);
                            break;
                        case Mode.TestEntities:
                            TestEntities(db, result);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (result.status == Status.Unknown)
                        result.status = Status.Success;
                }
            }
            catch (Exception ex)
            {
                Util.ConsoleWriteLine(ex);
                result.message = $"Unhandled exception: {ex.Message}";
            }

            return result;
        }

        private void GetTable(AcDb.Database db, DbSelectResult result)
        {
            result.db = result.db ?? PyWrapper<Database>.Create(new Database());

            if ((table_flags & (int)TableFlags.ModelSpace) != 0)
            {
                result.db.__mbr__.block_table =
                    result.db.__mbr__.block_table ??
                    new Dictionary<string, PyWrapper<BlockTableRecord>>();
            }

            if ((table_flags & (int)TableFlags.TextStyle) != 0)
            {
                result.db.__mbr__.text_style_table =
                    result.db.__mbr__.text_style_table ??
                    new Dictionary<string, PyWrapper<TextStyleTableRecord>>();

                SelectSymbols(db, db.TextStyleTableId,
                    result.db.__mbr__.text_style_table);
            }

            if ((table_flags & (int)TableFlags.Linetype) != 0)
            {
                result.db.__mbr__.linetype_table =
                    result.db.__mbr__.linetype_table ??
                    new Dictionary<string, PyWrapper<LinetypeTableRecord>>();

                SelectSymbols(db, db.LinetypeTableId,
                    result.db.__mbr__.linetype_table);
            }

            if ((table_flags & (int)TableFlags.Layer) != 0)
            {
                result.db.__mbr__.layer_table =
                    result.db.__mbr__.layer_table ??
                    new Dictionary<string, PyWrapper<LayerTableRecord>>();

                SelectSymbols(db, db.LayerTableId,
                    result.db.__mbr__.layer_table);
            }

            if ((table_flags & (int)TableFlags.DimStyle) != 0)
            {
                result.db.__mbr__.dim_style_table =
                    result.db.__mbr__.dim_style_table ??
                    new Dictionary<string, PyWrapper<DimStyleTableRecord>>();

                SelectSymbols(db, db.DimStyleTableId,
                    result.db.__mbr__.dim_style_table);
            }
        }

        private void TestEntities(AcDb.Database db, DbSelectResult result)
        {
            // TODO
        }

        private void GetUserSelection(AcDb.Database db, DbSelectResult result)
        {
            // TODO
        }

        private static void SelectSymbols<TRecord>(
            AcDb.Database db, AcDb.ObjectId tableId,
            IDictionary<string, PyWrapper<TRecord>> table)
            where TRecord : SymbolTableRecord, new()
        {
            var trans = db.TransactionManager.TopTransaction;

            var arxTable = (AcDb.SymbolTable)trans.GetObject(tableId,
                AcDb.OpenMode.ForRead);
            foreach (var id in arxTable)
            {
                if (!id.IsValid) continue;

                var arxSym = (AcDb.SymbolTableRecord)trans.GetObject(id,
                    AcDb.OpenMode.ForRead);

                var sym = new TRecord();
                sym.FromArx(arxSym, db);

                table[arxSym.Name] = PyWrapper<TRecord>.Create(sym);
            }
        }

        public enum Mode
        {
            GetTables,
            GetUserSelection,
            TestEntities,
        }

        private enum TableFlags
        {
            ModelSpace = 0x01,
            TextStyle = 0x02,
            Linetype = 0x04,
            Layer = 0x08,
            DimStyle = 0x10,
        }
    }
}
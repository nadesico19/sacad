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
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.EditorInput;
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
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
                    // TODO TextStyles
                    InsertSymbol(db, clientDb?.linetypetable, result);
                    InsertSymbol(db, clientDb?.layertable, result);
                    // TODO DimStyles

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
            Dictionary<string, PyWrapper<T>> symbolTable, DbInsertResult result)
            where T : SymbolTableRecord
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
                        if (upsert != null) continue;

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
                    WriteLog(db,
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

            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var hWnd = AcAp.Application.MainWindow.Handle;
            Util.SetForegroundWindow(hWnd);

            var resPoint = doc.Editor.GetPoint(
                "Please specify the insertion point:");
            if (resPoint.Status != PromptStatus.OK)
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
            if (clientDb?.blocktable?.ContainsKey(
                    AcDb.BlockTableRecord.ModelSpace) != true)
                return extents;

            var transform = insertion_point != null
                ? AcGe.Matrix3d.Displacement(insertion_point.ToVector3d())
                : (AcGe.Matrix3d?)null;

            var modelSpace = clientDb
                .blocktable[AcDb.BlockTableRecord.ModelSpace].__mbr__;
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
                    WriteLog(db,
                        $"{entity.__cls__} insertion failed: {ex.Message}");
                    result.num_failure++;
                }
            }

            return extents;
        }

        private void ZoomExtents(AcDb.Database db, AcDb.Extents3d ext)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
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

        private void WriteLog(AcDb.Database db, string message)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var doc = AcAp.Application.DocumentManager.MdiActiveDocument;
            if (doc.Database != db) return;

            doc.Editor.WriteMessage($"\n{message}");
        }
    }
}
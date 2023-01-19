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
using Autodesk.AutoCAD.ApplicationServices.Core;
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    public abstract class DbQuery : PyObject
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

    public class DbInsertQuery : DbQuery
    {
        public bool upsert;
        public ZoomMode zoom_mode;
        public double zoom_scale;

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
                    InsertSymbol(db, clientDb?.layertable, upsert, ref result.num_inserted,
                        ref result.num_updated);

                    extents = InsertModelSpace(clientDb, db, trans, result,
                        zoom_mode == ZoomMode.Added);

                    trans.Commit();
                }

                if (result.status != Status.Failure)
                {
                    result.status = Status.Success;

                    if (zoom_mode != ZoomMode.None)
                    {
                        if (zoom_mode == ZoomMode.All)
                        {
                            db.UpdateExt(false);
                            extents = new AcDb.Extents3d(db.Extmin, db.Extmax);
                        }

                        ZoomExtents(db, extents, zoom_scale);
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

        private static void InsertSymbol(AcDb.Database db,
            Dictionary<string, PyWrapper<SymbolTableRecord>> symTbl,
            bool upsert, ref int inserted, ref int updated)
        {
            if (symTbl == null) return;

            var trans = db.TransactionManager.TopTransaction;
            Debug.Assert(trans != null, nameof(trans) + " != null");

            foreach (var symbol in symTbl.Values)
            {
                var oldSymbol = symbol.__mbr__.GetFromSymbolTable(db);
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
                    symbol.__mbr__.AddToSymbolTable(newSymbol, db);
                    trans.AddNewlyCreatedDBObject(newSymbol, true);

                    inserted++;
                }
            }
        }

        private static AcDb.Extents3d InsertModelSpace(Database clientDb, AcDb.Database db,
            AcDb.Transaction trans, DbInsertResult result, bool genExtents)
        {
            var extents = new AcDb.Extents3d();

            if (clientDb?.blocktable?.ContainsKey(AcDb.BlockTableRecord.ModelSpace) != true)
                return extents;

            var modelSpace = (BlockTableRecord)clientDb
                .blocktable[AcDb.BlockTableRecord.ModelSpace].__mbr__;
            foreach (var entity in modelSpace.entities)
            {
                var newEntity = (AcDb.Entity)entity.__mbr__.ToArx(null, db);
                db.AddToModelSpace(newEntity);
                trans.AddNewlyCreatedDBObject(newEntity, true);

                if (genExtents && newEntity.Bounds.HasValue)
                {
                    (newEntity as AcDb.Dimension)?.GenerateLayout();
                    extents.AddExtents(newEntity.GeometricExtents);
                }

                result.num_inserted++;
            }

            return extents;
        }

        private static void ZoomExtents(AcDb.Database db, AcDb.Extents3d ext, double scale)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc.Database != db) return;

            if (ext.MinPoint.X >= ext.MaxPoint.X || ext.MinPoint.Y >= ext.MaxPoint.Y)
                return;

            var view = doc.Editor.GetCurrentView();
            view.CenterPoint = new AcGe.Point2d(
                (ext.MinPoint.X + ext.MaxPoint.X) / 2,
                (ext.MinPoint.Y + ext.MaxPoint.Y) / 2);
            view.Width = (ext.MaxPoint.X - ext.MinPoint.X) * scale;
            view.Height = (ext.MaxPoint.Y - ext.MinPoint.Y) * scale;
            doc.Editor.SetCurrentView(view);
        }
    }
}
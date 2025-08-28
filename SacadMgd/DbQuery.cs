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
using System.Linq;
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcDb = Autodesk.AutoCAD.DatabaseServices;
using AcEi = Autodesk.AutoCAD.EditorInput;
using AcGe = Autodesk.AutoCAD.Geometry;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    using BlockTable = Dictionary<string, PyWrapper<BlockTableRecord>>;
    using GroupDict = Dictionary<string, PyWrapper<Group>>;

    public abstract class DbQuery
    {
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
                    InsertDictItem(db, clientDb?.m_leader_style_dict, result);
                    InsertDictItem(db, clientDb?.group_dict, result);
                    InsertBlocks(db, clientDb?.block_table, trans, result);

                    if (prompt_insertion_point == true)
                    {
                        PromptInsertionPoint(db);
                        result.user_insertion_point = insertion_point;
                    }

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

        private void InsertSymbol<TRecord>(AcDb.Database db,
            Dictionary<string, PyWrapper<TRecord>> symbolTable,
            DbInsertResult result) where TRecord : SymbolTableRecord
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

        private void InsertDictItem<TItem>(AcDb.Database db,
            Dictionary<string, PyWrapper<TItem>> dict,
            DbInsertResult result) where TItem : DictionaryItem
        {
            if (dict == null) return;

            var trans = db.TransactionManager.TopTransaction;

            foreach (var item in dict.Values)
            {
                try
                {
                    var oldSymbol = item.__mbr__.GetFromDict(db);
                    if (oldSymbol != null)
                    {
                        if (upsert != true) continue;

                        oldSymbol.UpgradeOpen();
                        item.__mbr__.ToArx(oldSymbol, db);
                        oldSymbol.DowngradeOpen();

                        result.num_updated++;
                    }
                    else
                    {
                        var newSymbol = item.__mbr__.ToArx(null, db);
                        item.__mbr__.AddToDict(newSymbol, db);
                        trans.AddNewlyCreatedDBObject(newSymbol, true);

                        result.num_inserted++;
                    }
                }
                catch (Exception ex)
                {
                    Util.ConsoleWriteLine(
                        $"{item.__cls__} insertion failed: {ex.Message}");
                    result.num_failure++;
                }
            }
        }

        private void InsertBlocks(AcDb.Database db, BlockTable blockTable,
            AcDb.Transaction trans, DbInsertResult result)
        {
            foreach (var block in blockTable.Values.Where(blk =>
                         blk.__mbr__.name != null
                         && !blk.__mbr__.name.StartsWith("*")
                         && !blk.__mbr__.name.StartsWith("_")))
            {
                try
                {
                    var oldBlock = (AcDb.BlockTableRecord)block.__mbr__
                        .GetFromSymbolTable(db);
                    if (oldBlock != null && upsert != true) continue;

                    // MEMO: Something went wrong when trying to remove and 
                    // re-append all entities inside a block. It caused a block
                    // to be invalid, neither insertable nor editable.
                    // Finally, I implemented `upsert` by re-creation and
                    // replacement of the whole block.
                    var refIds = oldBlock?.GetBlockReferenceIds(false, true);
                    if (oldBlock != null)
                    {
                        oldBlock.UpgradeOpen();
                        oldBlock.Erase();
                    }

                    var newBlock = (AcDb.BlockTableRecord)block.__mbr__
                        .ToArx(null, db);
                    var newBlkId = block.__mbr__.AddToSymbolTable(newBlock, db);
                    trans.AddNewlyCreatedDBObject(newBlock, true);

                    if (refIds?.Count > 0)
                    {
                        foreach (AcDb.ObjectId eid in refIds)
                        {
                            var blkRef = (AcDb.BlockReference)trans.GetObject(
                                eid, AcDb.OpenMode.ForWrite);
                            blkRef.BlockTableRecord = newBlkId;
                            blkRef.DowngradeOpen();
                        }
                    }

                    result.num_inserted++;
                }
                catch (Exception ex)
                {
                    Util.ConsoleWriteLine(
                        $"{block.__cls__} insertion failed: {ex.Message}");
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
            var reversed_group = ReverseGroup(clientDb);
            foreach (var entity in modelSpace.entities)
            {
                try
                {
                    var newEntity = (AcDb.Entity)entity.__mbr__.ToArx(null, db);

                    if (transform.HasValue)
                        newEntity.TransformBy(transform.Value);

                    if (newEntity.ObjectId.IsNull)
                        db.AddToModelSpace(newEntity);
                    trans.AddNewlyCreatedDBObject(newEntity, true);

                    if (zoom_mode == ZoomMode.Added &&
                        newEntity.Bounds.HasValue)
                    {
                        (newEntity as AcDb.Dimension)?.GenerateLayout();
                        extents.AddExtents(newEntity.GeometricExtents);
                    }

                    result.num_inserted++;

                    Group value;
                    if (entity.__mbr__.id.HasValue &&
                        reversed_group.TryGetValue(entity.__mbr__.id.Value,
                            out value))
                    {
                        DoGrouping(newEntity, value, db);
                    }
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

        private static Dictionary<long, Group> ReverseGroup(Database db)
        {
            var reversed = new Dictionary<long, Group>();
            foreach (var entry in db.GetGroupDict())
            {
                var group = entry.Value.__mbr__;
                if (group.entity_ids == null) continue;

                foreach (var eid in group.entity_ids)
                {
                    reversed[eid] = group;
                }
            }

            return reversed;
        }

        private static void DoGrouping(AcDb.Entity entity, Group group,
            AcDb.Database db)
        {
            var arxGroup = db.GetGroup(group.name);
            try
            {
                arxGroup.UpgradeOpen();
                if (!arxGroup.Has(entity))
                    arxGroup.Append(entity.ObjectId);
            }
            finally
            {
                arxGroup.DowngradeOpen();
            }
        }
    }

    public sealed class DbSelectQuery : DbQuery
    {
        public Mode? mode;
        public int? table_flags;
        public List<string> group_names;
        public bool? explode_blocks;
        public List<string> block_names;
        public bool? select_by_prompt;

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
                        case Mode.GetGroups:
                            GetGroups(db, result);
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
            var trans = db.TransactionManager.TopTransaction;
            result.db = result.db ?? PyWrapper<Database>.Create(new Database());

            if ((table_flags & (int)TableFlags.ModelSpace) != 0)
            {
                result.db.__mbr__.block_table =
                    result.db.__mbr__.block_table ??
                    new Dictionary<string, PyWrapper<BlockTableRecord>>();

                SelectBlock(db, db.GetBlock(AcDb.BlockTableRecord.ModelSpace),
                    result.db.__mbr__.block_table);
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

            if ((table_flags & (int)TableFlags.MLeaderStyle) != 0)
            {
                result.db.__mbr__.m_leader_style_dict =
                    result.db.__mbr__.m_leader_style_dict ??
                    new Dictionary<string, PyWrapper<MLeaderStyle>>();

                SelectDictItems(db, db.MLeaderStyleDictionaryId,
                    result.db.__mbr__.m_leader_style_dict);
            }

            if ((table_flags & (int)TableFlags.Blocks) != 0
                && block_names != null)
            {
                result.db.__mbr__.block_table =
                    result.db.__mbr__.block_table ??
                    new Dictionary<string, PyWrapper<BlockTableRecord>>();

                var blkTbl = (AcDb.SymbolTable)trans.GetObject(
                    db.BlockTableId, AcDb.OpenMode.ForRead);
                foreach (var blkId in blkTbl)
                {
                    var block = (AcDb.BlockTableRecord)trans.GetObject(
                        blkId, AcDb.OpenMode.ForRead);
                    if (block.Name.StartsWith("*")
                        || block.Name.StartsWith("_"))
                    {
                        continue;
                    }

                    if (block_names.Count > 0
                        && block_names.Contains(block.Name))
                    {
                        SelectBlock(db, block, result.db.__mbr__.block_table);
                    }
                }
            }
        }

        private static void SelectBlock(AcDb.Database db,
            AcDb.BlockTableRecord arxBlock, BlockTable blockTable)
        {
            var block = new BlockTableRecord();
            block.FromArx(arxBlock, db);

            // *MODEL_SPACE needs special treatment for case-insensitivity。
            var blockName = arxBlock.Name;
            if (string.Equals(blockName, AcDb.BlockTableRecord.ModelSpace,
                    StringComparison.CurrentCultureIgnoreCase))
            {
                blockName = block.name = AcDb.BlockTableRecord.ModelSpace;
            }

            blockTable[blockName] = PyWrapper<BlockTableRecord>.Create(block);
        }

        private void TestEntities(AcDb.Database db, DbSelectResult result)
        {
            result.db = result.db ?? PyWrapper<Database>.Create(new Database());

            var modelSpace = database?.__mbr__?.GetModelSpace();
            if (!(modelSpace?.entities?.Count > 0)) return;

            var resultModelSpace = result.db.__mbr__.GetModelSpace();

            foreach (var entity in modelSpace.entities)
            {
                using (var arxEntity = entity.__mbr__.ToArx(null, db))
                {
                    (arxEntity as AcDb.Dimension)?.GenerateLayout();
                    (arxEntity as AcDb.DBText)?.AdjustAlignment(db);
                    // TODO

                    var newEntity = entity.__mbr__.CloneEntity();
                    newEntity.FromArx(arxEntity, db);

                    resultModelSpace.AddEntity(
                        PyWrapper<Entity>.Create(newEntity));
                }
            }
        }

        private void GetUserSelection(AcDb.Database db, DbSelectResult result)
        {
            var trans = db.TransactionManager.TopTransaction;

            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var editor = AcAp.Application.DocumentManager.MdiActiveDocument
                .Editor;

            AcEi.PromptSelectionResult selected;
            if (select_by_prompt == true)
            {
                var option = new AcEi.PromptSelectionOptions
                {
                    MessageForAdding =
                        "Please select entities for opertation"
                };
                selected = editor.GetSelection(option);
            }
            else
            {
                selected = editor.SelectImplied();
                if (selected.Status != AcEi.PromptStatus.OK)
                {
                    result.status = Status.Failure;
                    result.message = "None of entities is selected.";
                    return;
                }
            }

            var modelSpace = result.db.__mbr__.GetModelSpace();

            using (var exploded = new AcDb.DBObjectCollection())
            {
                foreach (var id in selected.Value.GetObjectIds())
                {
                    if (!id.IsValid) continue;

                    var arxEntity =
                        (AcDb.Entity)trans.GetObject(id, AcDb.OpenMode.ForRead);

                    if (arxEntity is AcDb.BlockReference &&
                        explode_blocks == true)
                    {
                        arxEntity.Explode(exploded);
                        continue;
                    }

                    var entity = Entity.Convert(arxEntity, db);
                    if (entity == null) continue;

                    modelSpace.entities.Add(PyWrapper<Entity>.Create(entity));
                }

                foreach (AcDb.Entity arxEntity in exploded)
                {
                    var entity = Entity.Convert(arxEntity, db);
                    if (entity == null) continue;

                    modelSpace.entities.Add(PyWrapper<Entity>.Create(entity));
                }
            }
        }

        private void GetGroups(AcDb.Database db, DbSelectResult result)
        {
            var trans = db.TransactionManager.TopTransaction;
            result.db = result.db ?? PyWrapper<Database>.Create(new Database());

            if (group_names?.Any() != true) return;
            var groups = group_names
                .Select(db.GetGroup)
                .Where(g => g != null)
                .ToArray();
            var groupIds = groups
                .Select(g => g.ObjectId)
                .ToList();

            var modelSpace = db.GetBlock(AcDb.BlockTableRecord.ModelSpace);
            var resultModelSpace = result.db.__mbr__.GetModelSpace();
            foreach (var eid in modelSpace)
            {
                var ent = (AcDb.Entity)trans.GetObject(
                    eid, AcDb.OpenMode.ForRead);

                var rids = ent.GetPersistentReactorIds();
                if ((rids?.Count ?? 0) == 0) continue;

                foreach (AcDb.ObjectId rid in rids)
                {
                    if (!groupIds.Contains(rid)) continue;

                    var resultEntity = Entity.Convert(ent, db);
                    if (resultEntity == null) continue;

                    resultModelSpace.entities.Add(
                        PyWrapper<Entity>.Create(resultEntity));
                }
            }

            var resultGroupDict = result.db.__mbr__.GetGroupDict();
            foreach (var group in groups)
            {
                var resultGroup = new Group();
                resultGroup.FromArx(group, db);
                resultGroupDict[resultGroup.name] =
                    PyWrapper<Group>.Create(resultGroup);
            }
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

        private static void SelectDictItems<TItem>(
            AcDb.Database db, AcDb.ObjectId dictionaryId,
            IDictionary<string, PyWrapper<TItem>> dict)
            where TItem : DictionaryItem, new()
        {
            var trans = db.TransactionManager.TopTransaction;

            var arxDict = (AcDb.DBDictionary)trans.GetObject(dictionaryId,
                AcDb.OpenMode.ForRead);
            foreach (var entry in arxDict)
            {
                if (!entry.Value.IsValid) continue;

                var arxItem = trans.GetObject(entry.Value,
                    AcDb.OpenMode.ForRead);

                var item = new TItem();
                item.FromArx(arxItem, db);

                dict[item.name] = PyWrapper<TItem>.Create(item);
            }
        }

        public enum Mode
        {
            GetTables,
            GetUserSelection,
            TestEntities,
            GetGroups,
        }

        private enum TableFlags
        {
            ModelSpace = 0x01,
            TextStyle = 0x02,
            Linetype = 0x04,
            Layer = 0x08,
            DimStyle = 0x10,
            MLeaderStyle = 0x20,
            Blocks = 0x40,
        }
    }

    public sealed class DbDeleteQuery : DbQuery
    {
        public bool? delete_group_entities;

        public override Result Execute()
        {
            var result = new DbDeleteResult();

            var db = AcDb.HostApplicationServices.WorkingDatabase;
            try
            {
                var clientDb = database?.__mbr__;

                using (var trans = db.TransactionManager.StartTransaction())
                {
                    DeleteEntities(db, clientDb?.block_table, trans, result);
                    DeleteGroups(db, clientDb?.group_dict, trans, result);
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

        private void DeleteEntities(AcDb.Database db, BlockTable blockTable,
            AcDb.Transaction trans, DbDeleteResult result)
        {
            foreach (var block in blockTable.Values)
            {
                foreach (var entity in block.__mbr__.entities)
                {
                    if (!entity.__mbr__.id.HasValue) continue;
                    var eid =
                        new AcDb.ObjectId(new IntPtr(entity.__mbr__.id.Value));
                    if (!eid.IsValid) continue;
                    trans.GetObject(eid, AcDb.OpenMode.ForWrite).Erase();
                    result.num_deleted++;
                }
            }
        }

        private void DeleteGroups(AcDb.Database db, GroupDict groupDict,
            AcDb.Transaction trans, DbDeleteResult result)
        {
            if (groupDict?.Any() != true) return;
            foreach (var entry in groupDict)
            {
                var group = db.GetGroup(entry.Key);
                if (group == null) continue;

                if (delete_group_entities == true)
                {
                    foreach (var eid in group.GetAllEntityIds())
                    {
                        trans.GetObject(eid, AcDb.OpenMode.ForWrite).Erase();
                        result.num_deleted++;
                    }
                }

                group.UpgradeOpen();
                group.Erase();
                result.num_deleted++;
            }
        }
    }
}
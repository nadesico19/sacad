# Copyright (c) 2022 Chin Ako <nadesico19@gmail.com>
# sacad is licensed under Mulan PSL v2.
# You can use this software according to the terms and conditions of the Mulan
# PSL v2.
# You may obtain a copy of Mulan PSL v2 at:
#          http://license.coscl.org.cn/MulanPSL2
# THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND,
# EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,
# MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
# See the Mulan PSL v2 for more details.

from dataclasses import dataclass
from functools import cached_property
from typing import List, Dict, Iterable

from sacad.acdb import (
    Database,
    DBObject,
    ObjectId,
    MODEL_SPACE,
)
from sacad.jsonify import Jsonify
from sacad.result import (
    DBInsertResult,
)
from sacad.session import Session

__all__ = [
    'DBInserter',
]


@dataclass
class DBQuery(Jsonify):
    query_type: str
    database: Database


@dataclass
class DBInsertQuery(DBQuery):
    upsert: bool

    def _jsonify_traverse_dict(self, self_dict):
        result = {"$type": "SacadMgd.DbInsertQuery, SacadMgd"}
        result.update(super()._jsonify_traverse_dict(self_dict))
        return result


class DBInserter:
    def __init__(self, session: Session, upsert: bool):
        self._session = session
        self._upsert = upsert

        self._db = Database()

    @cached_property
    def modelspace(self) -> 'ListInsertProxy':
        return ListInsertProxy(
            self, self._db.get_blocktable(MODEL_SPACE).entities)

    @cached_property
    def layertable(self) -> 'DictInsertProxy':
        return DictInsertProxy(self, self._db.layertable)

    def submit(self):
        query = DBInsertQuery('DB_INSERT', self._db, self._upsert)
        request = query.serialize()
        response = self._session.db_operation(request)
        return DBInsertResult.deserialize(response)


class ListInsertProxy:
    def __init__(self, inserter: DBInserter, objects: List[DBObject]):
        self._ins = inserter
        self._lst = objects

    def insert(self, dbobj: DBObject):
        self._lst.append(dbobj)

    def insert_and_ref(self, dbobj: DBObject) -> ObjectId:
        dbobj.id = id(dbobj)
        self.insert(dbobj)
        return dbobj.id

    def insert_many(self, dbobjs: Iterable[DBObject]):
        self._lst.extend(dbobjs)


class DictInsertProxy:
    def __init__(self, inserter: DBInserter, objects: Dict[str, DBObject]):
        self._ins = inserter
        self._dic = objects

    def insert(self, dbobj: DBObject):
        self._dic[getattr(dbobj, 'name')] = dbobj

    def insert_and_ref(self, dbobj: DBObject) -> ObjectId:
        dbobj.id = id(dbobj)
        self.insert(dbobj)
        return dbobj.id

    def insert_many(self, dbobjs: Iterable[DBObject]):
        for o in dbobjs:
            self.insert(o)

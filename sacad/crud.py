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

from dataclasses import dataclass, field
from enum import IntEnum
from functools import cached_property
from typing import List, Dict, Iterable

from sacad.acdb import (
    Database,
    DBObject,
    ObjectId,
    MODEL_SPACE,
)
from sacad.acge import Vector3d
from sacad.error import AcadTcpError
from sacad.jsonify import Jsonify
from sacad.result import (
    DBInsertResult,
)
from sacad.session import Session
from sacad.util import csharp_polymorphic_type

__all__ = [
    'DBInsert',
    'DBInsertQuery',
    'ZoomMode',
]


@dataclass
class DBQuery(Jsonify):
    database: Database = field(default_factory=Database)


class ZoomMode(IntEnum):
    NONE = 0
    ADDED = 1
    ALL = 2


@dataclass
class DBInsertQuery(DBQuery):
    insertion_point: Vector3d = None
    prompt_insertion_point: bool = None
    upsert: bool = None
    zoom_mode: ZoomMode = None
    zoom_factor: float = None


class DBOperator:
    def __init__(self, session: Session, db: Database):
        self._session = session
        self._db = db

    def _sumbit(self, request: str):
        try:
            if not self._session.is_alive():
                self._session.open()

            return self._session.db_operation(request)
        except AcadTcpError as e:
            self._session.reset()
            raise e


class DBInsert(DBOperator):
    def __init__(self, session: Session, query: DBInsertQuery):
        super().__init__(session, query.database)
        self._query = query

    @property
    def query(self):
        return self._query

    @cached_property
    def model_space(self) -> 'ListInsertProxy':
        return ListInsertProxy(self, self._db.get_block(MODEL_SPACE).entities)

    @cached_property
    def dim_style_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self, self._db.dim_style_table)

    @cached_property
    def layer_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self, self._db.layer_table)

    @cached_property
    def linetype_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self, self._db.linetype_table)

    @cached_property
    def text_style_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self, self._db.text_style_table)

    def submit(self):
        request = self._query.serialize()
        response = self._sumbit(request)
        return DBInsertResult.deserialize(response)


class ListInsertProxy:
    def __init__(self, inserter: DBInsert, objects: List[DBObject]):
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
    def __init__(self, inserter: DBInsert, objects: Dict[str, DBObject]):
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


# Syntactic sugar of @decorator may somehow break the code completion of IDE
# (e.g. PyCharm) on @dataclass.

DBInsertQuery = csharp_polymorphic_type("SacadMgd.DbInsertQuery, SacadMgd")(
    DBInsertQuery)

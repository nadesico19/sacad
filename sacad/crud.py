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
from typing import List, Dict, Iterable, Optional

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
    DBSelectResult,
)
from sacad.session import Session
from sacad.util import csharp_polymorphic_type

__all__ = [
    'ZoomMode',
    'SelectMode',
    'TableFlags',
    'DBInsert',
    'DBInsertQuery',
    'DBSelect',
    'DBSelectQuery',
]


class ZoomMode(IntEnum):
    NONE = 0
    ADDED = 1
    ALL = 2


class SelectMode(IntEnum):
    GET_TABLES = 0
    GET_USER_SELECTION = 1
    TEST_ENTITIES = 2


class TableFlags(IntEnum):
    MODEL_SPACE = 0x01
    TEXT_STYLE = 0x02
    LINETYPE = 0x04
    LAYER = 0x08
    DIM_STYLE = 0x10
    M_LEADER_STYLE = 0x20


@dataclass
class DBQuery(Jsonify):
    database: Database = field(default_factory=Database)


@dataclass
class DBInsertQuery(DBQuery):
    insertion_point: Optional[Vector3d] = None
    prompt_insertion_point: Optional[bool] = None
    upsert: Optional[bool] = None
    zoom_mode: Optional[ZoomMode] = None
    zoom_factor: Optional[float] = None


@dataclass
class DBSelectQuery(DBQuery):
    mode: Optional[SelectMode] = None
    table_flags: Optional[int] = None
    explode_blocks: Optional[bool] = None


class DBOperator:
    def __init__(self, session: Session, query: DBQuery):
        self._session = session
        self._query = query

    @property
    def query(self):
        return self._query

    def replace_db(self, db: Database) -> Database:
        tmp, self._query.database = self._query.database, db
        return tmp

    def _sumbit(self):
        try:
            request = self._query.serialize()

            if not self._session.is_alive():
                self._session.open()

            return self._session.db_operation(request)
        except AcadTcpError as e:
            self._session.reset()
            raise e


class DBInsert(DBOperator):
    def __init__(self, session: Session, query: DBInsertQuery):
        super().__init__(session, query)

    @cached_property
    def model_space(self) -> 'ListInsertProxy':
        return ListInsertProxy(
            self._query.database.get_block(MODEL_SPACE).entities)

    @cached_property
    def dim_style_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self._query.database.dim_style_table)

    @cached_property
    def layer_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self._query.database.layer_table)

    @cached_property
    def linetype_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self._query.database.linetype_table)

    @cached_property
    def text_style_table(self) -> 'DictInsertProxy':
        return DictInsertProxy(self._query.database.text_style_table)

    def submit(self) -> DBInsertResult:
        return DBInsertResult.deserialize(self._sumbit())


class DBSelect(DBOperator):
    def __init__(self, session: Session, query: DBSelectQuery):
        super().__init__(session, query)

    @cached_property
    def tested_entities(self) -> 'ListInsertProxy':
        return ListInsertProxy(
            self._query.database.get_block(MODEL_SPACE).entities)

    def submit(self) -> DBSelectResult:
        return DBSelectResult.deserialize(self._sumbit())


class ListInsertProxy:
    def __init__(self, objects: List[DBObject]):
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
    def __init__(self, objects: Dict[str, DBObject]):
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
DBSelectQuery = csharp_polymorphic_type("SacadMgd.DbSelectQuery, SacadMgd")(
    DBSelectQuery)

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

"""A facade class for user code to access all features of sacad."""

from contextlib import contextmanager
from typing import ContextManager

from sacad.constant import ACAD_LATEST
from sacad.crud import (
    DBInsert,
    DBInsertQuery,
)
from sacad.session import Session

__all__ = [
    'Acad',
    'instant_acad',
]


class Acad:
    def __init__(self, acad_name=ACAD_LATEST, host='127.0.0.1', port=48652):
        self._session = Session(acad_name, host, port)

    def open(self, netload=True):
        if not self._session.is_alive():
            self._session.open(netload=netload)

    def reset(self):
        self._session.reset()

    def close(self):
        self._session.close()

    def db_insert(self, **kwargs) -> DBInsert:
        return DBInsert(self._session, DBInsertQuery(**kwargs))

    @property
    def name(self):
        return self._session.acad_name


@contextmanager
def instant_acad(netload=True, **kwargs) -> ContextManager[Acad]:
    acad = Acad(**kwargs)
    try:
        acad.open(netload=netload)
        yield acad
    finally:
        acad.close()

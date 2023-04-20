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

import uuid

from typing import Callable, Optional

from sacad import env
from sacad.com import ComAcad
from sacad.constant import ACAD_LATEST
from sacad.error import (
    AcadConnectionError,
    AcadNotFoundError,
    AcadNotSupportedError,
)
from sacad.io import Requester

__all__ = ['Session']


class Session:
    def __init__(self, acad_name: str, host: str, port: int):
        self._name = acad_name
        self._host = host
        self._port = port
        self._skey = str(uuid.uuid1())

        self._req = Requester()
        self._com: Optional[ComAcad] = None

        self._precheck()

    def open(self, netload=True):
        self._com = ComAcad(env.acad_progid(self._name))
        self._com.show()

        dllpath = env.find_dll(self._name)
        if not dllpath:
            raise AcadNotSupportedError(
                f'SacadMgd.dll for AutoCAD {self._name} is not found.')

        if netload:
            self._com.netload(dllpath)

        self._req.open(self._host, self._port,
                       on_listening=lambda: self._com.connect(
                           f'{self._host}:{self._port}', self._skey))

        self._ensure_connection()

    def reset(self):
        self._req.reset()
        self._com = None

    def close(self):
        self._req.close()
        self._com = None

    def is_alive(self) -> bool:
        if not self._com:
            return False

        try:
            self._ensure_connection()
        except AcadConnectionError:
            return False

        return True

    def db_operation(self, opcmd: str):
        return self._request(opcmd, self._com.dbop)

    def doc_operation(self, opcmd: str):
        return self._request(opcmd, self._com.docop)

    @property
    def acad_name(self):
        return self._name

    def _precheck(self):
        acad_names = env.available_acad()
        if not acad_names:
            raise AcadNotFoundError(
                'No AutoCAD installation found in the registry.')

        if self._name == ACAD_LATEST:
            self._name = acad_names[-1]

        if not env.is_implemented(self._name):
            raise AcadNotSupportedError(
                f'AutoCAD {self._name} is not supported by sacad.')

        if self._name not in acad_names:
            raise AcadNotFoundError(
                f'AutoCAD {self._name} is not found in the registry.')

    def _request(self, msg: str, cmd: Callable):
        fut = self._req.request(msg)
        cmd(self._skey)
        return fut.result()

    def _ensure_connection(self):
        pong = self._request('ping', self._com.ping)
        assert pong == 'pong'

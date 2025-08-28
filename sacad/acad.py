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

"""A facade for user code to access features of sacad conveniently ."""

from contextlib import contextmanager
from typing import List, Optional, Union

import pythoncom

from sacad.acge import Vector3d
from sacad.constant import ACAD_LATEST
from sacad.crud import (
    DBInsert,
    DBInsertQuery,
    DBSelect,
    DBSelectQuery,
    DBDelete,
    DBDeleteQuery,
    SelectMode,
)
from sacad.env import available_acad
from sacad.session import Session

__all__ = [
    'Acad',
    'instant_acad',
]


class Acad:
    """
    A front-end to facilitate access to various features provided by sacad.
    """

    def __init__(self, acad_name=ACAD_LATEST, host='127.0.0.1', port=48652):
        """
        Initialization.

        Hostname and port pair are used to create TCP connection with AutoCAD.
        The Python module will run a TCP listener with these parameters, to
        accept connection from the C# module.

        :param acad_name: version identifier defined in constant.py.
        :param host: hostname of TCP listener.
        :param port: port of TCP listener.
        """
        self._session = Session(acad_name, host, port)

    def open(self, netload=True):
        """
        Create a connection to the specified version of AutoCAD. This will
        launch a new AutoCAD process automatically, if it is not running.

        :param netload: execute the NETLOAD command to load C# module while
                        creating connection with AutoCAD. In most cases, always
                        doing the same thing is not harmful, even if the C#
                        module has been loaded. But the command executing will
                        clear the user's selection set, so cause some operations
                        fail when calling Editor.SelectImplied.
        """
        if not self._session.is_alive():
            self._session.open(netload=netload)

    def reset(self):
        """
        Reset the connection with AutoCAD.

        Reopen is possible after this operation.
        """
        self._session.reset()

    def close(self):
        """
        Close the connection with AutoCAD.

        Cannot reopen after this operation.
        """
        self._session.close()

    def activate(self):
        """
        Brings the AutoCAD window into the foreground and activates the window.

        This could be useful when prompting for user input from AutoCAD. (e.g.,
        get a user specified insertion point.)

        This operation can only be performed while the connection is valid.
        """
        self.open(netload=False)
        self._session.com_acad.activate()

    def db_insert(
            self,
            insertion_point: Optional[Vector3d] = None,
            prompt_insertion_point: Optional[bool] = None,
            upsert: Optional[bool] = None,
            **kwargs) -> DBInsert:
        """
        Create an insertion type of transaction to be executed in AutoCAD.

        :param insertion_point: the origin of inserted entities in model space.
        :param prompt_insertion_point:
                       when True is specified, AutoCAD prompts for user input
                       of a point, which becomes the insertion point.
        :param upsert: when styles with same name already exist, True will lead
                       to updates. Otherwise, only insertion occurs.
        :param kwargs: other parameters of DBInsertQuery.__init__.
        """
        return DBInsert(self._session, DBInsertQuery(
            insertion_point=insertion_point,
            prompt_insertion_point=prompt_insertion_point,
            upsert=upsert,
            **kwargs))

    def db_get_tables(self, table_flags: int, **kwargs) -> DBSelect:
        return DBSelect(self._session, DBSelectQuery(
            mode=SelectMode.GET_TABLES, table_flags=table_flags, **kwargs))

    def db_get_user_selection(
            self, by_prompt: bool = False, **kwargs) -> DBSelect:
        """
        Create a transaction for getting user-selected entities.

        :param by_prompt: when True is specified, AutoCAD show prompt message
                          to ask user to select a set of entities. Unlike the
                          default behavior of pick first (False), AutoCAD will
                          block until user confirm the selection.
        :param kwargs: other parameters of DBSelectQuery.__init__.
        """
        return DBSelect(self._session, DBSelectQuery(
            mode=SelectMode.GET_USER_SELECTION,
            select_by_prompt=by_prompt,
            **kwargs))

    def db_test_entities(self, **kwargs) -> DBSelect:
        return DBSelect(self._session, DBSelectQuery(
            mode=SelectMode.TEST_ENTITIES, **kwargs))

    def db_get_groups(self, name: Union[str, List[str]], **kwargs) -> DBSelect:
        names = [name] if isinstance(name, str) else list(name)
        return DBSelect(self._session, DBSelectQuery(
            mode=SelectMode.GET_GROUPS, group_names=names, **kwargs))

    def db_delete(
            self,
            delete_group_entities: Optional[bool] = None,
            **kwargs) -> DBDelete:
        return DBDelete(self._session, DBDeleteQuery(
            delete_group_entities=delete_group_entities,
            **kwargs))

    def send_command(self, name, *args):
        com = self._session.com_acad
        cmd = com.buildcmd(name, *map(str, args))
        com.sendcmd(cmd)

    @staticmethod
    def get_available() -> List[str]:
        """
        Get versions of AutoCAD available on the operating system.

        :return: a list of strings, containing each version identifier of
                 AutoCAD which is available on the operating system.
        """
        return available_acad()

    @property
    def name(self) -> str:
        """
        The version identifier of AutoCAD specified to this object.

        If ACAD_LATEST is specified, the actual version identifier cannot be
        resolved until the call to open.

        :return: version identifier string.
        """
        return self._session.acad_name

    @property
    def com(self):
        return self._session.com_acad


@contextmanager
def instant_acad(netload=True, acad_name=ACAD_LATEST, sta=False, **kwargs):
    """
    Use with statement to create an auto open/close instance of Acad.

    Suitable for scripts that do one-shot execution.

    :param netload: execute the NETLOAD command to load C# module while
                    creating connection with AutoCAD. In most cases, always
                    doing the same thing is not harmful, even if the C#
                    module has been loaded. But the command executing will
                    clear the user's selection set, so cause some operations
                    fail when calling Editor.SelectImplied.
    :param acad_name: version identifier defined in constant.py.
    :param sta: initialize COM as single-thread apartment (STA). This is useful
                when submissions are done by threads other than caller of this.
    :param kwargs: other parameters of Acad.__init__
    """
    acad = Acad(acad_name=acad_name, **kwargs)
    try:
        if sta:
            pythoncom.CoInitialize()
        acad.open(netload=netload)
        yield acad
    finally:
        acad.close()
        if sta:
            pythoncom.CoUninitialize()

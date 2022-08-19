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

__all__ = [
    'Error',
    'JsonifyError',
    'SessionError',
    'AcadNotFoundError',
    'AcadNotSupportedError',
    'AcadConnectionError',
    'AcadComError',
    'AcadTcpError',
]


class Error(Exception):
    pass


class JsonifyError(Error):
    pass


class SessionError(Error):
    pass


class AcadNotFoundError(SessionError):
    pass


class AcadNotSupportedError(SessionError):
    pass


class AcadConnectionError(SessionError):
    pass


class AcadComError(AcadConnectionError):
    pass


class AcadTcpError(AcadConnectionError):
    pass

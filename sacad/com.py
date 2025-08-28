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

import os.path
import time

from contextlib import suppress

import pythoncom
import win32com.client as com
import win32con
import win32gui

from functools import wraps

from sacad.error import AcadComError

__all__ = ['ComAcad']

COM_EXCEPTION = -2147352567
RPC_E_CALL_REJECTED = -2147418111
E_NO_DOCUMENT = -2145320900


class RetryError(Exception):
    def __init__(self, e, *args):
        super().__init__(*args)
        self.real_e = e


def retryable(f):
    @wraps(f)
    def wrapper(*args, **kwargs):
        last_e = None

        for i in range(max(args[0].max_retry_count, 0) + 1):
            try:
                if i > 0:
                    time.sleep(max(args[0].retry_delay, 0))
                return f(*args, **kwargs)
            except RetryError as e:
                last_e = e

        raise AcadComError('CAD is busy now.') from last_e.real_e

    return wrapper


class ComAcad:
    def __init__(self, progid: str, retry_delay=0.1, max_retry_count=20):
        self._acad = com.Dispatch(progid)

        self.retry_delay = retry_delay
        self.max_retry_count = max_retry_count

    @property
    def app(self):
        return self._acad

    @retryable
    def show(self):
        try:
            self._acad.Visible = True
        except AttributeError as attr_err:
            raise RetryError(attr_err)

    def activate(self):
        with suppress(Exception):
            hwnd = self._acad.HWND
            is_visible = win32gui.IsWindowVisible(hwnd)
            win32gui.SetWindowPos(
                hwnd, win32con.HWND_TOP, 0, 0, 0, 0,
                win32con.SWP_NOMOVE |
                win32con.SWP_NOSIZE |
                win32con.SWP_SHOWWINDOW |
                (win32con.SWP_NOACTIVATE if is_visible else 0))
            win32gui.SetForegroundWindow(hwnd)

    def netload(self, path: str):
        dirname = os.path.dirname(path)
        dir_esc = dirname.replace('\\', '\\\\')
        cmd = self.buildcmd('TRUSTEDPATHS', dir_esc)
        self.sendcmd(cmd)

        path_esc = path.replace('\\', '\\\\')
        cmd = self.buildcmd('NETLOAD', path_esc)
        self.sendcmd(cmd)

    def connect(self, host: str, skey: str):
        cmd = self.buildcmd('SACAD_CONNECT', host, skey)
        self.sendcmd(cmd)

    def ping(self, skey: str):
        cmd = self.buildcmd('SACAD_PING', skey)
        self.sendcmd(cmd)

    def dbop(self, skey: str):
        cmd = self.buildcmd('SACAD_DBOP', skey)
        self.sendcmd(cmd)

    def docop(self, skey: str):
        cmd = self.buildcmd('SACAD_DOCOP', skey)
        self.sendcmd(cmd)

    @staticmethod
    def buildcmd(*args):
        argstr = '" "'.join(args)
        return f'(command "{argstr}") '

    @retryable
    def sendcmd(self, cmd):
        try:
            self._acad.ActiveDocument.SendCommand(cmd)
        except AttributeError as attr_err:
            raise RetryError(attr_err)
        except pythoncom.com_error as com_err:
            if com_err.hresult == COM_EXCEPTION:
                if E_NO_DOCUMENT in com_err.excepinfo:
                    self._acad.Documents.Add()
                    raise RetryError(com_err)
            elif com_err.hresult == RPC_E_CALL_REJECTED:
                raise RetryError(com_err)
            raise AcadComError(com_err.strerror) from com_err
        except Exception as other_err:
            raise AcadComError(other_err) from other_err

    def get_real(self, prompt='Please input a real number: '):
        return self._acad.ActiveDocument.Utility.GetReal(prompt)

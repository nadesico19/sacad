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

import os
import winreg

import sacad

from typing import List, Optional

__all__ = [
    'available_acad',
    'acad_progid',
    'find_dll',
    'is_implemented',
]

ACAD_NAME_PROGID_MAP = {
    '2010': 'AutoCAD.Application.18',
    '2011': 'AutoCAD.Application.18.1',
    '2012': 'AutoCAD.Application.18.2',
    '2013': 'AutoCAD.Application.19',
    '2014': 'AutoCAD.Application.19.1',
    '2015': 'AutoCAD.Application.20',
    '2016': 'AutoCAD.Application.20.1',
    '2017': 'AutoCAD.Application.21',
    '2018': 'AutoCAD.Application.22',
    '2019': 'AutoCAD.Application.23',
    '2020': 'AutoCAD.Application.23.1',
    '2021': 'AutoCAD.Application.24',
    '2022': 'AutoCAD.Application.24.1',
    '2023': 'AutoCAD.Application.24.2',
}

ACAD_VERSION_NAME_MAP = {
    'R18.0': '2010',
    'R18.1': '2011',
    'R18.2': '2012',
    'R19.0': '2013',
    'R19.1': '2014',
    'R20.0': '2015',
    'R20.1': '2016',
    'R21.0': '2017',
    'R22.0': '2018',
    'R23.0': '2019',
    'R23.1': '2020',
    'R24.0': '2021',
    'R24.1': '2022',
    'R24.2': '2023',
}


def available_acad() -> List[str]:
    versions = []

    try:
        with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE,
                            r'SOFTWARE\Autodesk\AutoCAD') as regkey:
            while True:
                versions.append(winreg.EnumKey(regkey, len(versions)))
    except OSError as e:
        if e.winerror != 259:  # ERROR_NO_MORE_ITEMS
            raise e

    return [ACAD_VERSION_NAME_MAP[k] for k in ACAD_VERSION_NAME_MAP if
            k in versions]


def is_implemented(name: str) -> bool:
    return name in ACAD_NAME_PROGID_MAP


def acad_progid(name: str) -> str:
    return ACAD_NAME_PROGID_MAP[name]


def find_dll(name: str) -> Optional[str]:
    toplevel = os.path.dirname(os.path.abspath(sacad.__file__))
    dllfolder = os.path.join(toplevel, 'dll')

    acad_names_desc = list(reversed(ACAD_NAME_PROGID_MAP.keys()))
    if name not in acad_names_desc:
        return

    startidx = acad_names_desc.index(name)
    dllfiles = os.listdir(dllfolder)

    for i in range(startidx, len(acad_names_desc)):
        if acad_names_desc[i] in dllfiles:
            return os.path.join(dllfolder, acad_names_desc[i], 'SacadMgd.dll')

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
from enum import IntEnum

from sacad.jsonify import Jsonify

__all__ = [
    'Result',
    'DBInsertResult',
]


class Status(IntEnum):
    UNKNOWN = 0
    SUCCESS = 1
    FAILURE = 2
    WARNING = 3


@dataclass
class Result(Jsonify):
    status: Status = Status.UNKNOWN
    message: str = None


@dataclass
class DBInsertResult(Result):
    num_inserted: int = 0
    num_updated: int = 0
    num_failure: int = 0

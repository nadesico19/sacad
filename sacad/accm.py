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

"""accm: AcCm stands for `Autodesk.AutoCAD.Colors`."""

from dataclasses import dataclass
from enum import IntEnum
from typing import Optional

from sacad.jsonify import Jsonify

__all__ = [
    'ColorMethod',
    'Color',
]


class ColorMethod(IntEnum):
    BY_ACI = 0xc3
    BY_BLOCK = 0xc1
    BY_COLOR = 0xc2
    BY_LAYER = 0xc0
    BY_PEN = 0xc4
    FOREGROUND = 0xc5
    LAYER_FROZEN = 0xc7
    LAYER_OFF = 0xc6
    NONE = 200


@dataclass
class Color(Jsonify):
    color_method: Optional[ColorMethod] = None
    red: Optional[int] = None
    green: Optional[int] = None
    blue: Optional[int] = None
    color_index: Optional[int] = None

    @staticmethod
    def rgb(r: int, g: int, b: int) -> 'Color':
        return Color(red=r, green=g, blue=b, color_method=ColorMethod.BY_COLOR)

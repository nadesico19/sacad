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

import ctypes

from typing import Type

from sacad.jsonify import Jsonify

__all__ = [
    'pyobj_from_id',
    'csharp_polymorphic_type',
]


def pyobj_from_id(poid: int):
    return ctypes.cast(poid, ctypes.py_object).value


def csharp_polymorphic_type(signature: str):
    def decorator(cls: Type[Jsonify]):
        original = cls._jsonify_traverse_dict

        def override(self, self_dict):
            result = {"$type": signature}
            result.update(original(self, self_dict))
            return result

        cls._jsonify_traverse_dict = override
        return cls

    return decorator

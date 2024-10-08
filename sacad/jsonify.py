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

import importlib
import json

from enum import IntEnum
from typing import Any, Dict, Set, TypeVar, Union

from sacad.error import JsonifyError

__all__ = ['Jsonify']

T = TypeVar('T', bound='Jsonify')
CLASS_KEY = '__cls__'
MEMBER_KEY = '__mbr__'


class Jsonify:
    _jsonify_registry: Dict[str, T] = {}
    _excluded_attributes: Set[str] = set()

    def __init_subclass__(cls, **kwargs):
        super().__init_subclass__(**kwargs)
        Jsonify._jsonify_registry[cls._jsonify_classname()] = cls

    @classmethod
    def _jsonify_classname(cls):
        return f'{cls.__module__}.{cls.__name__}'

    def _jsonify_to_dict(self):
        return {
            CLASS_KEY: self.__class__._jsonify_classname(),
            MEMBER_KEY: self._jsonify_traverse_dict(self.__dict__)
        }

    def _jsonify_traverse_dict(self, self_dict):
        return {key: self._jsonify_traverse(key, value)
                for key, value in self_dict.items()
                if value is not None}

    def _jsonify_traverse(self, key, value):
        if isinstance(value, Jsonify):
            return value._jsonify_to_dict()
        elif isinstance(value, dict):
            return self._jsonify_traverse_dict(value)
        elif isinstance(value, (list, tuple, set)):
            return [self._jsonify_traverse(key, e) for e in value]
        elif isinstance(value, IntEnum):
            return value
        elif hasattr(value, '__dict__'):
            return self._jsonify_traverse_dict(value.__dict__)
        else:
            return value

    @staticmethod
    def _check_loaded(clsname):
        if clsname in Jsonify._jsonify_registry:
            return

        importlib.import_module('.'.join(clsname.split('.')[:-1]))

    @staticmethod
    def _jsonify_from_jsonobj(obj):
        if isinstance(obj, dict):
            if CLASS_KEY in obj:
                Jsonify._check_loaded(obj[CLASS_KEY])
                return Jsonify._jsonify_construct(obj)
            else:
                return {key: Jsonify._jsonify_from_jsonobj(value)
                        for key, value in obj.items()}
        elif isinstance(obj, list):
            return [Jsonify._jsonify_from_jsonobj(e) for e in obj]
        else:
            return obj

    @staticmethod
    def _jsonify_construct(obj):
        cls = Jsonify._jsonify_registry[obj[CLASS_KEY]]
        arg = Jsonify._jsonify_from_jsonobj(obj[MEMBER_KEY])
        if arg is None:
            return cls()
        elif isinstance(arg, (str, int, float, bool)):
            return cls(arg)
        elif isinstance(arg, list):
            return cls(*arg)
        elif isinstance(arg, dict):
            for excluded in Jsonify._excluded_attributes:
                arg.pop(excluded, None)
            return cls(**arg)
        else:
            raise JsonifyError(f'Cannot construct {cls!r} with {arg!r}.')

    def serialize(self, **kwargs) -> str:
        if isinstance(self, (list, tuple, set)):
            json_dict = list(map(Jsonify._jsonify_to_dict, self))
        elif isinstance(self, dict):
            json_dict = {key: Jsonify._jsonify_to_dict(value)
                         for key, value in self.items()}
        else:
            json_dict = self._jsonify_to_dict()

        return json.dumps(json_dict, **kwargs)

    @classmethod
    def deserialize(cls: T, json_data: Union[str, bytes, bytearray]) -> T:
        return Jsonify._jsonify_from_jsonobj(json.loads(json_data))

    def to_jsonify_dict(self) -> Dict[str, Any]:
        return self._jsonify_to_dict()

    @classmethod
    def from_jsonify_dict(cls: T, json_dict: Dict[str, Any]) -> T:
        return Jsonify._jsonify_from_jsonobj(json_dict)

    @staticmethod
    def register_excluded_attribute(name: str):
        Jsonify._excluded_attributes.add(name)

    class Encoder(json.JSONEncoder):
        def default(self, obj):
            if isinstance(obj, Jsonify):
                return obj._jsonify_to_dict()
            return super().default(obj)

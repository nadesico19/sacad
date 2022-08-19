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

"""acge: AcGe stands for `Autodesk.AutoCAD.Geometry`."""

import math
import operator as op

from typing import Type, Union

from sacad.jsonify import Jsonify

__all__ = [
    'Vector2d',
    'Vector3d',
]

Vector = Union['Vector2d', 'Vector3d']
Number = Union[int, float]
NumberClass = (int, float)


class _VectorBase(tuple, Jsonify):
    def __init_subclass__(cls: Type[Vector], **kwargs):
        super().__init_subclass__(**kwargs)
        setattr(cls, '_xaxis', cls(1, 0, 0))
        setattr(cls, '_yaxis', cls(0, 1, 0))
        setattr(cls, '_zaxis', cls(0, 0, 1))

    def _jsonify_traverse_dict(self, self_dict):
        if isinstance(self, Vector2d):
            return [self.x, self.y]
        elif isinstance(self, Vector3d):
            return [self.x, self.y, self.z]
        else:
            raise NotImplementedError

    def __add__(self: Vector, other: Vector) -> Vector:
        return self.__new__(type(self), *map(op.add, self, other))

    def __sub__(self: Vector, other: Vector) -> Vector:
        return self.__new__(type(self), *map(op.sub, self, other))

    def __mul__(self: Vector, other: Union[Vector, Number]
                ) -> Union[Vector, float]:
        if isinstance(other, type(self)):
            return sum(map(op.mul, self, other))
        if isinstance(other, NumberClass):
            return self.__new__(type(self), *(i * other for i in self))
        return NotImplemented

    def __rmul__(self: Vector, other: Number) -> Vector:
        return self.__mul__(other)

    def __truediv__(self: Vector, other: Number) -> Vector:
        if isinstance(other, NumberClass):
            return self.__new__(type(self), *(i / other for i in self))

    def __neg__(self: Vector) -> Vector:
        return self.__new__(type(self), *map(op.neg, self))

    def __abs__(self: Vector) -> float:
        return math.sqrt(sum(map(op.mul, self, self)))

    def __repr__(self: Vector):
        return f'{self.__class__.__name__}({",".join(repr(e) for e in self)})'

    @property
    def x(self) -> float:
        return self[0]

    @property
    def y(self) -> float:
        return self[1]

    @property
    def z(self) -> float:
        return self[2]

    @classmethod
    def xaxis(cls) -> Vector:
        return getattr(cls, '_xaxis')

    @classmethod
    def yaxis(cls) -> Vector:
        return getattr(cls, '_yaxis')

    @classmethod
    def zaxis(cls) -> Vector:
        return getattr(cls, '_zaxis')


class Vector2d(_VectorBase):
    def __new__(cls, x: Number = 0.0, y: Number = 0.0, _=0.0) -> 'Vector2d':
        return super().__new__(cls, (float(x), float(y)))

    def __matmul__(self, other: 'Vector2d') -> float:
        return self.x * other.y - other.x * self.y

    def rotate(self, angle) -> 'Vector2d':
        return self.rotater(angle * math.pi / 180.0)

    def rotater(self, radian) -> 'Vector2d':
        cosa, sina = math.cos(radian), math.sin(radian)
        return Vector2d(self.x * cosa - self.y * sina,
                        self.x * sina + self.y * cosa)


class Vector3d(_VectorBase):
    def __new__(cls, x: Number = 0.0, y: Number = 0.0, z: Number = 0.0
                ) -> 'Vector3d':
        return super().__new__(cls, (float(x), float(y), float(z)))

    def __matmul__(self, other: 'Vector3d') -> 'Vector3d':
        return Vector3d(
            self.y * other.z - self.z * other.y,
            self.z * other.x - self.x * other.z,
            self.x * other.y - self.y * other.x
        )

    def rotate(self, angle: Number, axis: 'Vector3d') -> 'Vector3d':
        return self.rotater(angle * math.pi / 180.0, axis)

    def rotater(self, radian: Number, axis: 'Vector3d') -> 'Vector3d':
        cosa, sina = math.cos(radian), math.sin(radian)
        if axis == self.xaxis():
            return Vector3d(self.x,
                            self.y * cosa - self.z * sina,
                            self.y * sina + self.z * cosa)
        elif axis == self.yaxis():
            return Vector3d(self.x * cosa + self.z * sina,
                            self.y,
                            -self.x * sina + self.z * cosa)
        elif axis == self.zaxis():
            return Vector3d(self.x * cosa - self.y * sina,
                            self.x * sina + self.y * cosa,
                            self.z)
        else:
            norm = axis / abs(axis)
            one_cosa = 1 - cosa
            return Vector3d(
                # x
                self.x * (norm.x * norm.x * one_cosa + cosa) +
                self.y * (norm.x * norm.y * one_cosa - norm.z * sina) +
                self.z * (norm.x * norm.z * one_cosa + norm.y * sina),
                # y
                self.x * (norm.y * norm.x * one_cosa + norm.z * sina) +
                self.y * (norm.y * norm.y * one_cosa + cosa) +
                self.z * (norm.y * norm.z * one_cosa - norm.x * sina),
                # z
                self.x * (norm.z * norm.x * one_cosa - norm.y * sina) +
                self.y * (norm.z * norm.y * one_cosa + norm.x * sina) +
                self.z * (norm.z * norm.z * one_cosa + cosa)
            )

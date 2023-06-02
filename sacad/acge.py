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

import numpy as np

from typing import Type, Union

from sacad.jsonify import Jsonify

__all__ = [
    'Vector2d',
    'Vector3d',
    'Number',
    'Matrix3d',
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
        return math.hypot(*self)

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

    def normalize(self) -> Vector:
        return self / abs(self)


class Vector2d(_VectorBase):
    def __new__(cls, x: Number = 0.0, y: Number = 0.0, _=0.0) -> 'Vector2d':
        return super().__new__(cls, (float(x), float(y)))

    def __matmul__(self, other: 'Vector2d') -> float:
        return self.x * other.y - other.x * self.y

    def rotate(self, degrees) -> 'Vector2d':
        return self.rotater(math.radians(degrees))

    def rotater(self, radians) -> 'Vector2d':
        cosa, sina = math.cos(radians), math.sin(radians)
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

    def rotate(self, degrees: Number, axis: 'Vector3d') -> 'Vector3d':
        return self.rotater(math.radians(degrees), axis)

    def rotater(self, radians: Number, axis: 'Vector3d') -> 'Vector3d':
        cosa, sina = math.cos(radians), math.sin(radians)
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


class Matrix3d(np.ndarray, Jsonify):
    def __new__(cls, *args, **kwargs):
        return super().__new__(cls, shape=(4, 4), buffer=(
            args[0] if isinstance(args[0], np.ndarray)
            else np.array(args, dtype=float)))

    def _jsonify_traverse_dict(self, self_dict):
        return list(self.flat)

    @staticmethod
    def identity() -> 'Matrix3d':
        return Matrix3d(np.identity(4))

    def move(self,
             offset_x: Number = 0,
             offset_y: Number = 0,
             offset_z: Number = 0) -> 'Matrix3d':
        transfer = Matrix3d(np.array((
            1, 0, 0, offset_x,
            0, 1, 0, offset_y,
            0, 0, 1, offset_z,
            0, 0, 0, 1,
        ), dtype=float))
        return Matrix3d(np.dot(transfer, self))

    def rotate(self, degrees: Number, axis: Vector3d = Vector3d.zaxis()):
        return self.rotater(math.radians(degrees), axis)

    def rotater(self, radians: Number, axis: Vector3d = Vector3d.zaxis()):
        cosa, sina = math.cos(radians), math.sin(radians)
        if axis == Vector3d.xaxis():
            rotation = Matrix3d(np.array((
                1, 0, 0, 0,
                0, cosa, -sina, 0,
                0, sina, cosa, 0,
                0, 0, 0, 1,
            ), dtype=float))
        elif axis == Vector3d.yaxis():
            rotation = Matrix3d(np.array((
                cosa, 0, sina, 0,
                0, 1, 0, 0,
                -sina, 0, cosa, 0,
                0, 0, 0, 1,
            ), dtype=float))
        elif axis == Vector3d.zaxis():
            rotation = Matrix3d(np.array((
                cosa, -sina, 0, 0,
                sina, cosa, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1,
            ), dtype=float))
        else:
            norm = axis / abs(axis)
            one_cosa = 1 - cosa
            rotation = Matrix3d(np.array((
                # row 0
                norm.x * norm.x * one_cosa + cosa,
                norm.x * norm.y * one_cosa - norm.z * sina,
                norm.x * norm.z * one_cosa + norm.y * sina,
                0,
                # row 1
                norm.x * norm.y * one_cosa + norm.z * sina,
                norm.y * norm.y * one_cosa + cosa,
                norm.y * norm.z * one_cosa - norm.x * sina,
                0,
                # row 2
                norm.x * norm.z * one_cosa - norm.y * sina,
                norm.y * norm.z * one_cosa + norm.x * sina,
                norm.z * norm.z * one_cosa + cosa,
                0,
                # row 3
                0, 0, 0, 1,
            ), dtype=float))
        return Matrix3d(np.dot(rotation, self))

    def scale(self, linear_factor: Number, mirror_x: bool = False,
              mirror_y: bool = False, mirror_z: bool = False):
        scale = Matrix3d(np.array((
            linear_factor * (-1 if mirror_x else 1), 0, 0, 0,
            0, linear_factor * (-1 if mirror_y else 1), 0, 0,
            0, 0, linear_factor * (-1 if mirror_z else 1), 0,
            0, 0, 0, 1,
        ), dtype=float))
        return Matrix3d(np.dot(scale, self))

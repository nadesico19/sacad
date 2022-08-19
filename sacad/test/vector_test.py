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

"""Unit test cases for `sacad.acge`."""

import math
import operator as op
import unittest

from itertools import islice
from random import uniform

from sacad.acge import Vector2d, Vector3d


def rand():
    while True:
        n = uniform(-1000.0, 1000.0)
        if n != 0.0:
            yield n


class Vector2dTestCase(unittest.TestCase):
    def setUp(self):
        self.nums = rand()

    def take(self, count):
        nums = [tuple(islice(self.nums, 2)) for _ in range(count)]
        return nums[0] if count == 1 else nums

    def test_type(self):
        nums = self.take(1)
        a = Vector2d(*nums)
        self.assertIsInstance(a, tuple)
        self.assertTrue(all(isinstance(i, float) for i in a))

    def test_value(self):
        nums = self.take(1)
        a = Vector2d(*nums)
        self.assertEqual(a, nums)
        self.assertEqual(a.x, nums[0])
        self.assertEqual(a.y, nums[1])

    def test_zero(self):
        a = Vector2d()
        self.assertEqual(a, (0.0, 0.0))
        self.assertEqual(abs(a), 0.0)

    def test_axis(self):
        self.assertEqual(Vector2d.xaxis(), (1.0, 0.0))
        self.assertEqual(Vector2d.yaxis(), (0.0, 1.0))

    def test_add(self):
        numsa, numsb = self.take(2)
        a = Vector2d(*numsa)
        b = Vector2d(*numsb)
        self.assertEqual(a + b, tuple(map(op.add, numsa, numsb)))

    def test_sub(self):
        numsa, numsb = self.take(2)
        a = Vector2d(*numsa)
        b = Vector2d(*numsb)
        self.assertEqual(a - b, tuple(map(op.sub, numsa, numsb)))

    def test_mul_vector(self):
        numsa, numsb = self.take(2)
        a = Vector2d(*numsa)
        b = Vector2d(*numsb)
        c = a * b
        self.assertAlmostEqual(c, sum(map(op.mul, numsa, numsb)))
        self.assertEqual(c, b * a)

    def test_mul_number(self):
        nums = self.take(1)
        num = next(self.nums)
        a = Vector2d(*nums)
        b = a * num
        self.assertEqual(b, tuple(n * num for n in a))
        self.assertEqual(b, num * a)

    def test_mat_mul(self):
        numsa = self.take(1)
        while (numsb := self.take(1)) == numsa:
            pass
        a = Vector2d(*numsa)
        b = Vector2d(*numsb)
        cosa = (a * b) / (abs(a) * abs(b))
        self.assertAlmostEqual(abs(a @ b),
                               abs(a) * abs(b) * math.sqrt(1 - cosa ** 2))
        self.assertAlmostEqual(a @ a, 0.0)

    def test_mat_mul_signum(self):
        nums = self.take(1)
        ang = uniform(1, 359)
        a = Vector2d(*nums)
        b = a.rotate(ang)
        c = a.rotate(-ang)
        d = a @ b
        e = a @ c
        self.assertNotAlmostEqual(d, e)
        self.assertAlmostEqual(d + e, 0.0)

    def test_div(self):
        nums = self.take(1)
        num = next(self.nums)
        a = Vector2d(*nums)
        b = a / num
        c = a * (1.0 / num)
        self.assertEqual(b, tuple(n / num for n in a))
        [self.assertAlmostEqual(i, j) for i, j in zip(b, c)]

    def test_neg(self):
        nums = self.take(1)
        a = Vector2d(*nums)
        b = -a
        self.assertEqual(a + b, (0, 0))

    def test_abs(self):
        nums = self.take(1)
        a = abs(Vector2d(*nums))
        self.assertAlmostEqual(a, math.sqrt(sum(n * n for n in nums)))

    def test_rotate_angle_radian(self):
        nums = self.take(1)
        ang = uniform(1.0, 359.0)
        a = Vector2d(*nums)
        b = a.rotate(ang)
        c = a.rotater(math.pi * ang / 180.0)
        [self.assertAlmostEqual(i, j) for i, j in zip(b, c)]

    def test_rotate(self):
        nums = self.take(1)
        ang = uniform(1.0, 359.0)
        a = Vector2d(*nums)
        b = a.rotater(ang)
        cosa = (a * b) / (abs(a) * abs(b))
        self.assertAlmostEqual(math.cos(ang), cosa)


class Vector3dTestCase(unittest.TestCase):
    def setUp(self):
        self.nums = rand()

    def take(self, count):
        nums = [tuple(islice(self.nums, 3)) for _ in range(count)]
        return nums[0] if count == 1 else nums

    def test_type(self):
        nums = self.take(1)
        a = Vector3d(*nums)
        self.assertIsInstance(a, tuple)
        self.assertTrue(all(isinstance(i, float) for i in a))

    def test_value(self):
        nums = self.take(1)
        a = Vector3d(*nums)
        self.assertEqual(a, nums)
        self.assertEqual(a.x, nums[0])
        self.assertEqual(a.y, nums[1])
        self.assertEqual(a.z, nums[2])

    def test_zero(self):
        a = Vector3d()
        self.assertEqual(a, (0.0, 0.0, 0.0))
        self.assertEqual(abs(a), 0.0)

    def test_axis(self):
        self.assertEqual(Vector3d.xaxis(), (1.0, 0.0, 0.0))
        self.assertEqual(Vector3d.yaxis(), (0.0, 1.0, 0.0))
        self.assertEqual(Vector3d.zaxis(), (0.0, 0.0, 1.0))

    def test_add(self):
        numsa, numsb = self.take(2)
        a = Vector3d(*numsa)
        b = Vector3d(*numsb)
        self.assertEqual(a + b, tuple(map(op.add, numsa, numsb)))

    def test_sub(self):
        numsa, numsb = self.take(2)
        a = Vector3d(*numsa)
        b = Vector3d(*numsb)
        self.assertEqual(a - b, tuple(map(op.sub, numsa, numsb)))

    def test_mul_vector(self):
        numsa, numsb = self.take(2)
        a = Vector3d(*numsa)
        b = Vector3d(*numsb)
        c = a * b
        self.assertAlmostEqual(c, sum(map(op.mul, numsa, numsb)))
        self.assertEqual(c, b * a)

    def test_mul_number(self):
        nums = self.take(1)
        num = next(self.nums)
        a = Vector3d(*nums)
        b = a * num
        self.assertEqual(b, tuple(n * num for n in a))
        self.assertEqual(b, num * a)

    def test_mat_mul(self):
        numsa = self.take(1)
        while (numsb := self.take(1)) == numsa:
            pass
        a = Vector3d(*numsa)
        b = Vector3d(*numsb)
        cosa = (a * b) / (abs(a) * abs(b))
        self.assertAlmostEqual(abs(a @ b),
                               abs(a) * abs(b) * math.sqrt(1 - cosa ** 2))
        self.assertAlmostEqual(abs(a @ a), 0.0)

    def test_mat_mul_direction(self):
        nums = self.take(1)
        ang = uniform(1, 359)
        a = Vector3d(*nums)
        rot_axis = Vector3d(-a.y, a.x, 0)
        b = a.rotate(ang, rot_axis)
        c = a.rotate(-ang, rot_axis)
        d = a @ b
        e = a @ c
        self.assertNotAlmostEqual(abs(d), 0.0)
        self.assertAlmostEqual(abs(d + e), 0.0)

    def test_div(self):
        nums = self.take(1)
        num = next(self.nums)
        a = Vector3d(*nums)
        b = a / num
        c = a * (1.0 / num)
        self.assertEqual(b, tuple(n / num for n in a))
        [self.assertAlmostEqual(i, j) for i, j in zip(b, c)]

    def test_neg(self):
        nums = self.take(1)
        a = Vector3d(*nums)
        b = -a
        self.assertEqual(a + b, (0, 0, 0))

    def test_abs(self):
        nums = self.take(1)
        a = abs(Vector3d(*nums))
        self.assertAlmostEqual(a, math.sqrt(sum(n * n for n in nums)))

    def test_rotate_angle_radian(self):
        pass

    def test_rotate(self):
        pass


if __name__ == '__main__':
    unittest.main()

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

import math
import time
import sacad as ac

from typing import Iterable


def main(cad: ac.Acad):
    trans = cad.db_insert(zoom_mode=ac.ZoomMode.ADDED, zoom_factor=1.5)

    trans.model_space.insert_many(draw_golden_section_arcs())

    return trans.submit()


def draw_golden_section_arcs(center: ac.Vector2d = ac.Vector2d(),
                             direction: ac.Vector2d = ac.Vector2d(100),
                             num: int = 10,
                             tolerance: float = 1e-6):
    for i in range(num):
        yield draw_simple_polygon(gen_rect_vertices(center, direction))

        radius = abs(direction)
        normal_dir = direction.normalize()

        cross_prod = ac.Vector2d.xaxis() @ direction.normalize()
        dot_prod = ac.Vector2d.xaxis() * direction.normalize()

        start_angle = math.acos(dot_prod) * (
            cross_prod / abs(cross_prod) if abs(cross_prod) > tolerance else 1)

        yield ac.Arc.new_vec2(center, radius, start_angle,
                              start_angle + math.pi / 2)

        next_radius = radius * (math.sqrt(1.25) - 0.5)
        next_normal_dir = normal_dir.rotate(90)

        direction = next_normal_dir * next_radius
        center += next_normal_dir * (radius - next_radius)


def draw_simple_polygon(vertices: Iterable[ac.Vector2d]):
    return ac.Polyline.new(*(ac.Vertex.new(v.x, v.y) for v in vertices),
                           closed=True)


def gen_rect_vertices(start_pos: ac.Vector2d, direction: ac.Vector2d):
    pos = start_pos
    for _ in range(4):
        yield pos
        pos += direction
        direction = direction.rotate(90)


if __name__ == '__main__':
    with ac.instant_acad() as _cad:
        _start_at = time.perf_counter()
        _result = main(_cad)
        _stop_at = time.perf_counter()
        print(f'Done in {_stop_at - _start_at:0.3f} seconds.',
              f'Result: {_result}.')

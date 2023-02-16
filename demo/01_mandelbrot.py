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

import itertools
import time
import sacad as ac


def main(cad: ac.Acad):
    width, height, depth = 300, 200, 32
    trans = cad.db_insert(zoom_mode=ac.ZoomMode.ADDED, zoom_scale=1.5)

    trans.layertable.insert_many(
        ac.LayerTableRecord(name=f'LAYER{i}', color=ac.Color.rgb(j, j, j))
        for i, j in map(lambda k: (k, k * 8), range(depth)))

    for x, y in itertools.product(range(width), range(height)):
        xn, yn = xc, yc = float(x) / width * 3 - 2, float(y) / height * 2 - 1
        for i in range(depth):
            if xn ** 2 + yn ** 2 > 4:
                trans.modelspace.insert(ac.Polyline(
                    vertices=[ac.Vertex(ac.Vector2d(x - 0.25, y), bulge=1),
                              ac.Vertex(ac.Vector2d(x + 0.25, y), bulge=1)],
                    closed=True, constant_width=0.5, layer=f'LAYER{i}'))
                break
            xn, yn = xn ** 2 - yn ** 2 + xc, 2 * xn * yn + yc

    return trans.submit()


if __name__ == '__main__':
    with ac.instant_acad() as _cad:
        _start_at = time.perf_counter()
        _result = main(_cad)
        _stop_at = time.perf_counter()
        print(f'Done in {_stop_at - _start_at:0.3f} seconds.',
              f'Result: {_result}.')

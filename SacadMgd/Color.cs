/* Copyright (c) 2022 Chin Ako <nadesico19@gmail.com>
 * sacad is licensed under Mulan PubL v2.
 * You can use this software according to the terms and conditions of the Mulan PubL v2.
 * You may obtain a copy of Mulan PubL v2 at:
 *          http://license.coscl.org.cn/MulanPubL-2.0
 * THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND,
 * EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,
 * MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
 * See the Mulan PubL v2 for more details.
 */

using AcCm = Autodesk.AutoCAD.Colors;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType(Name = "sacad.accm.Color")]
    public class Color : PyObject
    {
        public ColorMethod? color_method;

        public int? red;
        public int? green;
        public int? blue;

        public short? color_index;

        public AcCm.Color ToArx()
        {
            var cm = color_method ?? ColorMethod.None;

            return cm == ColorMethod.ByColor
                ? AcCm.Color.FromRgb((byte)(red ?? 0), (byte)(green ?? 0), (byte)(blue ?? 0))
                : AcCm.Color.FromColorIndex((AcCm.ColorMethod)cm, color_index ?? 0);
        }
    }
}
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

using System;
using AcCm = Autodesk.AutoCAD.Colors;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType("sacad.accm.Color")]
    public sealed class Color
    {
        public AcCm.ColorMethod? color_method;

        public int? red;
        public int? green;
        public int? blue;

        public short? color_index;

        public AcCm.Color ToArx()
        {
            var cm = color_method ?? AcCm.ColorMethod.None;

            return cm == AcCm.ColorMethod.ByColor
                ? AcCm.Color.FromRgb(
                    (byte)(red ?? 0), (byte)(green ?? 0), (byte)(blue ?? 0))
                : AcCm.Color.FromColorIndex(Ensure(cm), color_index ?? 0);
        }

        public static Color FromArx(AcCm.Color color)
        {
            var result = new Color { color_method = color.ColorMethod };

            if (color.ColorMethod == AcCm.ColorMethod.ByColor)
            {
                result.red = color.Red;
                result.green = color.Green;
                result.blue = color.Blue;
            }
            else
            {
                result.color_index = color.ColorIndex;
            }

            return result;
        }

        private static AcCm.ColorMethod Ensure(AcCm.ColorMethod method) =>
            Enum.IsDefined(typeof(AcCm.ColorMethod), method)
                ? method
                : AcCm.ColorMethod.None;
    }
}
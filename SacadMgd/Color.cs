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
        public int r;
        public int g;
        public int b;

        // TODO indexed color

        public AcCm.Color ToArx()
        {
            return AcCm.Color.FromRgb((byte)r, (byte)g, (byte)b);
        }
    }
}
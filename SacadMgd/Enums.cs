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

namespace SacadMgd
{
    public enum ColorMethod
    {
        ByAci = 0xc3,
        ByBlock = 0xc1,
        ByColor = 0xc2,
        ByLayer = 0xc0,
        ByPen = 0xc4,
        Foreground = 0xc5,
        LayerFrozen = 0xc7,
        LayerOff = 0xc6,
        None = 200,
    }

    public enum LineWeight
    {
        ByBlock = -2,
        ByLayer = -1,
        ByLineWeightDefault = -3,
        LineWeight000 = 0,
        LineWeight005 = 5,
        LineWeight009 = 9,
        LineWeight013 = 13,
        LineWeight015 = 15,
        LineWeight018 = 0x12,
        LineWeight020 = 20,
        LineWeight025 = 0x19,
        LineWeight030 = 30,
        LineWeight035 = 0x23,
        LineWeight040 = 40,
        LineWeight050 = 50,
        LineWeight053 = 0x35,
        LineWeight060 = 60,
        LineWeight070 = 70,
        LineWeight080 = 80,
        LineWeight090 = 90,
        LineWeight100 = 100,
        LineWeight106 = 0x6a,
        LineWeight120 = 120,
        LineWeight140 = 140,
        LineWeight158 = 0x9e,
        LineWeight200 = 200,
        LineWeight211 = 0xd3
    }
}
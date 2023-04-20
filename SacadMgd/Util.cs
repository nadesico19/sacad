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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using AcAp = Autodesk.AutoCAD.ApplicationServices;

namespace SacadMgd
{
    public static class Util
    {
        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                    { NullValueHandling = NullValueHandling.Ignore });
        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json,
                new JsonSerializerSettings
                    { TypeNameHandling = TypeNameHandling.Auto });
        }

        public static void ConsoleWriteLine(object content)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var editor = AcAp.Application.DocumentManager.MdiActiveDocument
                .Editor;
            editor.WriteMessage($"\n{content}");
        }

        public static T? ToOptional<T>(T value) where T : struct
            => EqualityComparer<T>.Default.Equals(value, default(T))
                ? (T?)null
                : value;

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hwnd);
    }
}
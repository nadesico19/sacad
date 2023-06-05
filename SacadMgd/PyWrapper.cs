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
using System.Linq;
using System.Reflection;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    public sealed class PyWrapper<T>
    {
        public string __cls__;
        public T __mbr__;

        internal PyWrapper()
        {
        }

        public static PyWrapper<T> Create(T pyObject)
        {
            return new PyWrapper<T>
            {
                __cls__ = Python.PyTypes[pyObject.GetType()],
                __mbr__ = pyObject,
            };
        }
    }

    public static class Python
    {
        internal static readonly Dictionary<Type, string> PyTypes =
            new Dictionary<Type, string>();

        internal static void RegisterAll()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var pyType in asm.GetTypes().Where(t =>
                         t.GetCustomAttribute<PyTypeAttribute>() != null))
            {
                PyTypes[pyType] =
                    pyType.GetCustomAttribute<PyTypeAttribute>().Name;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class PyTypeAttribute : Attribute
    {
        public readonly string Name;

        public PyTypeAttribute(string name)
        {
            Name = name;
        }
    }
}
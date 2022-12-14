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

using Newtonsoft.Json;

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    [PyType(Name = "sacad.result.Result")]
    public class Result : PyObject
    {
        [JsonIgnore] public const int FAILURE = 0;
        [JsonIgnore] public const int SUCCESS = 1;

        public int status;
        public string message;
    }

    [PyType(Name = "sacad.result.DBInsertResult")]
    public class DbInsertResult : Result
    {
        public int num_inserted;
        public int num_updated;
    }
}
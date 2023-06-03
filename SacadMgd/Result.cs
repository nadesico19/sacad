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

// ReSharper disable InconsistentNaming

namespace SacadMgd
{
    public enum Status
    {
        Unknown,
        Success,
        Failure,
        Warning,
    }

    [PyType(Name = "sacad.result.Result")]
    public class Result
    {
        public Status status;
        public string message;
    }

    [PyType(Name = "sacad.result.DBInsertResult")]
    public sealed class DbInsertResult : Result
    {
        public int num_inserted;
        public int num_updated;
        public int num_failure;
    }

    [PyType(Name = "sacad.result.DBSelectResult")]
    public sealed class DbSelectResult : Result
    {
        public PyWrapper<Database> db;
    }
}
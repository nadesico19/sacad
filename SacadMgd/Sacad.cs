﻿/* Copyright (c) 2022 Chin Ako <nadesico19@gmail.com>
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
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using AcAp = Autodesk.AutoCAD.ApplicationServices;
using AcEi = Autodesk.AutoCAD.EditorInput;
using AcRt = Autodesk.AutoCAD.Runtime;

[assembly: AcRt.ExtensionApplication(typeof(SacadMgd.Sacad))]
[assembly: AcRt.CommandClass(typeof(SacadMgd.Sacad))]

namespace SacadMgd
{
    public sealed class Sacad : AcRt.IExtensionApplication
    {
        public void Initialize()
        {
            _connKeeper = new Dictionary<string, TcpClient>();

            Entity.RegisterALl();
            Python.RegisterAll();
        }

        public void Terminate()
        {
        }

        [AcRt.CommandMethod("SACAD_CONNECT", AcRt.CommandFlags.Redraw)]
        public static void ConnectCommand()
        {
            try
            {
                RemoveDeadConnections();

                var client = PromptClientInfo("connect");

                TcpClient conn;
                if (_connKeeper.TryGetValue(client.Skey, out conn))
                    conn.Close();

                _connKeeper[client.Skey] =
                    new TcpClient(client.Host, client.Port) { NoDelay = true };
            }
            catch (Exception ex)
            {
                Util.ConsoleWriteLine($"[sacad connect] error: {ex}");
            }
        }

        [AcRt.CommandMethod("SACAD_PING", AcRt.CommandFlags.Redraw)]
        public static void PingCommand()
        {
            try
            {
                var netStream = GetNetStream("ping");

                if (ReceiveMessage(netStream) != "ping")
                {
                    throw new InvalidDataException(
                        $"Wrong ping message \"{netStream}\".");
                }

                SendMessage(netStream, "pong");
            }
            catch (Exception ex)
            {
                Util.ConsoleWriteLine($"[sacad ping] error: {ex}");
            }
        }

        [AcRt.CommandMethod("SACAD_DOCOP",
            AcRt.CommandFlags.Session | AcRt.CommandFlags.Redraw)]
        public static void DocOperationCommand()
        {
            DoOperationCommand("doc operation", message => new Result());
            // TODO
        }

        [AcRt.CommandMethod("SACAD_DBOP",
            AcRt.CommandFlags.DocExclusiveLock | AcRt.CommandFlags.Redraw)]
        public static void DbOperationCommand()
        {
            DoOperationCommand("db operation", message =>
                Util.Deserialize<PyWrapper<DbQuery>>(message).__mbr__
                    .Execute());
        }

        private static void DoOperationCommand(string cmdTitle,
            Func<string, Result> opFunc)
        {
            NetworkStream netStream = null;

            try
            {
                netStream = GetNetStream(cmdTitle);
                var request = ReceiveMessage(netStream);
                var result = opFunc.Invoke(request);
                var response = Util.Serialize(PyWrapper<Result>.Create(result));
                SendMessage(netStream, response);
            }
            catch (Exception ex)
            {
                Util.ConsoleWriteLine($"[sacad {cmdTitle}] error: {ex}");

                if (netStream != null)
                    try
                    {
                        var result = PyWrapper<Result>.Create(new Result
                        {
                            status = Status.Unknown,
                            message = $"Unhandled exception: {ex.Message}"
                        });
                        SendMessage(netStream, Util.Serialize(result));
                    }
                    catch
                    {
                        // ignored
                    }
            }
        }

        private static ClientInfo PromptClientInfo(string cmdName)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var editor = AcAp.Application.DocumentManager.MdiActiveDocument
                .Editor;

            var option = new AcEi.PromptStringOptions(string.Empty)
                { AllowSpaces = false };

            option.Message = $"\n[sacad {cmdName}] host info: ";
            var hostInput = editor.GetString(option);
            if (hostInput.Status != AcEi.PromptStatus.OK)
                throw new InvalidOperationException("Wrong PromptStatus.");

            option.Message = $"\n[sacad {cmdName}] session key: ";
            var skeyInput = editor.GetString(option);
            if (skeyInput.Status != AcEi.PromptStatus.OK)
                throw new InvalidOperationException("Wrong PromptStatus.");

            var hostPort = hostInput.StringResult.Split(':');
            return new ClientInfo
            {
                Skey = skeyInput.StringResult,
                Host = hostPort[0],
                Port = int.Parse(hostPort[1])
            };
        }

        private static string PromptSkey(string cmdName)
        {
            // ReSharper disable once AccessToStaticMemberViaDerivedType
            var editor = AcAp.Application.DocumentManager.MdiActiveDocument
                .Editor;

            var option = new AcEi.PromptStringOptions(
                $"\n[sacad {cmdName}] session key: ") { AllowSpaces = false };

            var skeyInput = editor.GetString(option);
            if (skeyInput.Status != AcEi.PromptStatus.OK)
                throw new InvalidOperationException("Wrong PromptStatus.");

            return skeyInput.StringResult;
        }

        private static NetworkStream GetNetStream(string cmdInfo)
        {
            var skey = PromptSkey(cmdInfo);
            if (!_connKeeper.ContainsKey(skey))
            {
                throw new InvalidOperationException(
                    "No connection established.");
            }

            var netStream = _connKeeper[skey].GetStream();

            // TODO
            netStream.ReadTimeout = 10000;
            netStream.WriteTimeout = 10000;

            return netStream;
        }

        private static string ReceiveMessage(NetworkStream netStream)
        {
            if (!netStream.CanRead)
            {
                throw new InvalidOperationException(
                    "NetworkStream cannot read.");
            }

            int msgLen;
            using (var memStream = new MemoryStream())
            {
                int b;
                while (true)
                {
                    b = netStream.ReadByte();
                    if (b == -1)
                    {
                        throw new EndOfStreamException(
                            "EOF while reading body length.");
                    }

                    if (b == LineBreak) break;
                    memStream.WriteByte((byte)b);
                }

                if (b != LineBreak)
                {
                    throw new InvalidDataException(
                        "Fail to read body length line.");
                }

                msgLen = int.Parse(Encoding.UTF8.GetString(
                    memStream.ToArray()));
            }

            using (var memStream = new MemoryStream())
            {
                do
                {
                    var readNum = netStream.Read(ReadBuf, 0, ReadBuf.Length);
                    memStream.Write(ReadBuf, 0, readNum);
                    msgLen -= readNum;
                } while (msgLen > 0);

                return Encoding.UTF8.GetString(memStream.ToArray());
            }
        }

        private static void SendMessage(NetworkStream netStream, string msg)
        {
            if (!netStream.CanWrite)
            {
                throw new InvalidOperationException(
                    "NetworkStream cannot write.");
            }

            var bytes = Encoding.UTF8.GetBytes(msg);
            var lenBytes = Encoding.UTF8.GetBytes($"{bytes.Length}\n");

            netStream.Write(lenBytes, 0, lenBytes.Length);
            netStream.Write(bytes, 0, bytes.Length);
        }

        private static void RemoveDeadConnections()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnections = ipProperties.GetActiveTcpConnections();

            var toBeRemove = _connKeeper
                .Where(conn => !tcpConnections.Any(
                    activeConn =>
                        activeConn.LocalEndPoint.Equals(conn.Value.Client
                            .LocalEndPoint) &&
                        activeConn.RemoteEndPoint.Equals(conn.Value.Client
                            .RemoteEndPoint)))
                .ToArray();

            foreach (var conn in toBeRemove) _connKeeper.Remove(conn.Key);
        }

        private class ClientInfo
        {
            /// <summary>
            /// Session key.
            /// </summary>
            public string Skey;

            public string Host;
            public int Port;
        }

        private static readonly int LineBreak =
            Encoding.UTF8.GetBytes("\n").First();

        private static readonly byte[] ReadBuf = new byte[4096];
        private static Dictionary<string, TcpClient> _connKeeper;
    }
}
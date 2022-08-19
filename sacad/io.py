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

import asyncio

from asyncio import Future, StreamReader, StreamWriter
from queue import SimpleQueue
from threading import Thread
from typing import Callable, Optional

from sacad.error import AcadTcpError

__all__ = ['Requester']


class Requester:
    def __init__(self, host: str, port: int):
        self._host = host
        self._port = port

        self._loop = asyncio.new_event_loop()
        self._thread = Thread(target=self._loop.run_forever)

        self._reader: Optional[StreamReader] = None
        self._writer: Optional[StreamWriter] = None

        self._thread.start()

    @property
    def host(self):
        return self._host

    @property
    def port(self):
        return self._port

    def connect(self, on_listening: Optional[Callable] = None):
        chan = SimpleQueue()

        try:
            server = asyncio.run_coroutine_threadsafe(
                asyncio.start_server(
                    lambda sr, sw: chan.put((sr, sw)),
                    self._host, self._port), self._loop
            ).result()
        except Exception as e:
            raise AcadTcpError from e

        if callable(on_listening):
            on_listening()

        self._reader, self._writer = chan.get()
        server.close()

    def request(self, msg: str, encoding='utf-8') -> Future:
        if self.is_disconnected():
            raise AcadTcpError

        return asyncio.run_coroutine_threadsafe(
            self._request(msg, encoding), self._loop)

    def disconnect(self):
        asyncio.run_coroutine_threadsafe(
            self._disconnect(), self._loop).result()

    def close(self):
        asyncio.run_coroutine_threadsafe(self._stop(), self._loop)

        self._thread.join()
        self._loop.close()

        self._reader = self._writer = None

    def is_closed(self):
        return not self._thread.is_alive()

    def is_disconnected(self):
        return not self._writer or self._writer.is_closing()

    async def _request(self, msg, encoding='utf-8'):
        request = msg.encode(encoding)

        try:
            self._writer.writelines([f'{len(request)}\n'.encode(), request])
            await self._writer.drain()

            num_bytes = await self._reader.readline()
            response = await self._reader.readexactly(int(num_bytes))

        except Exception as e:
            raise AcadTcpError from e

        return response.decode(encoding)

    async def _disconnect(self):
        if self._writer and not self._writer.is_closing():
            self._writer.close()
            await self._writer.wait_closed()

    async def _stop(self):
        await self._disconnect()

        for task in asyncio.all_tasks(self._loop):
            task.cancel()

        self._loop.stop()

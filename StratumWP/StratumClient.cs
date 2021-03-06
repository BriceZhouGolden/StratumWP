﻿using StratumWP.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace StratumWP
{
    public class StratumClient
    {
        private DnsEndPoint[] servers;

        private Socket socket;

        private long lastId;

        private Dictionary<long, TaskCompletionSource<ResultMessage>> callers;
        private Dictionary<long, SubscribeResult> subscribes;

        private byte[] recBuffer;

        public StratumClient(IEnumerable<DnsEndPoint> servers)
            : this(servers.ToArray()) { }

        public StratumClient(params DnsEndPoint[] servers)
        {
            this.servers = servers;

            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            callers = new Dictionary<long, TaskCompletionSource<ResultMessage>>();
            subscribes = new Dictionary<long, SubscribeResult>();

            recBuffer = new byte[4 * 1024];
        }

        public Task<SocketError> ConnectAsync()
        {
            var tcs = new TaskCompletionSource<SocketError>();

            // TODO use random, exponentially backoff from failed connections
            var connectArgs = new SocketAsyncEventArgs() { RemoteEndPoint = servers[0] };
            EventHandler<SocketAsyncEventArgs> completed = (s, ea) =>
            {
                recieveMessage();
                tcs.SetResult(ea.SocketError);
            };
            connectArgs.Completed += completed;

            if (!socket.ConnectAsync(connectArgs))
                completed(null, connectArgs);

            return tcs.Task;
        }

        public void TriggerShutdown()
        {
            try { socket.Close(); }
            catch { }

            foreach (var tcs in callers.Values)
                tcs.SetCanceled();
            subscribes.Clear();
        }

        private void sendToServer(string str)
        {
            var sendArgs = new SocketAsyncEventArgs();
            var payload = Encoding.UTF8.GetBytes(str);
            sendArgs.SetBuffer(payload, 0, payload.Length);
            socket.SendAsync(sendArgs);
        }

        public Task<ResultMessage> Call(CallMessage call)
        {
            lock (this)
                call.Id = lastId++;

            sendToServer(call.ToString());

            var tcs = new TaskCompletionSource<ResultMessage>();
            lock (this)
                callers[call.Id] = tcs;

            return tcs.Task;
        }

        public void Subscibe(CallMessage call, SubscribeResult handler)
        {
            lock (this)
                call.Id = lastId++;

            sendToServer(call.ToString());

            var tcs = new TaskCompletionSource<ResultMessage>();
            lock (this)
                subscribes[call.Id] = handler;
        }

        private void recieveMessage()
        {
            var recArgs = new SocketAsyncEventArgs();
            recArgs.SetBuffer(recBuffer, 0, recBuffer.Length);
            EventHandler<SocketAsyncEventArgs> completed = (s, ea) =>
            {
                if (ea.SocketError == SocketError.Success)
                {
                    var resp = Encoding.UTF8.GetString(recBuffer, 0, ea.BytesTransferred);
                    var msg = new ResultMessage(resp);

                    handleMessage(msg);
                }

                recieveMessage();
            };
            recArgs.Completed += completed;

            if (!socket.ReceiveAsync(recArgs))
                completed(null, recArgs);
        }

        private void handleMessage(ResultMessage result)
        {
            if (callers.ContainsKey(result.Id))
            {
                callers[result.Id].SetResult(result);
                callers.Remove(result.Id);
            }
            else if (subscribes.ContainsKey(result.Id))
                Task.Run(() => subscribes[result.Id](result));
        }
    }

    public delegate void SubscribeResult(ResultMessage message);
}

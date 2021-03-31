﻿using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Muziekspeler.Server
{
    public class Server
    {
        public List<UserConnection> Clients;
        public Dictionary<string, Room> Rooms;
        private CancellationTokenSource cancellation;
        private TcpListener listener;
        private int idCounter = 1;

        public Server(CancellationTokenSource cts)
        {
            cancellation = cts;
            listener = new TcpListener(IPAddress.Parse("0.0.0.0"),5069);
        }

        public async Task StartServerLoopAsync()
        {
            listener.Start();
            Console.WriteLine("Server up n running!");
            while (!cancellation.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                var connection = new UserConnection(client);
                await connection.SendId(idCounter);
                idCounter++; // just increase the ID counter by one to ensure unique IDs. No fancy ID reservation system beyond that.
                Clients.Add(connection);
            }
        }

        public async Task TickLoop()
        {
            List<UserConnection> deadclients = new List<UserConnection>();
            while (!cancellation.IsCancellationRequested)
            {
                foreach(var u in Clients)
                {
                    await u.KeepAliveAsync();
                    if (u.MissedKeepalives >= 5)
                    {
                        deadclients.Add(u);
                        u.Disconnect();
                    }
                }
                Clients.RemoveAll(x => deadclients.Contains(x));
                deadclients.Clear();

                await Task.Delay(2500);
            }
        }
    }
}

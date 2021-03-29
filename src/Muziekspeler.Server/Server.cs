using Muziekspeler.Common.Types;
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
            }
        }
    }
}

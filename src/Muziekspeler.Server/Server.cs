using Muziekspeler.Common.Packets;
using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<ServerRoom> Rooms;
        private CancellationTokenSource cancellation;
        private TcpListener listener;
        private int idCounter = 1;

        public Server(CancellationTokenSource cts)
        {
            Clients = new List<UserConnection>();
            Rooms = new List<ServerRoom>();
            cancellation = cts;
            listener = new TcpListener(IPAddress.Parse("0.0.0.0"), 5069);
        }

        public async Task StartServerLoopAsync()
        {
            listener.Start();
            Console.WriteLine("Server up n running!");
            _ = Task.Run(async () => await TickLoop());
            while (!cancellation.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Accepted a client!");
                var connection = new UserConnection(client, this);
                connection.StartClientLoop();
                await connection.SendId(idCounter);
                idCounter++; // just increase the ID counter by one to ensure unique IDs. No fancy ID reservation system beyond that.
                Clients.Add(connection);
            }
        }

        public async Task JoinRoom(UserConnection connection, JoinRoomData data)
        {
            if (!this.Rooms.Any(x => x.Name == data.RoomName))
            {
                await connection.SendPacketAsync(new Packet(PacketType.Fail, new ReasonData() { Reason = "Room no longer exists!" }));
                return;
            }

            var room = Rooms.First(x => x.Name == data.RoomName);
            room.Users.Add(connection.GetUser());

            connection.SetRoom(room);

            await connection.SendPacketAsync(new Packet(PacketType.JoinRoom, null));
            await Task.Delay(1500);
            await sendRoomUpdate(room);
        }

        public async Task CreateRoomAndJoin(UserConnection connection, CreateRoomData data)
        {
            if(this.Rooms.Any(x => x.Name == data.Name))
            {
                await connection.SendPacketAsync(new Packet(PacketType.Fail, new ReasonData() { Reason = "Room name already taken!" }));
                return;
            }

            var room = new ServerRoom(this);
            room.Name = data.Name;
            room.HostUserId = connection.GetUser().Id;
            room.Users.Add(connection.GetUser());
            connection.SetRoom(room);
            this.Rooms.Add(room);

            await connection.SendPacketAsync(new Packet(PacketType.JoinRoom, null));
            await Task.Delay(1500);
            await sendRoomUpdate(room);
        }

        public async Task sendRoomUpdate(ServerRoom room)
        {
            await room.BroadcastPacketAsync(new Packet(PacketType.RoomUpdate, new RoomUpdateData()
            {
                HostId = room.HostUserId,
                Queue = room.SongQueue,
                Users = room.Users
            }));
        }

        public async Task BroadcastRoomAsync(ServerRoom room, Packet packet)
        {
            await Task.Yield();

            foreach (var connection in Clients.Where(x => room.Users.Select(y => y.Id).Contains(x.GetUser().Id)))
            {
                _ = Task.Run(async () => await connection.SendPacketAsync(packet));
            }
        }

        //public async Task BroadcastRoomDataAsync(ServerRoom room, byte[] data)
        //{
        //    await Task.Yield();

        //    foreach (var connection in Clients.Where(x => room.Users.Contains(x.GetUser())))
        //    {
        //        // Could make this into a Parallel.ForEach?
        //        _ = Task.Run(async () => await connection.SendDataAsync(data));
        //    }
        //}

        public async Task SendPacketToUserAsync(int userId, Packet packet)
        {
            var client = this.Clients.FirstOrDefault(x => x.GetUser().Id == userId);

            if(client != null)
            {
                await client.SendPacketAsync(packet);
            }
        }

        public async Task TickLoop()
        {
            List<UserConnection> deadclients = new List<UserConnection>();
            while (!cancellation.IsCancellationRequested)
            {
                if (this.Clients.Count() > 0)
                {
                    Console.WriteLine("Broadcasting KeepAlives.");
                    // quick linq query to remove empty rooms
                    Rooms.RemoveAll(x => x.Users.Count <= 0);

                    foreach (var r in Rooms)
                    {
                        if (!r.Users.Any(x => x.Id == r.HostUserId))
                        {
                            r.HostUserId = r.Users.First().Id;
                            await sendRoomUpdate(r);
                        }

                        r.Users.RemoveAll(x => !this.Clients.Any(y => y.GetUser().Id == x.Id));
                    }

                    foreach (var u in Clients)
                    {
                        await u.KeepAliveAsync();
                        if (u.MissedKeepalives >= 5)
                        {
                            Console.WriteLine($"client with uid {u.GetUser().Id} died.");
                            deadclients.Add(u);
                            u.Disconnect();
                        }
                    }
                    Clients.RemoveAll(x => deadclients.Contains(x));
                    deadclients.Clear();
                } //do nothing when no clients

                await Task.Delay(1000);
            }
        }
    }
}

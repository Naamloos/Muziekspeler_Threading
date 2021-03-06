using Muziekspeler.Common;
using Muziekspeler.Common.Packets;
using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.Server
{
    public class UserConnection
    {
        private Connection connection;
        private User user = new User();
        private ServerRoom room;
        private Server server;
        public int MissedKeepalives = 0;

        public UserConnection(TcpClient client, Server server)
        {
            connection = new Connection(client, handlePacketAsync);
            this.server = server;
        }

        public void StartClientLoop() => connection.StartClientLoop();

        public User GetUser()
        {
            return this.user;
        }

        public ServerRoom GetRoom() => room;
        public void SetRoom(ServerRoom room)
        {
            this.room = room;
        }

        public async Task SendPacketAsync(Packet packet)
        {
            await this.connection.SendPacketAsync(packet);
        }

        public async Task SendRoomList()
        {
            await this.SendPacketAsync(new Packet(PacketType.RoomList, new RoomListData() { RoomNames = this.server.Rooms.Select(x => x.Name).ToList() }));
        }

        public async Task SendDataAsync(byte[] data)
        {
            await this.connection.SendDataAsync(data);
        }

        public async Task KeepAliveAsync()
        {
            this.MissedKeepalives++;
            await connection.SendPacketAsync(new Packet(PacketType.KeepAlive, null));
        }

        public async Task SendId(int id)
        {
            await connection.SendPacketAsync(new Packet(PacketType.UserId, new UserIdData() { Id = id }));
            this.user.Id = id;
        }

        public void Disconnect()
        {
            connection.StopClientLoop();
        }

        private void handleUserData(SetUserData data)
        {
            this.user.DisplayName = data.DisplayName;
            this.user.Status = data.Status;
        }

        private async Task handlePacketAsync(Packet packet) // TODO add behavior
        {
            object data = null;
            switch (packet.Type)
            {
                default:
                    throw new NotImplementedException("Packet type has no suitable handler!");

                case PacketType.ChatMessage:
                    data = packet.Data.ToObject<ChatMessageData>();
                    await room.HandleChatAsync($"{user.DisplayName}: {((ChatMessageData)data).Message}");
                    break;

                case PacketType.RoomList:
                    await SendRoomList();
                    break;

                case PacketType.JoinRoom:
                    await server.JoinRoom(this, packet.Data.ToObject<JoinRoomData>());
                    break;

                case PacketType.LeaveRoom:
                    // Has no data
                    room.Users.Remove(this.user);
                    await server.sendRoomUpdate(room);
                    break;

                case PacketType.RoomUpdate:
                    data = packet.Data.ToObject<RoomUpdateData>();
                    break;

                case PacketType.KeepAlive:
                    // Just a keepalive, needs no data
                    this.MissedKeepalives = 0;
                    Console.WriteLine($"Kept alive user {user.Id}");
                    break;

                case PacketType.PlayMusic:
                    // Just indicates a play command
                    break;

                case PacketType.PauseMusic:
                    // Just indicates pause command
                    break;

                case PacketType.ClearQueue:
                    // Just indicates clear queue command
                    await room.ClearQueueAsync();
                    break;

                case PacketType.CreateRoom:
                    await server.CreateRoomAndJoin(this, packet.Data.ToObject<CreateRoomData>());
                    break;

                case PacketType.SetUserData:
                    data = packet.Data.ToObject<SetUserData>(); // Just in case the server can for some reason change the user's data
                    handleUserData((SetUserData)data);
                    break;

                case PacketType.SkipSong:
                case PacketType.Done:
                    if (this.room.HostUserId == this.user.Id)
                    {
                        room.SongQueue.Dequeue();
                        await room.NextSongAsync();
                    }
                    break;

                case PacketType.EncodingData:
                    await this.SendPacketAsync(packet);
                    //await this.Room.BroadcastPacketAsync(packet);
                    break;

                case PacketType.QueueSong:
                    var song = new QueueSong()
                    {
                        Path = packet.Data.ToObject<QueueSongData>().Song,
                        UserId = this.user.Id
                    };

                    await room.QueueSongAsync(song);
                    break;
            }
        }
    }
}

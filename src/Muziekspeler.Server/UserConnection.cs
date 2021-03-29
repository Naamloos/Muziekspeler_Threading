using Muziekspeler.Common;
using Muziekspeler.Common.Packets;
using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.Server
{
    public class UserConnection
    {
        public Connection Connection;
        public User User;

        public UserConnection(TcpClient client)
        {
            Connection = new Connection(client, handlePacketAsync, null);
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
                    break;

                case PacketType.RoomList:
                    data = packet.Data.ToObject<RoomListData>();
                    break;

                case PacketType.JoinRoom:
                    data = packet.Data.ToObject<JoinRoomData>();
                    break;

                case PacketType.LeaveRoom:
                    // Has no data
                    data = packet.Data.ToObject<ReasonData>();
                    break;

                case PacketType.RoomUpdate:
                    data = packet.Data.ToObject<RoomUpdateData>();
                    break;

                case PacketType.KeepAlive:
                    // Just a keepalive, needs no data
                    break;

                case PacketType.PlayMusic:
                    // Just indicates a play command
                    break;

                case PacketType.PauseMusic:
                    // Just indicates pause command
                    break;

                case PacketType.ClearQueue:
                    // Just indicates clear queue command
                    break;

                case PacketType.SkipSong:
                    // Just indicates skip song command
                    break;

                case PacketType.CreateRoom:
                    data = packet.Data.ToObject<CreateRoomData>();
                    break;

                case PacketType.SetUserData:
                    data = packet.Data.ToObject<SetUserData>(); // Just in case the server can for some reason change the user's data
                    break;
            }
        }
    }
}

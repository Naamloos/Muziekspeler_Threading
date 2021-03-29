using Muziekspeler.Common;
using Muziekspeler.Common.Packets;
using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.UWP.Connectivity
{
    public class Client
    {
        public Connection ServerConnection;
        public User CurrentUser;
        public Room CurrentRoom;

        public Client()
        {

        }

        public async Task ConnectAsync()
        {
            var tcp = new TcpClient();
            tcp.Connect("127.0.0.1", 5069);

            this.ServerConnection = new Connection(tcp, handlePacketAsync, handleMediaAsync);
        }

        private async Task handleMediaAsync(byte[] data)
        {

        }

        private async Task handlePacketAsync(Packet packet)
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

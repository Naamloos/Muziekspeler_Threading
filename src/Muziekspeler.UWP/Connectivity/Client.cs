using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.MediaFoundation;
using CSCore.SoundOut;
using CSCore.Streams;
using Muziekspeler.Common;
using Muziekspeler.Common.Packets;
using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Storage;

namespace Muziekspeler.UWP.Connectivity
{
    public class Client
    {
        const string SERVER_HOST = "127.0.0.1"; // TODO host a small server instance instead?
        // events
        public delegate void OnRoomListUpdate(RoomListData data);
        public delegate void OnJoinRoom(JoinRoomData data);
        public delegate void OnLeaveRoom(ReasonData data);
        public delegate void OnChatMessage(ChatMessageData data);
        public delegate void OnRoomUpdate(Room data);
        public delegate void OnPlayMusic(StartPlayingData data);
        public delegate void OnPauseMusic();
        public delegate void OnUserUpdate(User currentUser);
        public delegate void OnServerFail(ReasonData data);

        public event OnRoomListUpdate RoomListUpdate;
        public event OnJoinRoom JoinRoom;
        public event OnLeaveRoom LeaveRoom;
        public event OnChatMessage ChatMessageReceived;
        public event OnRoomUpdate RoomUpdated;
        public event OnPlayMusic StartPlaying;
        public event OnPauseMusic PausePlaying;
        public event OnUserUpdate UserUpdated;
        public event OnServerFail ServerError;

        public Connection ServerConnection;
        public User CurrentUser = new User();
        public Room CurrentRoom;

        private bool isPlaying = false;

        private Client()
        {
        }

        public async Task ConnectAsync()
        {
            var tcp = new TcpClient();
            tcp.Connect(SERVER_HOST, 5069);
            ServerConnection = new Connection(tcp, handlePacketAsync);
            ServerConnection.StartClientLoop();

            await Task.Delay(1000);
        }

        private async Task handlePacketAsync(Packet packet) // TODO add behavior
        {
            object data = null;
            switch (packet.Type)
            {
                // After the data object is set you might need type casting to get its data.
                default:
                    throw new NotImplementedException("Packet type has no suitable handler!");

                case PacketType.ChatMessage:
                    ChatMessageReceived(packet.Data.ToObject<ChatMessageData>());
                    break;

                case PacketType.RoomList:
                    if (RoomListUpdate != null)
                        RoomListUpdate(packet.Data.ToObject<RoomListData>());
                    break;

                case PacketType.JoinRoom:
                    data = packet.Data.ToObject<JoinRoomData>();
                    CurrentRoom.Name = ((JoinRoomData)data).RoomName;
                    if (JoinRoom != null)
                        JoinRoom((JoinRoomData)data);
                    break;

                case PacketType.LeaveRoom:
                    if (LeaveRoom != null)
                        LeaveRoom(packet.Data.ToObject<ReasonData>());
                    break;

                case PacketType.RoomUpdate:
                    data = packet.Data.ToObject<RoomUpdateData>();
                    CurrentRoom.HostUserId = ((RoomUpdateData)data).HostId;
                    CurrentRoom.Users = ((RoomUpdateData)data).Users;
                    CurrentRoom.SongQueue = ((RoomUpdateData)data).Queue;
                    if (RoomUpdated != null)
                        RoomUpdated(CurrentRoom);
                    break;

                case PacketType.KeepAlive:
                    // Just a keepalive, needs no data
                    await ServerConnection.SendPacketAsync(new Packet(PacketType.KeepAlive, null));
                    break;

                case PacketType.PauseMusic:
                    // Just indicates pause command
                    if (PausePlaying != null)
                        PausePlaying();
                    break;

                case PacketType.ClearQueue:
                    // Just indicates clear queue command
                    await this.CurrentRoom.ClearQueueAsync();
                    if (RoomUpdated != null)
                        RoomUpdated(CurrentRoom);
                    break;

                case PacketType.SkipSong:
                    // Just indicates skip song command
                    if (isPlaying)
                        isPlaying = false;
                    await this.CurrentRoom.NextSongAsync();
                    break;

                case PacketType.CreateRoom:
                    data = packet.Data.ToObject<CreateRoomData>();
                    break;

                case PacketType.SetUserData:
                    data = packet.Data.ToObject<SetUserData>(); // Just in case the server can for some reason change the user's data
                    this.CurrentUser.DisplayName = ((SetUserData)data).DisplayName;
                    this.CurrentUser.Status = ((SetUserData)data).Status;
                    if (UserUpdated != null)
                        UserUpdated(CurrentUser);
                    break;

                case PacketType.UserId:
                    data = packet.Data.ToObject<UserIdData>();
                    CurrentUser.Id = ((UserIdData)data).Id;
                    if(UserUpdated != null)
                        UserUpdated(CurrentUser);
                    break;

                case PacketType.Done:
                    break;

                case PacketType.StartPlaying:
                    StartPlaying(packet.Data.ToObject<StartPlayingData>());
                    break;

                case PacketType.Fail:
                    if (ServerError != null)
                        ServerError(packet.Data.ToObject<ReasonData>());
                    break;
            }
        }

        private static Client _instance;
        public static Client Get()
        {
            if(_instance == null)
            {
                _instance = new Client();
                _ = Task.Run(async () => await _instance.ConnectAsync());
            }

            return _instance;
        }
    }
}

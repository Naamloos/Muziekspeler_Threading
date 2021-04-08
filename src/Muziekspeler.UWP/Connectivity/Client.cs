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
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.UWP.Connectivity
{
    public class Client
    {
        // events
        public delegate void OnRoomListUpdate(RoomListData data);
        public delegate void OnJoinRoom(JoinRoomData data);
        public delegate void OnLeaveRoom(ReasonData data);
        public delegate void OnChatMessage(ChatMessageData data);
        public delegate void OnRoomUpdate(Room data);
        public delegate void OnPlayMusic();
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

        //cscore APIs
        private WriteableBufferingSource audioSource;
        private WasapiOut audioOut;
        private bool isPlaying = false;

        public Client()
        {
        }

        public async Task ConnectAsync()
        {
            var tcp = new TcpClient();
            tcp.Connect("127.0.0.1", 5069);
            _ = Task.Run(async () => await StartMusicPlayer());
            ServerConnection = new Connection(tcp, handlePacketAsync, handleMediaAsync);
            ServerConnection.StartClientLoop();
        }

        private async Task StartMusicPlayer()
        {
            audioSource = new WriteableBufferingSource(new WaveFormat(4000, 16, 1)) { FillWithZeros = true };
            audioOut = new WasapiOut();
            audioOut.Initialize(audioSource);
            audioOut.Play();
        }

        public async Task StartPlayingAsync(string mp3file)
        {
            FileStream fs = File.OpenRead(mp3file);
            var mp3 = new DmoMp3Decoder(fs);
            _ = Task.Run(async () => await startPlayingAsync(mp3));
        }

        private async Task startPlayingAsync(DmoMp3Decoder mp3)
        {
            this.isPlaying = true;
            while(mp3.GetPosition() < mp3.GetLength() && this.isPlaying)
            {
                // make one sample buffer
                var buffer = new byte[mp3.WaveFormat.BytesPerSample];
                // read one sample
                mp3.Read(buffer, 0, buffer.Length);
                // send one sample
                await ServerConnection.SendDataAsync(buffer);
                // sleep for sample rate
                await Task.Delay(mp3.WaveFormat.SampleRate / 1000); // converting hertz (s) to milliseconds
            }
            await ServerConnection.SendPacketAsync(new Packet(PacketType.Done, null));
            this.isPlaying = false;
        }

        private async Task handleMediaAsync(byte[] data)
        {
            // write received data to writablebufferingsource.
            this.audioSource.Write(data, 0, data.Length);
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
                    data = packet.Data.ToObject<ChatMessageData>();
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

                case PacketType.PlayMusic:
                    // Just indicates a play command
                    if (StartPlaying != null)
                        StartPlaying();
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
                    data = packet.Data.ToObject<StartPlayingData>();
                    await this.StartPlayingAsync(((StartPlayingData)data).SongToPlay.Path);
                    break;

                case PacketType.Fail:
                    if (ServerError != null)
                        ServerError(packet.Data.ToObject<ReasonData>());
                    break;
            }
        }
    }
}

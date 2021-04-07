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
using Xabe.FFmpeg;

namespace Muziekspeler.UWP.Connectivity
{
    public class Client
    {
        public Connection ServerConnection;
        public User CurrentUser;
        public Room CurrentRoom;

        //cscore APIs
        private WriteableBufferingSource audioSource;
        private WasapiOut audioOut;

        public Client()
        {
        }

        public async Task ConnectAsync()
        {
            var tcp = new TcpClient();
            tcp.Connect("127.0.0.1", 5069);

            ServerConnection = new Connection(tcp, handlePacketAsync, handleMediaAsync);
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
            while(mp3.GetPosition() < mp3.GetLength())
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
                    await ServerConnection.SendPacketAsync(new Packet(PacketType.KeepAlive, null));
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

                case PacketType.UserId:
                    data = packet.Data.ToObject<UserIdData>();
                    break;

                case PacketType.Done:
                    break;

                case PacketType.StartPlaying:
                    data = packet.Data.ToObject<StartPlayingData>();
                    await this.StartPlayingAsync(((StartPlayingData)data).SongToPlay.Path);
                    break;

                case PacketType.Fail:
                    break;
            }
        }
    }
}

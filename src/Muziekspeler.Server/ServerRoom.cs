using Muziekspeler.Common.Packets;
using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.Server
{
    public class ServerRoom : Room
    {
        private Server server;

        public ServerRoom(Server server)
        {
            this.server = server;
        }

        public async Task BroadcastPacketAsync(Packet packet) => await server.BroadcastRoomAsync(this, packet);

        public async Task BroadcastDataAsync(byte[] data) => await server.BroadcastRoomDataAsync(this, data);

        public override async Task ClearQueueAsync()
        {
            this.SongQueue.Clear();
            await BroadcastPacketAsync(new Packet(PacketType.ClearQueue, null));
        }

        public override async Task NextSongAsync()
        {
            SongQueue.Dequeue();
            var song = SongQueue.Peek();
            await BroadcastPacketAsync(new Packet(PacketType.RoomUpdate, new RoomUpdateData()
            {
                HostId = this.HostUserId,
                Queue = this.SongQueue,
                Users = this.Users
            }));
            var user = this.Users.FirstOrDefault(x => x.Id == song.UserId);
            if(user != null)
            {
                await server.SendPacketToUserAsync(user.Id, new Packet(PacketType.StartPlaying, new StartPlayingData()
                {
                    SongToPlay = song
                }));
            }
        }

        public override async Task PauseMusicAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task PreviousSongAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task QueueSongAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task StartMusicAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task StopMusicAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task HandleChatAsync(ChatMessageData chat)
        {
            await BroadcastPacketAsync(new Packet(PacketType.ChatMessage, chat));
        }
    }
}

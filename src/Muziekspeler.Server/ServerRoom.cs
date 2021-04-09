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
        private bool done = false;

        public ServerRoom(Server server)
        {
            this.server = server;
            this.Users = new List<User>();
            this.SongQueue = new Queue<QueueSong>();
        }

        public async Task BroadcastPacketAsync(Packet packet) => await server.BroadcastRoomAsync(this, packet);

        //public async Task BroadcastDataAsync(byte[] data) => await server.BroadcastRoomDataAsync(this, data);

        public override async Task ClearQueueAsync()
        {
            this.SongQueue.Clear();
            await BroadcastPacketAsync(new Packet(PacketType.ClearQueue, null));
        }

        public override async Task NextSongAsync()
        {
            if (SongQueue.Count() <= 0)
                return;

            var song = SongQueue.Peek();

            await server.sendRoomUpdate(this);

            await server.BroadcastRoomAsync(this, new Packet(PacketType.StartPlaying, new StartPlayingData()
            {
                SongToPlay = song
            }));
        }

        public async Task QueueSongAsync(QueueSong song)
        {
            this.SongQueue.Enqueue(song);

            if (SongQueue.Count() == 1)
                await NextSongAsync();
            await server.sendRoomUpdate(this);
        }

        public async Task HandleChatAsync(string chat)
        {
            await BroadcastPacketAsync(new Packet(PacketType.ChatMessage, new ChatMessageData() { Message = chat }));
        }
    }
}

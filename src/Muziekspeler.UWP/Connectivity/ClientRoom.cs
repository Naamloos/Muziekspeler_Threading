using Muziekspeler.Common.Packets;
using Muziekspeler.Common.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muziekspeler.UWP.Connectivity
{
    public class ClientRoom : Room
    {
        private Client client;

        public ClientRoom(string name, Client client)
        {
            this.Name = name;
            this.client = client;
        }

        public void UpdateRoomData(RoomUpdateData data)
        {
            this.SongQueue = data.Queue;
            this.Users = data.Users;
            this.HostUserId = data.HostId;
        }

        public override async Task ClearQueueAsync()
        {
            await client.ServerConnection.SendPacketAsync(new Packet(PacketType.ClearQueue, null));
        }

        public override async Task NextSongAsync()
        {
            await client.ServerConnection.SendPacketAsync(new Packet(PacketType.SkipSong, null));
        }

        public async Task QueueSongAsync(string url)
        {
            await client.ServerConnection.SendPacketAsync(new Packet(PacketType.QueueSong, new QueueSongData() { Song = url }));
        }
    }
}

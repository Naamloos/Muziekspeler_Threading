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

        private async Task BroadcastPacketAsync(Packet packet) => await server.BroadcastRoomAsync(this, packet);

        public override async Task ClearQueueAsync()
        {
            this.SongQueue.Clear();
            await BroadcastPacketAsync(new Packet(PacketType.ClearQueue, null));
        }

        public override async Task NextSongAsync()
        {
            throw new NotImplementedException();
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
    }
}

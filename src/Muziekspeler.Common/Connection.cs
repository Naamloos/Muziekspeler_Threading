using Muziekspeler.Common.Packets;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Muziekspeler.Common
{
    public class Connection
    {
        private TcpClient packetStream;
        private TcpClient mediaStream;
        CancellationToken cancellation;

        public delegate Task PacketHandlerAsync(Packet packet);
        public delegate Task MediaHandlerAsync(Packet packet);

        public PacketHandlerAsync PacketReceived;
        public MediaHandlerAsync MediaReceived;

        public async Task StartClientLoopAsync()
        {

        }

        public async Task StopClientLoopAsync()
        {

        }

        public async Task SendPacketAsync(Packet packet)
        {

        }

        private async Task handlePacketAsync()
        {

        }
    }
}
